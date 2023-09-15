using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pong
{
    class Ball
    {
        Texture2D ball;
        Vector2 position, startPosition, velocity, origin;
        float speedMultiplier, startSpeed;
        Paddle paddle1, paddle2;
        double lastBounce;
        System.Random random;

        public void Update(GameTime _gameTime)
        {
            position += velocity;
            CheckCollision(_gameTime);
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(ball, position - origin, Microsoft.Xna.Framework.Color.White);
        }
        void CheckCollision(GameTime _gameTime)
        {
            //check for collison with top and bottom of the screen
            if (position.Y < 0 + origin.Y || position.Y > Pong.screenSize.Y - origin.Y)
            {
                velocity.Y = -velocity.Y;
                velocity *= speedMultiplier;
            }
            //check if bounce hasn't occured in the last 200 miliseconds
            if (_gameTime.TotalGameTime.TotalMilliseconds > lastBounce)
            {
                //check for collision with paddle 1
                if (position.X + origin.X >= paddle1.Position.X && position.X - origin.X <= paddle1.Position.X + paddle1.Width && position.Y + origin.Y >= paddle1.Position.Y && position.Y - origin.Y <= paddle1.Position.Y + paddle1.Height)
                {
                    //check whether ball hits from top OR bottom
                    if (position.Y + origin.Y >= paddle1.Position.Y + paddle1.Height || position.Y - origin.Y <= paddle1.Position.Y && position.X > paddle1.Position.X && position.X < paddle1.Position.X + paddle1.Width)
                    {
                        velocity.Y = -velocity.Y;
                        velocity *= speedMultiplier;
                    }
                    else
                    {
                        velocity.X = -velocity.X;
                        velocity *= speedMultiplier;
                    }
                    lastBounce = _gameTime.TotalGameTime.TotalMilliseconds + 200;
                }
                //check collision with paddle 2
                if (position.X + origin.X >= paddle2.Position.X && position.X - origin.X <= paddle2.Position.X + paddle2.Width && position.Y + origin.Y >= paddle2.Position.Y && position.Y - origin.Y <= paddle2.Position.Y + paddle2.Height)
                {
                    //check whether ball hits from top OR bottom
                    if (position.Y + origin.Y >= paddle2.Position.Y + paddle2.Height || position.Y - origin.Y <= paddle2.Position.Y && position.X > paddle2.Position.X && position.X < paddle2.Position.X + paddle2.Width)
                    {
                        velocity.Y = -velocity.Y;
                        velocity *= speedMultiplier;
                    }
                    else
                    {
                        velocity.X = -velocity.X;
                        velocity *= speedMultiplier;
                    }
                    lastBounce = _gameTime.TotalGameTime.TotalMilliseconds + 200;
                }
            } 
        }
        void Reset()
        {
            position = startPosition;
            velocity = new Vector2(random.Next(-10, 10), random.Next(-10, 10));
            velocity.Normalize();
            velocity *= startSpeed;
            
        }

        public Ball(Vector2 _startPosition, ContentManager _Content, Paddle _paddle1, Paddle _paddle2)
        {
            ball = _Content.Load<Texture2D>("ball");
            origin = new Vector2(ball.Width, ball.Height) / 2;
            startPosition = _startPosition;
            speedMultiplier = 1.05f;
            startSpeed = 3f;
            paddle1 = _paddle1;
            paddle2 = _paddle2;
            random = new System.Random();
            Reset();
        }


    }
}

