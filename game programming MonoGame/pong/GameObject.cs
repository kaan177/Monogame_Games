using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pong
{
    class GameObject
    {
        //class provides the framework for all game objects, all game objects in this game have: a reference to the main class Pong that does most of the directing, a texture, a position, start position and origin.
        //All objects can be Drawn and Reset. It is assumed that objects have an update method and override it. This is for future proofing, as you might want to keep a list of game Objects and update them all.

        protected Pong pong;
        protected Texture2D texture;
        protected Vector2 position, startPosition, origin;
        protected GameObject(ContentManager _content, string _textureString, Vector2 _startPosition, Pong _pong)
        {
            startPosition = _startPosition;
            texture = _content.Load<Texture2D>(_textureString);
            origin = new Vector2(texture.Width, texture.Height) / 2;
            pong = _pong;
            Reset();
        }
        public virtual void Update(GameTime gameTime)
        {
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
