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

        //varoiable used to wait before checking collisions again 
        float lastBounce;

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
            speedMultiplier = 1.05f;
            startSpeed = 420f;
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
                //temporary fix for phasing through paddles
                for (int i = 0; i < 10; i++)
                {
                    velocity /= 10f;
                    base.Update(gameTime);
                    velocity *= 10f;
                    CheckCollision(gameTime);
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if(renderBall)
                base.Draw(spriteBatch);
        }
        void CheckCollision(GameTime _gameTime)
        {
            CheckVerticalBorders();
            CheckHorizontalBorders();
            //check if bounce hasn't very recently occured to avoid bouncing back and forth when the ball ends up within the paddle due to the movement being in steps
            if (_gameTime.TotalGameTime.TotalMilliseconds > lastBounce)
            {
                //Check for collisions with paddles
                if (CheckPaddle(player1) || CheckPaddle(player2))
                    lastBounce = (float)_gameTime.TotalGameTime.TotalMilliseconds + 100f;
            } 
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
            if (position.Y < 0 + origin.Y || position.Y > Pong.screenSize.Y - origin.Y)
            {
                velocity.Y = -velocity.Y;
                borderHitSound.Play();
            }
        }

        bool CheckPaddle(Player player)
        {
            //check for collision with paddle
            if (position.X + origin.X >= player.CollisionPosition.X && position.X - origin.X <= player.CollisionPosition.X + player.Width && position.Y + origin.Y >= player.CollisionPosition.Y && position.Y - origin.Y <= player.CollisionPosition.Y + player.Height)
            {
                //check whether ball hits from top OR bottom
                if (position.X > player.CollisionPosition.X && position.X < player.CollisionPosition.X + player.Width)
                {
                    velocity.Y = -velocity.Y;
                    velocity *= speedMultiplier;
                    hitSound.Play();
                }
                else
                {
                    //check if not colliding with backside of the player
                    if (velocity.X < 0 && position.X < player.CollisionPosition.X + player.Width / 2)
                        return false;
                    else if (velocity.X > 0 && position.X > player.CollisionPosition.X + player.Width / 2)
                        return false;

                    //bounce
                    velocity.X = -velocity.X;

                    //check for collision near edge of player
                    float distanceToMiddle = (player.CollisionPosition.Y + player.Height / 2 - position.Y);

                    if (Math.Abs(distanceToMiddle) > player.Height / 2 - player.Height / 4)
                    {
                        //mainpulate velocity to end up with altered angle
                        float currentSpeed = velocity.Length();
                        velocity.Y -= Math.Sign(distanceToMiddle) * paddleAngleScaler * currentSpeed;
                        velocity.Normalize();
                        velocity *= currentSpeed;

                        edgeHitSound.Play();
                    }
                    else
                        hitSound.Play();

                    velocity *= speedMultiplier;
                }
                return true;
            }
            return false;
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

