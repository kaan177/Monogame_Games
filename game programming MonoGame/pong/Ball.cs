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
        public void Update()
        {
            position += speed;
            CheckCollision();
        }
        void CheckCollision()
        {
            if (position.Y < 0 + origin.Y || position.Y > Pong.screenSize.Y - origin.Y)
            {
                speed.Y = -speed.Y;
                speed *= speedMultiplier;
            }
            /*
            //temporary value 15 for where the paddlle is
            //TO DO: get texture width from paddle instead
            if (position.X < 0 + 15 + origin.X || position.X > Pong.screenSize.X - 15 - origin.Y)
            {
                //TO DO: check if colliding with paddle, when true use following code
                if (true)
                {
                    speed.X = -speed.X;
                    speed *= speedMultiplier;
                }
            }
            */
            if(position.X + origin.X >= paddle1.Position.X && position.X - origin.X <= paddle1.Position.X + paddle1.Width && position.Y + origin.Y >= paddle1.Position.Y && position.Y - origin.Y <= paddle1.Position.Y + paddle1.Height)
            {
                /*
                if (position.X + origin.X >= paddle1.Position.X && position.X <= paddle1.Position.X + paddle1.Width)
                {
                    speed.Y = -speed.Y;
                    speed *= speedMultiplier;
                }
                else
                {
                    speed.X = -speed.X;
                    speed *= speedMultiplier;
                }
                */
                speed.X = -speed.X;
            }
            if (position.X + origin.X >= paddle2.Position.X && position.X - origin.X <= paddle2.Position.X + paddle2.Width && position.Y + origin.Y >= paddle2.Position.Y && position.Y - origin.Y <= paddle2.Position.Y + paddle2.Height)
            {
                /*
                if (position.X + origin.X >= paddle2.Position.X && position.X + origin.X <= paddle2.Position.X + paddle2.Width || position.X - origin.X >= paddle2.Position.X && position.X - origin.X <= paddle2.Position.X + paddle2.Width)
                {
                    speed.Y = -speed.Y;
                    speed *= speedMultiplier;
                }
                else
                {
                    speed.X = -speed.X;
                    speed *= speedMultiplier;
                }
                */
                speed.X = -speed.X;
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
            speed = new Vector2(-2, 2);
            speedMultiplier = 1.02f;
            paddle1 = _paddle1;
            paddle2 = _paddle2;
        }


    }
}
