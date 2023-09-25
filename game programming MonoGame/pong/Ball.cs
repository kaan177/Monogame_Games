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
            CheckPaddle(player1);
            CheckPaddle(player2);
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
            //create variables for checking whether the ball bounces with the side while calculating the correct collision position
            Vector2 directionVector = position - lastPosition;
            Vector2? intersectionTop = CollisionHelper.HorizontalIntersection(lastPosition, directionVector, origin.Y, null, null);
            Vector2? intersectionBottom = CollisionHelper.HorizontalIntersection(lastPosition, directionVector, Pong.screenSize.Y - origin.Y, null, null);

            //Check whether a collision should occur
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

        void CheckPaddle(Player player)
        {
            //create the idea of a bounding box that is slightly larger than the actual bounding box of the padle to accomodate for the ball colliding at the centre only
            Vector2 topLeftBound = new Vector2(player.OriginAdjustedPosition.X - origin.X, player.OriginAdjustedPosition.Y - origin.Y);
            Vector2 bottomRightBound = new Vector2(player.OriginAdjustedPosition.X + origin.X + player.Width, player.OriginAdjustedPosition.Y + origin.Y + player.Height);
            Vector2 topLeftLastBound = new Vector2(player.OriginAdjustedLastPosition.X - origin.X, player.OriginAdjustedLastPosition.Y - origin.Y);
            Vector2 bottomRightLastBound = new Vector2(player.OriginAdjustedLastPosition.X + origin.X + player.Width, player.OriginAdjustedLastPosition.Y + origin.Y + player.Height);

            //check if the paddle has moved through the ball between frames and if so move the ball by the same amount the paddle moved. This is to prevent the ball from getting stuck inside the paddle
            //note: the bouncing isn't calculated here, even though its likely that it should. The reasons are that the exact collision position remains unknown and that the paddle might move in the same vertical direction as the ball
            if ((position.X > topLeftBound.X || lastPosition.X > topLeftBound.X) && (position.X < bottomRightBound.X || lastPosition.X < bottomRightBound.X))
            {
                Vector2? paddleIntersectionTop = CollisionHelper.HorizontalIntersection(topLeftBound + new Vector2(origin.X, 0f), topLeftBound - topLeftLastBound, position.Y, null, null);
                Vector2? paddleIntersectionBottom = CollisionHelper.HorizontalIntersection(bottomRightBound - new Vector2(origin.X, 0f), bottomRightBound - bottomRightLastBound, position.Y, null, null);
                if (paddleIntersectionBottom.HasValue || paddleIntersectionTop.HasValue)
                {
                    lastPosition += (bottomRightBound - bottomRightLastBound);
                    position += (bottomRightBound - bottomRightLastBound);
                }
            }

            //check for collision with paddle
            Vector2? boxIntersection = CollisionHelper.BoxIntersection(lastPosition, position - lastPosition, topLeftBound, bottomRightBound);
            if (boxIntersection.HasValue)
            {
                Vector2 collisionPosition = boxIntersection.Value;
                float extraY = 0f;
                //check whether ball hits from top OR bottom
                if (topLeftBound.X < collisionPosition.X && collisionPosition.X < bottomRightBound.X)
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
       
    }
}

