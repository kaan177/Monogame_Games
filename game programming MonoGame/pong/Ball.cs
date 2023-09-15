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

        public void Update()
        {
            position += speed;
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(ball, position - origin, Color.White);
        }
        public Ball(Vector2 _startPosition, ContentManager _Content)
        {
            ball = _Content.Load<Texture2D>("ball");
            origin = new Vector2(ball.Width, ball.Height) / 2;
            position = _startPosition;
            speed = new Vector2(1, 1);
        }

    }
}
