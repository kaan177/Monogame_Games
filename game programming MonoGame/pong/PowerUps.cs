using System;
using System.CodeDom;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pong
{
    internal class PowerUps : GameObject
    {
        enum powerUp {swerve , shrink}
        Texture2D wobblePowerUp;
        float lastPowerUpTime;
        float interval;
        bool isActive;
        bool timerNeedsReset;
        public PowerUps(ContentManager _content, string _textureString, Vector2 _startPosition, Pong _pong) : base(_content, _textureString, _startPosition, _pong)
        {
            wobblePowerUp = _content.Load<Texture2D>("swerve");
            isActive = false;
            interval = 10f;
            timerNeedsReset = true;
        }

        public override void Update(GameTime gameTime)
        {
            float totalSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
            if (timerNeedsReset)
            {
                lastPowerUpTime = totalSeconds + interval;
                timerNeedsReset = false;
            }
            if (totalSeconds > lastPowerUpTime && !isActive)
            {
                isActive = true;
                SetRandomPosition();
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isActive)
            {
                base.Draw(spriteBatch);
                spriteBatch.Draw(wobblePowerUp, position - origin, Color.White);
            }
        }
        public override void Reset()
        {
            timerNeedsReset = true;
        }
        void SetRandomPosition()
        {
            //sets the position to a random position within the screen, that is not too close to any edges
            float xMultiplier = Pong.Random.NextSingle() * 0.8f + 0.1f;
            float yMultiplier = Pong.Random.NextSingle() * 0.8f + 0.1f;
            position = new Vector2(Pong.screenSize.X * xMultiplier, Pong.screenSize.Y * yMultiplier);
        }
        public void GetHit(int lastPaddle)
        {
            isActive = false;
            timerNeedsReset = true;
        }
        public Vector2 Position
        {
            get { return position; }
        }
        public Vector2 Origin
        {
            get { return origin; }
        }
        public bool IsActive
        {
            get { return isActive; }
        }
    }
}
