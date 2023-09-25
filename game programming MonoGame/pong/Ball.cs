using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace pong
{
    class Ball : GameObject
    {
        SoundEffect hitSound, edgeHitSound, borderHitSound;

        //variables for adjusting the feel of the game
        float speedMultiplier, startSpeed, playerAngleScaler;

        //variables for flickering
        float lastFlicker, flickerTime;
        int totalFlickers;
        bool flicker, renderBall;

        public Ball(Vector2 _startPosition, ContentManager _content, Pong _pong) : base(_content, "ball", _startPosition, _pong)
        {
            hitSound = _content.Load<SoundEffect>("paddleHitSound");
            edgeHitSound = _content.Load<SoundEffect>("edgeHitSound");
            borderHitSound = _content.Load<SoundEffect>("borderHitSound");
            speedMultiplier = 1.03f;
            startSpeed = 320f;
            playerAngleScaler = 0.5f;
            flickerTime = 0.33f;
        }

        public override void Update(GameTime gameTime)
        {
            if (flicker == true)
            {
                Flicker(gameTime);
            }
            else
            {
                base.Update(gameTime);
                CheckCollision(gameTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch) 
        {
            if (renderBall)
                base.Draw(spriteBatch);
        }
        void CheckCollision(GameTime _gameTime)
        {
            foreach (Player player in pong.Players)
            {
                if(player.IsAlive)
                    CheckPaddle(player);
            }
            CheckVerticalBorders();
            CheckHorizontalBorders();
        }

        void CheckVerticalBorders()
        {
            if (!pong.IsFourPlayers)
            {
                if (position.X <= -origin.X)
                {
                    Reset();
                    foreach (Player player in pong.Players)
                    {
                        player.Reset();
                    }
                    pong.Players[0].TakeDamage(1);
                }
                if (position.X >= Pong.screenSize.X + origin.X)
                {
                    Reset();
                    foreach (Player player in pong.Players)
                    {
                        player.Reset();
                    }
                    pong.Players[1].TakeDamage(1);
                }
            }
            else
            {
                //create variables for checking whether the ball bounces with the side while calculating the correct collision position
                Vector2 directionVector = position - lastPosition;
                Vector2? intersectionTop = CollisionHelper.VerticalIntersection(lastPosition, directionVector, origin.X, null, null);
                Vector2? intersectionBottom = CollisionHelper.VerticalIntersection(lastPosition, directionVector, Pong.screenSize.X - origin.Y, null, null);

                //Check whether a collision should occur
                if (intersectionTop.HasValue || intersectionBottom.HasValue)
                {
                    Vector2 collisionPosition;
                    if (intersectionTop.HasValue)
                    {
                        collisionPosition = intersectionTop.Value;
                        if (pong.Players[0].IsAlive)
                        {
                            pong.Players[0].TakeDamage(1);
                            Reset();
                            foreach (Player player in pong.Players)
                            {
                                player.Reset();
                            }
                            return;
                        }
                    }
                    else
                    {
                        collisionPosition = intersectionBottom.Value;
                        if (pong.Players[1].IsAlive)
                        {
                            pong.Players[1].TakeDamage(1);
                            Reset();
                            foreach (Player player in pong.Players)
                            {
                                player.Reset();
                            }
                            return;
                        }
                    }
                    //bounce 
                    velocity.X = -velocity.X;
                    borderHitSound.Play();

                    //code for making up for lost distance from phasing through the border(noticable at high speeds)
                    Vector2 lostDistance = position - collisionPosition;
                    position = collisionPosition;
                    lastPosition = position;
                    position += new Vector2(-lostDistance.X, lostDistance.Y);
                }
            }
        }

        void CheckHorizontalBorders()
        {
            //create variables for checking whether the ball bounces with the side while calculating the correct collision position
            Vector2 directionVector = position - lastPosition;
            Vector2? intersectionTop = CollisionHelper.HorizontalIntersection(lastPosition, directionVector, origin.Y, null, null);
            Vector2? intersectionBottom = CollisionHelper.HorizontalIntersection(lastPosition, directionVector, Pong.screenSize.Y - origin.Y, null, null);

            //Check whether a collision should occur
            if (intersectionTop.HasValue || intersectionBottom.HasValue)
            {
                Vector2 collisionPosition;
                if (intersectionTop.HasValue)
                {
                    collisionPosition = intersectionTop.Value;
                    if (pong.IsFourPlayers && pong.Players[2].IsAlive)
                    {
                        pong.Players[2].TakeDamage(1);
                        Reset();
                        foreach (Player player in pong.Players)
                        {
                            player.Reset();
                        }
                        return;
                    }
                }
                else
                {
                    collisionPosition = intersectionBottom.Value;
                    if (pong.IsFourPlayers && pong.Players[3].IsAlive)
                    {
                        pong.Players[3].TakeDamage(1);
                        Reset();
                        foreach (Player player in pong.Players)
                        {
                            player.Reset();
                        }
                        return;
                    }
                }

                //bounce 
                velocity.Y = -velocity.Y;
                borderHitSound.Play();

                //code for making up for lost distance from phasing through the border(noticable at high speeds)
                Vector2 lostDistance = position - collisionPosition;
                position = collisionPosition;
                lastPosition = position;
                position += new Vector2(lostDistance.X, -lostDistance.Y);      

            }
        }

        void CheckPaddle(Player player)
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
                    Vector2? playerBallIntersectionTop = CollisionHelper.HorizontalIntersection(topLeftLastBound, topLeftBound - topLeftLastBound, position.Y, null, null);
                    Vector2? playerLastBallIntersectionTop = CollisionHelper.HorizontalIntersection(topLeftLastBound, topLeftBound - topLeftLastBound, lastPosition.Y, null, null);
                    Vector2? playerBallIntersectionBottom = CollisionHelper.HorizontalIntersection(bottomRightLastBound, bottomRightBound - bottomRightLastBound, position.Y, null, null);
                    Vector2? playerLastBallIntersectionBottom = CollisionHelper.HorizontalIntersection(bottomRightLastBound, bottomRightBound - bottomRightLastBound, lastPosition.Y, null, null);
                
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
                    Vector2? playerBallIntersectionLeft = CollisionHelper.VerticalIntersection(topLeftLastBound, topLeftBound - topLeftLastBound, position.X, null, null);
                    Vector2? playerLastBallIntersectionLeft = CollisionHelper.VerticalIntersection(topLeftLastBound, topLeftBound - topLeftLastBound, lastPosition.X, null, null);
                    Vector2? playerBallIntersectionRigth = CollisionHelper.VerticalIntersection(bottomRightLastBound, bottomRightBound - bottomRightLastBound, position.X, null, null);
                    Vector2? playerLastBallIntersectionRight = CollisionHelper.VerticalIntersection(bottomRightLastBound, bottomRightBound - bottomRightLastBound, lastPosition.X, null, null);

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
            Vector2? boxIntersection = CollisionHelper.BoxIntersection(lastPosition, position - lastPosition, topLeftBound, bottomRightBound);
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

                    //check for collision near edge of player
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

                    //check for collision near edge of player
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
            //calculate random direction that excludes directions that are close to exclusively vertical
            int x;
            int y = Pong.Random.Next(-10, 10);

            int minimumX;
            if (pong.IsFourPlayers)
                minimumX = 0;
            else
                minimumX = 5;

            if (Pong.Random.Next(2) == 0)
                x = Pong.Random.Next(-10, -minimumX);
            else
                x = Pong.Random.Next(minimumX, 10);

            //assign random direction to velocity
            velocity = new Vector2(x, y);
            velocity.Normalize();
            velocity *= startSpeed;
        }
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
       
    }
}

