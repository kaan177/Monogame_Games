using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Data;

namespace pong
{
    class Ball
    {
        Texture2D ball;
        SoundEffect hitSound, edgeHitSound, borderHitSound;

        Vector2 position, startPosition, velocity, origin;
        float speedMultiplier, startSpeed, paddleAngleScaler;
        float lastBounce, lastFlicker, flickerTime;

        int collisionPrecision = 1;
        int totalFlickers;

        bool flicker, renderBall;

        Player paddle1, paddle2;
        System.Random random;

        public void Update(GameTime _gameTime)
        {
            if (flicker == true)
            {
                if(_gameTime.TotalGameTime.TotalSeconds > lastFlicker)
                {
                    lastFlicker = (float)_gameTime.TotalGameTime.TotalSeconds + flickerTime;
                    renderBall = !renderBall;
                    totalFlickers++;
                    if(totalFlickers > 6)
                    {
                        flicker = false;
                        totalFlickers = 0;
                    }
                }
            }
            else
            {
                for (int i = 0; i < collisionPrecision; i++)
                {
                    position += velocity / collisionPrecision;
                    CheckCollision(_gameTime);
                }
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            if(renderBall)
                _spriteBatch.Draw(ball, position - origin, Microsoft.Xna.Framework.Color.White);
        }
        void CheckCollision(GameTime _gameTime)
        {
            CheckVerticalBorders();
            CheckHorizontalBorders();
            //check if bounce hasn't very recently occured to avoid bouncing back and forth when the ball ends up within the paddle due to the movement being in steps
            if (_gameTime.TotalGameTime.TotalMilliseconds > lastBounce)
            {
                //Check for collisions with paddles
                if(CheckPaddle(paddle1) || CheckPaddle (paddle2))
                    lastBounce = (float)_gameTime.TotalGameTime.TotalMilliseconds + 100f;
            } 
        }

        void CheckVerticalBorders()
        {
            if (position.X + origin.X <= 0)
            {
                Reset();
                paddle1.Reset();
                paddle2.Reset();

                paddle1.TakeDamage(1);
            }
            if (position.X - origin.X >= Pong.screenSize.X)
            {
                Reset();
                paddle1.Reset();
                paddle2.Reset();

                paddle2.TakeDamage(1);
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

        bool CheckPaddle(Player paddle)
        {
            //check for collision with paddle
            if (position.X + origin.X >= paddle.Position.X && position.X - origin.X <= paddle.Position.X + paddle.Width && position.Y + origin.Y >= paddle.Position.Y && position.Y - origin.Y <= paddle.Position.Y + paddle.Height)
            {
                //check whether ball hits from top OR bottom
                if (position.X > paddle.Position.X && position.X < paddle.Position.X + paddle.Width)
                {
                    velocity.Y = -velocity.Y;
                    velocity *= speedMultiplier;
                    hitSound.Play();
                }
                else
                {
                    //check if not colliding with backside of the paddle
                    if (velocity.X < 0 && position.X < paddle.Position.X + paddle.Width / 2)
                        return false;
                    else if (velocity.X > 0 && position.X > paddle.Position.X + paddle.Width / 2)
                        return false;

                    //bounce
                    velocity.X = -velocity.X;

                    //check for collision near edge of paddle
                    float distanceToMiddle = (paddle.Position.Y + paddle.Height / 2 - position.Y);

                    if (Math.Abs(distanceToMiddle) > paddle.Height / 2 - paddle.Height / 4)
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

        public void Reset()
        {
            position = startPosition;
            flicker = true;
            renderBall = false;

            int x;
            int y = random.Next(-10, 10);

            if (random.Next(2) == 0)
                x = random.Next(-10, -5);
            else
                x = random.Next(5, 10);

            velocity = new Vector2(x, y);
            velocity.Normalize();
            velocity *= startSpeed;
        }

        public Ball(Vector2 _startPosition, ContentManager _Content, Player _paddle1, Player _paddle2)
        {
            ball = _Content.Load<Texture2D>("ball");
            hitSound = _Content.Load<SoundEffect>("paddleHitSound");
            edgeHitSound = _Content.Load<SoundEffect>("edgeHitSound");
            borderHitSound = _Content.Load<SoundEffect>("borderHitSound");
            origin = new Vector2(ball.Width, ball.Height) / 2;
            startPosition = _startPosition;
            speedMultiplier = 1.05f;
            startSpeed = 7f;
            paddleAngleScaler = 0.5f;
            paddle1 = _paddle1;
            paddle2 = _paddle2;
            random = new System.Random();
            collisionPrecision = 10;
            flickerTime = 0.33f;
            Reset();
        }


    }
}

