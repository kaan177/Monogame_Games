using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace pong
{
    class Ball
    {
        Texture2D ball;
        Vector2 position, startPosition, velocity, origin;
        float speedMultiplier, startSpeed, paddleAngleScaler;
        Paddle paddle1, paddle2;
        double lastBounce;
        int collisionPrecision = 1;
        SoundEffect hitSound, edgeHitSound, borderHitSound;
        System.Random random;

        public void Update(GameTime _gameTime)
        {
            for (int i = 0; i < collisionPrecision; i++)
            {
                position += velocity / collisionPrecision;
                CheckCollision(_gameTime);
            }
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(ball, position - origin, Microsoft.Xna.Framework.Color.White);
        }
        void CheckCollision(GameTime _gameTime)
        {
            CheckVerticalBorders();
            CheckHorizontalBorders();
            //check if bounce hasn't very recently occured to avoid bouncing back and forth when the ball ends up within the paddle due to the movement being in steps
            if (_gameTime.TotalGameTime.TotalMilliseconds > lastBounce)
            {
                //Cjheck for collisions with paddles
                if(CheckPaddle(paddle1) || CheckPaddle (paddle2))
                    lastBounce = _gameTime.TotalGameTime.TotalMilliseconds + 100;
            } 
        }
        void CheckVerticalBorders()
        {
            if (position.X + origin.X <= 0 || position.X - origin.X >= Pong.screenSize.X)
                Reset();
        }
        void CheckHorizontalBorders()
        {
            if (position.Y < 0 + origin.Y || position.Y > Pong.screenSize.Y - origin.Y)
            {
                velocity.Y = -velocity.Y;
                borderHitSound.Play();
            }
        }
        bool CheckPaddle(Paddle paddle)
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
                    //TO DO: check if not colliding with backside of the paddle
                    velocity.X = -velocity.X;

                    float distanceToMiddle = (paddle.Position.Y + paddle.Height / 2 - position.Y);
                    //check for collision near edge of paddle
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
                    {
                        hitSound.Play();
                    }

                    velocity *= speedMultiplier;
                }
                return true;
            }
            return false;
        }
        void Reset()
        {
            position = startPosition;

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

        public Ball(Vector2 _startPosition, ContentManager _Content, Paddle _paddle1, Paddle _paddle2)
        {
            ball = _Content.Load<Texture2D>("ball");
            hitSound = _Content.Load<SoundEffect>("paddleHitSound");
            edgeHitSound = _Content.Load<SoundEffect>("edgeHitSound");
            borderHitSound = _Content.Load<SoundEffect>("borderHitSound");
            origin = new Vector2(ball.Width, ball.Height) / 2;
            startPosition = _startPosition;
            speedMultiplier = 1.05f;
            startSpeed = 5f;
            paddleAngleScaler = 0.5f;
            paddle1 = _paddle1;
            paddle2 = _paddle2;
            random = new System.Random();
            collisionPrecision = 100;
            Reset();
        }


    }
}

