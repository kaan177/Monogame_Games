﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pong
{
    class GameObject
    {
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
