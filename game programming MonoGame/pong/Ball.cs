using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace pong
{
    class Ball : GameObject
    {
        SoundEffect hitSound, edgeHitSound, borderHitSound;

        Player player1, player2;

        //variable for keeping track of passed time
        double collisionTime;

        //variables for adjusting the feel of the game
        float speedMultiplier, startSpeed, paddleAngleScaler;

        //variables for flickering
        float lastFlicker, flickerTime;
        int totalFlickers;
        bool flicker, renderBall;

        public Ball(Vector2 _startPosition, ContentManager _content, Player _player1, Player _player2) : base(_content, "ball", _startPosition)
        {
            hitSound = _content.Load<SoundEffect>("paddleHitSound");
            edgeHitSound = _content.Load<SoundEffect>("edgeHitSound");
            borderHitSound = _content.Load<SoundEffect>("borderHitSound");
            player1 = _player1;
            player2 = _player2;
            speedMultiplier = 1.03f;
            startSpeed = 320f;
            paddleAngleScaler = 0.5f;
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
            //check whether a collision hasn't very recently occured to prevent the ball from getting stuck inside the paddle when the paddle moves into the ball at high speed
            if (_gameTime.TotalGameTime.TotalSeconds > collisionTime)
            {
                if (CheckPaddle(player1) || CheckPaddle(player2))
                    collisionTime = _gameTime.TotalGameTime.TotalSeconds + 0.1f;
            }
            CheckVerticalBorders();
            CheckHorizontalBorders();
        }

        void CheckVerticalBorders()
        {
            if (position.X + origin.X <= 0)
            {
                Reset();
                player1.Reset();
                player2.Reset();

                player1.TakeDamage(1);
            }
            if (position.X - origin.X >= Pong.screenSize.X)
            {
                Reset();
                player1.Reset();
                player2.Reset();

                player2.TakeDamage(1);
            }
        }

        void CheckHorizontalBorders()
        {
            //code for checking whether the ball bounces with the side and calculating the correct collision position
            Vector2 directionVector = position - lastPosition;
            Vector2? intersectionTop = XAxisIntersection(lastPosition, directionVector, origin.Y, null, null);
            Vector2? intersectionBottom = XAxisIntersection(lastPosition, directionVector, Pong.screenSize.Y - origin.Y, null, null);
            if (intersectionTop.HasValue || intersectionBottom.HasValue)
            {
                Vector2 collisionPosition;
                if(intersectionTop.HasValue)
                    collisionPosition = intersectionTop.Value;
                else
                    collisionPosition = intersectionBottom.Value;

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

        bool CheckPaddle(Player player)
        {
            //create the idea of a bounding box that is slightly larger than the actual bounding box of the padle to accomodate for the ball colliding at the centre only
            Vector2 playerTopLeftPos = new Vector2(player.OriginAdjustedPosition.X - origin.X, player.OriginAdjustedPosition.Y - origin.Y);
            Vector2 playerBottomRightPos = new Vector2(player.OriginAdjustedPosition.X + origin.X + player.Width, player.OriginAdjustedPosition.Y + origin.Y + player.Height);
            Vector2 playerTopLeftLastPos = new Vector2(player.OriginAdjustedLastPosition.X - origin.X, player.OriginAdjustedLastPosition.Y - origin.Y);
            Vector2 playerBottomRightLastPos = new Vector2(player.OriginAdjustedLastPosition.X + origin.X + player.Width, player.OriginAdjustedLastPosition.Y + origin.Y + player.Height);

            // check if the paddle has moved through the ball between frames and if so move the ball by the same amount the paddle moved. This is to prevent the ball from getting stuck inside the paddle
            if ((position.X > playerTopLeftPos.X || lastPosition.X > playerTopLeftPos.X) && (position.X < playerBottomRightPos.X || lastPosition.X < playerBottomRightPos.X))
            {
                Vector2? paddleIntersectionTop = XAxisIntersection(playerTopLeftPos + new Vector2(origin.X, 0f), playerTopLeftPos - playerTopLeftLastPos, position.Y, null, null);
                Vector2? paddleIntersectionBottom = XAxisIntersection(playerBottomRightPos - new Vector2(origin.X, 0f), playerBottomRightPos - playerBottomRightLastPos, position.Y, null, null);
                if (paddleIntersectionBottom.HasValue || paddleIntersectionTop.HasValue)
                {
                    lastPosition += (playerBottomRightPos - playerBottomRightLastPos);
                    position += (playerBottomRightPos - playerBottomRightLastPos);
                }
            }

            //cheeck all possible intersections
            Vector2 directionVector = (position - lastPosition);

            Vector2? possiblePos1 = YAxisIntersection(lastPosition, directionVector, playerTopLeftPos.X, playerTopLeftPos.Y, playerBottomRightPos.Y);
            Vector2? possiblePos2 = YAxisIntersection(lastPosition, directionVector, playerBottomRightPos.X, playerTopLeftPos.Y, playerBottomRightPos.Y);
            Vector2? possiblePos3 = XAxisIntersection(lastPosition, directionVector, playerTopLeftPos.Y, playerTopLeftPos.X, playerBottomRightPos.X);
            Vector2? possiblePos4 = XAxisIntersection(lastPosition, directionVector, playerBottomRightPos.Y, playerTopLeftPos.X, playerBottomRightPos.X);

            //create local Variables for setting the right collision position and telling if a collision has occurred  
            bool colliding = false;

            Vector2 collisionPosition = Vector2.Zero;

            float distance1 = 1000f; 
            float distance2 = 1000f; 
            float distance3 = 1000f;

            if (possiblePos1.HasValue)
            {
                colliding = true;
                collisionPosition = possiblePos1.Value;
                distance1 = (possiblePos1.Value - lastPosition).Length();
            }
            if(possiblePos2.HasValue)
            {
                colliding = true;
                distance2 = (possiblePos2.Value - lastPosition).Length();
                if (distance2 < distance1)
                    collisionPosition = possiblePos2.Value;
            }
            if (possiblePos3.HasValue)
            {
                colliding = true;
                distance3 = (possiblePos3.Value - lastPosition).Length();
                if (distance3 < distance2 && distance3 < distance2)
                    collisionPosition = possiblePos3.Value;
            }
            if(possiblePos4.HasValue) 
            {
                colliding = true;
                float distance4 = (possiblePos4.Value - lastPosition).Length();
                if (distance4 < distance3 && distance4 < distance2 && distance4 < distance1)
                    collisionPosition = possiblePos4.Value;                
            }

            //check for collision with paddle
            if (colliding)
            {
                float extraY = 0f;
                //check whether ball hits from top OR bottom
                if (collisionPosition == possiblePos3 || collisionPosition == possiblePos4)
                {
                    velocity.Y = -velocity.Y;
                    hitSound.Play();
                }
                else
                {
                    //bounce
                    velocity.X = -velocity.X;

                    //check for collision near edge of player
                    float distanceToMiddle = (player.OriginAdjustedPosition.Y + player.Height / 2 - collisionPosition.Y);
                    if (Math.Abs(distanceToMiddle) > player.Height / 2 - player.Height / 4)
                    {
                        //calculate extra y velocity that still needs adjustment for currentspeed      
                        extraY = -Math.Sign(distanceToMiddle) * paddleAngleScaler;

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

                velocity.Y += extraY * currentSpeed;
                velocity.Normalize();
                position += velocity * lostDistance;
                velocity *= currentSpeed;
            }
            return colliding;
        }
       public override void Reset()
        {
            base.Reset();
            flicker = true;
            renderBall = false;
            RandomDirection();
        }
        void RandomDirection()
        {
            //calculate random direction that excludes directions that are close to exclusively vertical
            int x;
            int y = Pong.Random.Next(-10, 10);

            if (Pong.Random.Next(2) == 0)
                x = Pong.Random.Next(-10, -5);
            else
                x = Pong.Random.Next(5, 10);

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
        Vector2? YAxisIntersection(Vector2 supportVector, Vector2 directionVector, float xCoordinate, float? minY, float? maxY)
        {
            //Check if directionVector.X is not zero(it would break the code and it would never collide anyways with the Y axis)
            if (directionVector.X == 0f)
                return null;

            //Vector representation of line x = sV.X + lambda * dV.X 
            //So: lambda = (x = sV.X) / sV.X
            float lambda = (xCoordinate - supportVector.X) / directionVector.X;

            //make sure to only check in the direction of the Vector(not in the opposite direction) as well as make sure the intersection doesn't lie beyond the current position
            if (lambda < 0 || lambda > 1)
                return null;

            // y = sV.Y + lambda * dV.Y
            float yCoordinate = (supportVector.Y + lambda * directionVector.Y);

            //check if ycoordinate is within the specified range and if a range is given
            if (minY.HasValue && maxY.HasValue)
            {
                if (minY <= yCoordinate && yCoordinate <= maxY)
                    return new Vector2(xCoordinate, yCoordinate);
                else
                    return null;
            }
            else
                return new Vector2(xCoordinate, yCoordinate);
        }
        Vector2? XAxisIntersection(Vector2 supportVector, Vector2 directionVector, float yCoordinate, float? minX, float? maxX)
        {
            //Check if directionVector.Y is not zero(it would break the code and it would never collide anyways with the X axis)
            if (directionVector.Y == 0f)
                return null;

            //Vector representation of line y = sV.Y + lambda * dV.Y
            //So: lambda = (Y - sV.Y) / sV.Y
            float lambda = (yCoordinate - supportVector.Y) / directionVector.Y;

            //make sure to only check in the direction of the Vector(not in the opposite direction) as well as make sure the intersection doesn't lie beyond the current position
            if (lambda < 0 || lambda > 1)
                return null;

            //x = sV.X + lambda * dV.X 
            float xCoordinate = (supportVector.X + lambda * directionVector.X);

            //check if ycoordinate is within the specified range and if a range is given
            if (minX.HasValue && maxX.HasValue)
            {
                if (minX <= xCoordinate && xCoordinate <= maxX)
                    return new Vector2(xCoordinate, yCoordinate);
                else
                    return null;
            }
            else
                return new Vector2(xCoordinate, yCoordinate);
        }
    }
}

