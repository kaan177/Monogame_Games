using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace pong
{
    class MovingGameObject : GameObject
    {
        //class that can't be instantiated, it is a framework to build upon for game objects that need to be able to move. It inherits from GameObject and is the parent class of Ball and Player

        protected Vector2 lastPosition, velocity;
        protected MovingGameObject(ContentManager _content, string _textureString, Vector2 _startPosition, Pong _pong) : base(_content, _textureString, _startPosition, _pong)
        {
        }

        //updates the position by adding the velocity per second and saves the position of the last frame
        public override void Update(GameTime gameTime)
        {
            lastPosition = position;
            position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        //resets the position(base) last position and sets the velocity to zero
        public override void Reset()
        {
            base.Reset();
            lastPosition = position;
            velocity = Vector2.Zero;
        }
    }
}
