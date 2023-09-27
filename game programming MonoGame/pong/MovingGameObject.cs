using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pong
{
    class MovingGameObject : GameObject
    {
        protected Vector2 lastPosition, velocity;
        protected MovingGameObject(ContentManager _content, string _textureString, Vector2 _startPosition, Pong _pong) : base(_content, _textureString, _startPosition, _pong)
        {
        }
        public override void Update(GameTime gameTime)
        {
            lastPosition = position;
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        public override void Reset()
        {
            base.Reset();
            lastPosition = position;
            velocity = Vector2.Zero;
        }
    }
}
