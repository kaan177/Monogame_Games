using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pong
{
    class GameObject
    {
        protected Texture2D texture;
        protected Vector2 position, startPosition, velocity, origin;
        protected GameObject(ContentManager _content, string _textureString, Vector2 _startPosition)
        {
            velocity = Vector2.Zero;
            startPosition = _startPosition;
            texture = _content.Load<Texture2D>(_textureString);
            origin = new Vector2(texture.Width, texture.Height) / 2;
            Reset();
        }

        public virtual void Update(GameTime gameTime)
        {
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position - origin, Color.White);
        }
        public virtual void Reset()
        {
            position = startPosition;
        }
    }
}
