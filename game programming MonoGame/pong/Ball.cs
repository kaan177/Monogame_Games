using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pong
{
    class Ball
    {
        Texture2D ball;
        Vector2 position, speed, origin;
        float speedMultiplier;
        Paddle paddle1, paddle2;
        double lastBounce;      
        public void Update(GameTime _gameTime)
        {
            position += speed;
            CheckCollision(_gameTime);
        }
        void CheckCollision(GameTime _gameTime)
        {
            if (position.Y < 0 + origin.Y || position.Y > Pong.screenSize.Y - origin.Y)
            {
                speed.Y = -speed.Y;
                speed *= speedMultiplier;
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
                        speed.Y = -speed.Y;
                        speed *= speedMultiplier;
                    }
                    else
                    {
                        speed.X = -speed.X;
                        speed *= speedMultiplier;
                    }
                    lastBounce = _gameTime.TotalGameTime.TotalMilliseconds + 200;
                }
                //check collision with paddle 2
                if (position.X + origin.X >= paddle2.Position.X && position.X - origin.X <= paddle2.Position.X + paddle2.Width && position.Y + origin.Y >= paddle2.Position.Y && position.Y - origin.Y <= paddle2.Position.Y + paddle2.Height)
                {
                    //check whether ball hits from top OR bottom
                    if (position.Y + origin.Y >= paddle2.Position.Y + paddle2.Height || position.Y - origin.Y <= paddle2.Position.Y && position.X > paddle2.Position.X && position.X < paddle2.Position.X + paddle2.Width)
                    {
                        speed.Y = -speed.Y;
                        speed *= speedMultiplier;
                    }
                    else
                    {
                        speed.X = -speed.X;
                        speed *= speedMultiplier;
                    }
                    lastBounce = _gameTime.TotalGameTime.TotalMilliseconds + 200;
                }
            } 
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(ball, position - origin, Color.White);
        }
        public Ball(Vector2 _startPosition, ContentManager _Content, Paddle _paddle1, Paddle _paddle2)
        {
            ball = _Content.Load<Texture2D>("ball");
            origin = new Vector2(ball.Width, ball.Height) / 2;
            position = _startPosition;
            speed = new Vector2(-3,3);
            speedMultiplier = 1.0f;
            paddle1 = _paddle1;
            paddle2 = _paddle2;
        }


    }
}

