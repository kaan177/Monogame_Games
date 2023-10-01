using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace pong
{
    internal class Ball : MovingGameObject
    {
        //The class Ball is responsible for moving and displaying the ball, as well as handling all collisions between the ball, players and power ups
        SoundEffect hitSound, edgeHitSound, borderHitSound;

        //variables for adjusting the feel of the game
        float speedMultiplier, startSpeed, playerAngleScaler;

        //variables for flickering
        float lastFlicker, flickerTime;
        int totalFlickers;
        bool flicker, renderBall;

        int lastPlayerHit;

        public Ball(Vector2 _startPosition, ContentManager _content, Pong _pong) : base(_content, "ball", _startPosition, _pong)
        {
            hitSound = _content.Load<SoundEffect>("paddleHitSound");
            edgeHitSound = _content.Load<SoundEffect>("edgeHitSound");
            borderHitSound = _content.Load<SoundEffect>("borderHitSound");
            speedMultiplier = 1.02f;
            startSpeed = 320f;
            playerAngleScaler = 0.5f;
            flickerTime = 0.33f;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------------------

        public override void Update(GameTime gameTime)
        {
            if (flicker == true)
            {
                Flicker(gameTime);
            }
            else
            {
                //check if the ball should swerve
                if (pong.PowerUps.ActivePowerUp == PowerUps.PowerUp.swerve)
                {
                    //save the current speed
                    float currentSpeed = velocity.Length();

                    //save the current angle
                    float directionAngle = (float)Math.Atan2(velocity.Y, velocity.X);

                    //calculate a new angle with a Sin based on the totalseconds
                    directionAngle += 50f / currentSpeed * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds * 15f);

                    //calculate the new velocity direction based of the result of the sin calculation 
                    velocity = new Vector2(MathF.Cos(directionAngle), MathF.Sin(directionAngle));
                    velocity.Normalize();

                    //set the velocity back to its initial speed
                    velocity *= currentSpeed;
                }
                //update the position and last position
                base.Update(gameTime);
                
                //check for collisions
                CheckCollision(gameTime);
            }
        }

        //draw the ball if it should be drawn based on renderBall which is altered in method  Flicker()
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (renderBall)
                base.Draw(spriteBatch);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------------------

        //Code to check for all the types of collisions, if any of the collision checks return true all collisions should be checked again with the updated last position and position from that collision
        void CheckCollision(GameTime _gameTime)
        {
            bool checkForCollisions = true;
            while (checkForCollisions)
            {
                //standard behaviour stop looping
                checkForCollisions = false;

                //check all alive/active players 
                foreach (Player player in pong.Players)
                {
                    if (player.IsAlive)
                        if (CheckPaddle(player))
                        {
                            //make sure to check for new collisions afterwards as CheckPaddle has been called and it has collided so the last position and position of ball have been updated
                            checkForCollisions = true;
                            foreach(Player potentialBot in pong.Players)
                            {
                                //let the bots recalculate their optimal position as the direction of the ball has changed
                                if(potentialBot is Bot)
                                {
                                    Bot bot = (Bot)potentialBot;
                                    bot.NeedsVelocityRecalculation = true;
                                }
                            }
                        }
                }
                if (CheckVerticalBorders() || CheckHorizontalBorders())
                {
                    //make sure to check for new collisions afterwards as Check Borders have been called and the ball has collided so the last position and position of ball have been updated
                    checkForCollisions = true;
                    foreach (Player potentialBot in pong.Players)
                    {
                        //let the bots recalculate their optimal position as the direction of the ball has changed
                        if (potentialBot is Bot)
                        {
                            Bot bot = (Bot)potentialBot;
                            bot.NeedsVelocityRecalculation = true;
                        }
                    }
                }
                //check for collisions with the power up object if it is visible
                if (pong.PowerUps.IsVisible)
                    CheckPowerUps();
            }
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------------------

        bool CheckVerticalBorders()
        {
            //create variables for checking whether the ball bounces with the side while calculating the correct collision position
            Vector2 directionVector = position - lastPosition;
            Vector2? intersectionLeft = CollisionHelper.VerticalIntersection(lastPosition, directionVector, origin.X, null, null, 1f);
            Vector2? intersectionRight = CollisionHelper.VerticalIntersection(lastPosition, directionVector, Pong.screenSize.X - origin.X, null, null, 1f);

            //Check whether a bounce should occur
            if (intersectionLeft.HasValue || intersectionRight.HasValue)
            {
                Vector2 collisionPosition;
                if (intersectionLeft.HasValue)
                {
                    //if this side has collided, check if there are players alive on that side and if there is not a shield, if so damage the player and reset the playing field
                    collisionPosition = intersectionLeft.Value;
                    if ((pong.Players[0].IsAlive || pong.Players[4].IsAlive) && pong.PowerUps.ActiveShield != PowerUps.Shield.leftShield)
                    {
                        pong.Players[0].TakeDamage(1);
                        pong.Players[4].TakeDamage(1);
                        Reset();
                        pong.PowerUps.Reset();
                        foreach (Player player in pong.Players)
                        {
                            player.Reset();
                        }
                        //returns false as nothing should be done with the information that the ball has collided as the game should reset
                        return false;
                    }
                    //break shield if it is there
                    if (pong.PowerUps.ActiveShield == PowerUps.Shield.leftShield)
                        pong.PowerUps.BreakShield();
                }
                else
                {
                    //if this side has collided, check if there are players alive on that side and if there is not a shield, if so damage the player and reset the playing field
                    collisionPosition = intersectionRight.Value;
                    if ((pong.Players[1].IsAlive || pong.Players[5].IsAlive) && pong.PowerUps.ActiveShield != PowerUps.Shield.rightShield)
                    {
                        pong.Players[1].TakeDamage(1);
                        pong.Players[5].TakeDamage(1);
                        Reset();
                        pong.PowerUps.Reset();
                        foreach (Player player in pong.Players)
                        {
                            player.Reset();
                        }
                        //returns false as nothing should be done with the information that the ball has collided as the game should reset
                        return false;
                    }
                    //break shield if it is there
                    if (pong.PowerUps.ActiveShield == PowerUps.Shield.rightShield)
                        pong.PowerUps.BreakShield();
                }
                //bounce 
                velocity.X = -velocity.X;
                borderHitSound.Play();

                //code for making up for lost distance from phasing through the border(noticable at high speeds)
                Vector2 lostDistance = position - collisionPosition;
                position = collisionPosition;
                lastPosition = position;
                position += new Vector2(-lostDistance.X, lostDistance.Y);

                return true;
            }

            //perform secondary check in case the more complex collision system fails, to avoid a stalemate where the ball can never collide 
            if(position.X < origin.X || position.X > Pong.screenSize.X - origin.X)
            {
                Vector2? collisionPosition;
                if (position.X < origin.X)
                {
                    //if this side has collided, check if there are players alive on that side and if there is not a shield, if so damage the player and reset the playing field
                    collisionPosition = CollisionHelper.VerticalIntersection(position, (lastPosition - position), origin.X, origin.Y, Pong.screenSize.Y - origin.Y, null);
                    if ((pong.Players[0].IsAlive || pong.Players[4].IsAlive) && pong.PowerUps.ActiveShield != PowerUps.Shield.leftShield || collisionPosition == null)
                    {
                        pong.Players[0].TakeDamage(1);
                        pong.Players[4].TakeDamage(1);
                        Reset();
                        pong.PowerUps.Reset();
                        foreach (Player player in pong.Players)
                        {
                            player.Reset();
                        }
                        //returns false as nothing should be done with the information that the ball has collided as the game should reset
                        return false;
                    }
                    //break shield if it is there
                    if (pong.PowerUps.ActiveShield == PowerUps.Shield.leftShield)
                        pong.PowerUps.BreakShield();
                }
                else
                {
                    //if this side has collided, check if there are players alive on that side and if there is not a shield, if so damage the player and reset the playing field
                    collisionPosition = CollisionHelper.VerticalIntersection(position, (lastPosition - position), Pong.screenSize.X - origin.X, origin.Y, Pong.screenSize.Y - origin.Y, null);
                    if ((pong.Players[1].IsAlive || pong.Players[5].IsAlive) && pong.PowerUps.ActiveShield != PowerUps.Shield.rightShield || collisionPosition == null)
                    {
                        pong.Players[1].TakeDamage(1);
                        pong.Players[5].TakeDamage(1);
                        Reset();
                        pong.PowerUps.Reset();
                        foreach (Player player in pong.Players)
                        {
                            player.Reset();
                        }
                        //returns false as nothing should be done with the information that the ball has collided as the game should reset
                        return false;
                    }
                    //break shield if it is there
                    if (pong.PowerUps.ActiveShield == PowerUps.Shield.rightShield)
                        pong.PowerUps.BreakShield();
                }
                //bounce 
                velocity.X = -velocity.X;
                borderHitSound.Play();

                //code for making up for lost distance from phasing through the border(noticable at high speeds)
                Vector2 lostDistance = position - collisionPosition.Value;
                position = collisionPosition.Value;
                lastPosition = position;
                position += new Vector2(-lostDistance.X, lostDistance.Y);

                return true;
            }
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------------------

        bool CheckHorizontalBorders()
        {
            //create variables for checking whether the ball bounces with the side while calculating the correct collision position
            Vector2 directionVector = position - lastPosition;
            Vector2? intersectionTop = CollisionHelper.HorizontalIntersection(lastPosition, directionVector, origin.Y, null, null, 1f);
            Vector2? intersectionBottom = CollisionHelper.HorizontalIntersection(lastPosition, directionVector, Pong.screenSize.Y - origin.Y, null, null, 1f);

            //Check whether a collision should occur
            if (intersectionTop.HasValue || intersectionBottom.HasValue)
            {
                Vector2 collisionPosition;
                if (intersectionTop.HasValue)
                {
                    //if this side has collided, check if there are players alive on that side and if there is not a shield, if so damage the player and reset the playing field
                    collisionPosition = intersectionTop.Value;
                    if ((pong.IsFourPlayers && pong.Players[2].IsAlive || pong.Players[6].IsAlive) && pong.PowerUps.ActiveShield != PowerUps.Shield.topShield)
                    {
                        pong.Players[2].TakeDamage(1);
                        pong.Players[6].TakeDamage(1);
                        Reset();
                        pong.PowerUps.Reset();
                        foreach (Player player in pong.Players)
                        {
                            player.Reset();
                        }
                        //returns false as nothing should be done with the information that the ball has collided as the game should reset
                        return false;
                    }
                    //break shield if it is there
                    if (pong.PowerUps.ActiveShield == PowerUps.Shield.topShield)
                        pong.PowerUps.BreakShield();
                }
                else
                {
                    //if this side has collided, check if there are players alive on that side and if there is not a shield, if so damage the player and reset the playing field
                    collisionPosition = intersectionBottom.Value;
                    if ((pong.IsFourPlayers && pong.Players[3].IsAlive || pong.Players[7].IsAlive) && pong.PowerUps.ActiveShield != PowerUps.Shield.bottomShield)
                    {
                        pong.Players[3].TakeDamage(1);
                        pong.Players[7].TakeDamage(1);
                        Reset();
                        pong.PowerUps.Reset();
                        foreach (Player player in pong.Players)
                        {
                            player.Reset();
                        }
                        //returns false as nothing should be done with the information that the ball has collided as the game should reset
                        return false;
                    }
                    //break shield if it is there
                    if (pong.PowerUps.ActiveShield == PowerUps.Shield.bottomShield)
                        pong.PowerUps.BreakShield();
                }
                //bounce 
                velocity.Y = -velocity.Y;
                borderHitSound.Play();

                //code for making up for lost distance from phasing through the border(noticable at high speeds)
                Vector2 lostDistance = position - collisionPosition;
                position = collisionPosition;
                lastPosition = position;
                position += new Vector2(lostDistance.X, -lostDistance.Y);

                return true;
            }

            //perform secondary check in case the more complex collision system fails, to avoid a stalemate where the ball can never collide 
            if (position.Y < origin.Y || position.Y > Pong.screenSize.Y - origin.Y)
            {
                Vector2? collisionPosition;
                if (position.Y < origin.Y)
                {
                    //if this side has collided, check if there are players alive on that side and if there is not a shield, if so damage the player and reset the playing field
                    collisionPosition = CollisionHelper.HorizontalIntersection(position, (lastPosition - position), origin.Y, origin.X, Pong.screenSize.X - origin.X, null);
                    if ((pong.Players[2].IsAlive || pong.Players[6].IsAlive) && pong.PowerUps.ActiveShield != PowerUps.Shield.topShield || collisionPosition == null)
                    {
                        pong.Players[2].TakeDamage(1);
                        pong.Players[6].TakeDamage(1);
                        Reset();
                        pong.PowerUps.Reset();
                        foreach (Player player in pong.Players)
                        {
                            player.Reset();
                        }
                        //returns false as nothing should be done with the information that the ball has collided as the game should reset
                        return false;
                    }
                    //break shield if it is there
                    if (pong.PowerUps.ActiveShield == PowerUps.Shield.topShield)
                        pong.PowerUps.BreakShield();
                }
                else
                {
                    //if this side has collided, check if there are players alive on that side and if there is not a shield, if so damage the player and reset the playing field
                    collisionPosition = CollisionHelper.HorizontalIntersection(position, (lastPosition - position), Pong.screenSize.Y - origin.Y, origin.X, Pong.screenSize.X - origin.X, null);
                    if ((pong.Players[3].IsAlive || pong.Players[7].IsAlive) && pong.PowerUps.ActiveShield != PowerUps.Shield.bottomShield || collisionPosition == null)
                    {
                        pong.Players[3].TakeDamage(1);
                        pong.Players[7].TakeDamage(1);
                        Reset();
                        pong.PowerUps.Reset();
                        foreach (Player player in pong.Players)
                        {
                            player.Reset();
                        }
                        //returns false as nothing should be done with the information that the ball has collided as the game should reset
                        return false;
                    }
                    //break shield if it is there
                    if (pong.PowerUps.ActiveShield == PowerUps.Shield.bottomShield)
                        pong.PowerUps.BreakShield();
                }
                //bounce 
                velocity.Y = -velocity.Y;
                borderHitSound.Play();

                //code for making up for lost distance from phasing through the border(noticable at high speeds)
                Vector2 lostDistance = position - collisionPosition.Value;
                position = collisionPosition.Value;
                lastPosition = position;
                position += new Vector2(lostDistance.X, -lostDistance.Y);

                return true;
            }
            return false;
        }

        //-----------------------------------------------------------------------------------------------------------------------------------------------
        //-----------------------------------------------------------------------------------------------------------------------------------------------

        bool CheckPaddle(Player player)
        {
            //create a bounding box that is slightly larger than the actual bounding box of the player to accomodate for the ball colliding at the centre only
            Vector2 topLeftBound = new Vector2(player.OriginAdjustedPosition.X - origin.X, player.OriginAdjustedPosition.Y - origin.Y);
            Vector2 bottomRightBound = new Vector2(player.OriginAdjustedPosition.X + origin.X + player.Width, player.OriginAdjustedPosition.Y + origin.Y + player.Height);
            Vector2 topLeftLastBound = new Vector2(player.OriginAdjustedLastPosition.X - origin.X, player.OriginAdjustedLastPosition.Y - origin.Y);
            Vector2 bottomRightLastBound = new Vector2(player.OriginAdjustedLastPosition.X + origin.X + player.Width, player.OriginAdjustedLastPosition.Y + origin.Y + player.Height);

            //check if the player has moved through the ball between frames and if so move the ball by the same amount the player moved. This is to prevent the ball from getting stuck inside the player. Also check for the last position of the ball to catch edge cases
            //note: the bouncing isn't calculated here, even though its likely that it should. The reasons are that the exact collision position remains unknown and that the player might move in the same vertical direction as the ball
            if (player.IsVertical)
            {
                if ((position.X > topLeftBound.X || lastPosition.X > topLeftBound.X) && (position.X < bottomRightBound.X || lastPosition.X < bottomRightBound.X))
                {
                    Vector2? playerBallIntersectionTop = CollisionHelper.HorizontalIntersection(topLeftLastBound, topLeftBound - topLeftLastBound, position.Y, null, null, 1f);
                    Vector2? playerLastBallIntersectionTop = CollisionHelper.HorizontalIntersection(topLeftLastBound, topLeftBound - topLeftLastBound, lastPosition.Y, null, null, 1f);
                    Vector2? playerBallIntersectionBottom = CollisionHelper.HorizontalIntersection(bottomRightLastBound, bottomRightBound - bottomRightLastBound, position.Y, null, null, 1f);
                    Vector2? playerLastBallIntersectionBottom = CollisionHelper.HorizontalIntersection(bottomRightLastBound, bottomRightBound - bottomRightLastBound, lastPosition.Y, null, null, 1f);

                    //check if the paddle has indeed collided
                    if (playerBallIntersectionTop.HasValue || playerBallIntersectionBottom.HasValue || playerLastBallIntersectionTop.HasValue || playerLastBallIntersectionBottom.HasValue)
                    {
                        //add the amount the paddle has moved to the ball to resolve the issue
                        lastPosition += (bottomRightBound - bottomRightLastBound);
                        position += (bottomRightBound - bottomRightLastBound);
                    }
                }
            }
            else
            {
                if ((position.Y > topLeftBound.Y || lastPosition.Y > topLeftBound.Y) && (position.Y < bottomRightBound.Y || lastPosition.Y < bottomRightBound.Y))
                {
                    Vector2? playerBallIntersectionLeft = CollisionHelper.VerticalIntersection(topLeftLastBound, topLeftBound - topLeftLastBound, position.X, null, null, 1f);
                    Vector2? playerLastBallIntersectionLeft = CollisionHelper.VerticalIntersection(topLeftLastBound, topLeftBound - topLeftLastBound, lastPosition.X, null, null, 1f);
                    Vector2? playerBallIntersectionRigth = CollisionHelper.VerticalIntersection(bottomRightLastBound, bottomRightBound - bottomRightLastBound, position.X, null, null, 1f);
                    Vector2? playerLastBallIntersectionRight = CollisionHelper.VerticalIntersection(bottomRightLastBound, bottomRightBound - bottomRightLastBound, lastPosition.X, null, null, 1f);

                    //check if the paddle has indeed collided
                    if (playerBallIntersectionLeft.HasValue || playerBallIntersectionRigth.HasValue || playerLastBallIntersectionLeft.HasValue || playerLastBallIntersectionRight.HasValue)
                    {
                        //add the amount the paddle has moved to the ball to resolve the issue
                        lastPosition += (bottomRightBound - bottomRightLastBound);
                        position += (bottomRightBound - bottomRightLastBound);
                    }
                }
            }

            //check for collision with player
            Vector2? boxIntersection = CollisionHelper.BoxIntersection(lastPosition, position - lastPosition, topLeftBound, bottomRightBound, 1f);
            if (boxIntersection.HasValue)
            {
                Vector2 collisionPosition = boxIntersection.Value;
                float extraY = 0f;
                float extraX = 0f;
                //check whether ball hits from top OR bottom
                if (topLeftBound.X < collisionPosition.X && collisionPosition.X < bottomRightBound.X)
                {
                    //bounce
                    velocity.Y = -velocity.Y;

                    //check for collision near edge of player, if so the ball should be directed towards the side of that edge
                    float distanceToMiddle = (player.OriginAdjustedPosition.X + player.Width / 2 - collisionPosition.X);
                    if (Math.Abs(distanceToMiddle) > player.Width / 4)
                    {
                        //calculate extra X velocity that still needs adjustment for currentspeed      
                        extraX = -Math.Sign(distanceToMiddle) * playerAngleScaler;

                        edgeHitSound.Play();
                    }
                    else
                        hitSound.Play();
                }
                else
                {
                    //bounce
                    velocity.X = -velocity.X;

                    //check for collision near edge of player, if so the ball should be directed towards the side of that edge
                    float distanceToMiddle = (player.OriginAdjustedPosition.Y + player.Height / 2 - collisionPosition.Y);
                    if (Math.Abs(distanceToMiddle) > player.Height / 4)
                    {
                        //calculate extra y velocity that still needs adjustment for currentspeed      
                        extraY = -Math.Sign(distanceToMiddle) * playerAngleScaler;

                        edgeHitSound.Play();
                    }
                    else
                        hitSound.Play();
                }
                velocity *= speedMultiplier;

                //calculate the distance that needs to be added to the position to make up for the lost distance from setting the position to the collision position
                float lostDistance = (position - collisionPosition).Length();

                //set position and lastposition to the place the ball is supposed to be
                position = collisionPosition;
                lastPosition = position;

                //add the extra y velocity and add the lost distance in the right direction
                float currentSpeed = velocity.Length();
                velocity.X += extraX * currentSpeed;
                velocity.Y += extraY * currentSpeed;
                velocity.Normalize();
                position += velocity * lostDistance;
                velocity *= currentSpeed;

                lastPlayerHit = player.PlayerId;
                return true;
            }
            return false;
        }

        //-------------------------------------------------------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------------------------------------------------------

        //check for collisions with the power up object
        void CheckPowerUps()
        {
            Vector2 topLeftBound = new Vector2(pong.PowerUps.Position.X - pong.PowerUps.Origin.X, pong.PowerUps.Position.Y - pong.PowerUps.Origin.Y);
            Vector2 bottomRightBound = new Vector2(pong.PowerUps.Position.X + pong.PowerUps.Origin.X, pong.PowerUps.Position.Y + pong.PowerUps.Origin.Y);
            Vector2? boxIntersection = CollisionHelper.BoxIntersection(lastPosition, position - lastPosition, topLeftBound, bottomRightBound, 1f);
            if (boxIntersection.HasValue)
            {
                //tell powerUps it has been hit and let it handle the power Up applying based on the last player the ball hit
                pong.PowerUps.GetHit(lastPlayerHit);
            }
        }
        public override void Reset()
        {
            base.Reset();
            flicker = true;
            renderBall = false;
            RandomDirection();
        }
        public void ReCalculateStartPosition()
        {
            startPosition = Pong.screenSize / 2;
        }
        void RandomDirection()
        {
            //calculate and set random direction that excludes directions that are close to exclusively vertical
            float x = 1;
            float y = Pong.Random.NextSingle() * 2 - 0.5f;

            //if the game is in four player mode, allow all angles by randomising x as well
            if (pong.IsFourPlayers)
                x = Pong.Random.NextSingle() * 2 - 0.5f;

            //assign random direction to velocity
            velocity = new Vector2(x, y);
            velocity.Normalize();
            velocity *= startSpeed;
        }

        //Gives the ball a visual flickering effect
        void Flicker(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalSeconds > lastFlicker)
            {
                lastFlicker = (float)gameTime.TotalGameTime.TotalSeconds + flickerTime;
                renderBall = !renderBall;
                totalFlickers++;
                if (totalFlickers > 6)
                {
                    flicker = false;
                    totalFlickers = 0;
                }
            }
        }

        public Vector2 Origin
        {
            get { return origin; }
        }
        public Vector2 Position
        {
            get { return position; }
        }
        public Vector2 LastPosition
        {
            get { return lastPosition; }
        }
        public int LastPlayerHit
        {
            get { return lastPlayerHit; }
        }
    }
}

