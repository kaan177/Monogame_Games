using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pong
{
    internal class PowerUps : GameObject
    {
        public enum PowerUp {swerve , shrink, none}
        PowerUp powerUp;
        Texture2D overlayTexture, wobbleTexture, shrinkTexture;
        float nextPowerUpTime, nextResetTime;
        float resetDuration;

        const float interval = 10f;
        const float swerveDuration = 5f, shrinkDuration = 7f;
        bool isActive, isVisible;
        bool nextPowerUpTimerNeedsReset;
        bool applyPowerUpTimerNeedsReset;
        public PowerUps(ContentManager _content, string _textureString, Vector2 _startPosition, Pong _pong) : base(_content, _textureString, _startPosition, _pong)
        {
            wobbleTexture = _content.Load<Texture2D>("swerve");
            shrinkTexture = _content.Load<Texture2D>("shrink");
            isActive = false;
            nextPowerUpTimerNeedsReset = true;
            applyPowerUpTimerNeedsReset = false;
        }

        public override void Update(GameTime gameTime)
        {
            float totalSeconds = (float)gameTime.TotalGameTime.TotalSeconds;
            if(applyPowerUpTimerNeedsReset)
            {
                nextResetTime = totalSeconds + resetDuration;
                applyPowerUpTimerNeedsReset = false;
            }
            if(totalSeconds > nextResetTime && isActive)
            {
                isActive = false;
                if (powerUp == PowerUp.shrink)
                    foreach (Player player in pong.Players)
                        player.UnShrink();
                nextPowerUpTimerNeedsReset = true;
            }
            if (nextPowerUpTimerNeedsReset)
            {
                nextPowerUpTime = totalSeconds + interval;
                nextPowerUpTimerNeedsReset = false;
            }
            if (totalSeconds > nextPowerUpTime && !isVisible && !isActive)
            {
                switch (Pong.Random.Next(0, 2))
                {
                    case 0:
                        powerUp = PowerUp.swerve;
                        overlayTexture = wobbleTexture;
                    break;
                    case 1:
                        powerUp = PowerUp.shrink;
                        overlayTexture = shrinkTexture;
                    break;
                }
                isVisible = true;
                SetRandomPosition();
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isVisible)
            {
                base.Draw(spriteBatch);
                spriteBatch.Draw(overlayTexture, position - origin, Color.White);
            }
        }
        public override void Reset()
        {
            nextPowerUpTimerNeedsReset = true;
            isActive = false;
            isVisible = false;
            if (powerUp == PowerUp.shrink)
                foreach (Player player in pong.Players)
                    player.UnShrink();
        }
        void SetRandomPosition()
        {
            //sets the position to a random position within the screen, that is not too close to any edges
            float xMultiplier = Pong.Random.NextSingle() * 0.8f + 0.1f;
            float yMultiplier = Pong.Random.NextSingle() * 0.8f + 0.1f;
            position = new Vector2(Pong.screenSize.X * xMultiplier, Pong.screenSize.Y * yMultiplier);
        }
        void ApplySwerve()
        {
            applyPowerUpTimerNeedsReset = true;
            resetDuration = swerveDuration;
            isActive = true;
        }
        void ApplyShrink(int lastPaddle)
        {
            applyPowerUpTimerNeedsReset = true;
            resetDuration = shrinkDuration;
            isActive = true;
            foreach(Player player in pong.Players)
            {
                if (player.PlayerId != lastPaddle)
                    player.Shrink();
            }
        }
        public void GetHit(int lastPaddle)
        {
            isVisible = false;
            switch(powerUp)
            {
                case PowerUp.swerve:
                    ApplySwerve();
                    break;
                case PowerUp.shrink:
                    ApplyShrink(lastPaddle);
                    break;
            }
        }
        public Vector2 Position
        {
            get { return position; }
        }
        public Vector2 Origin
        {
            get { return origin; }
        }
        public bool IsVisible
        {
            get { return isVisible; }
        }
        public PowerUp ActivePowerUp
        {
            get 
            {
                if (isActive) 
                    return powerUp; 
                else 
                    return PowerUp.none;
            }
        }
    }
}
