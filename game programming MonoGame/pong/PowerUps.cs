﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;

namespace pong
{
    internal class PowerUps : GameObject
    {
        public enum PowerUp {swerve , shrink, shield, heart, reversed, none}
        public enum Shield { leftShield, rightShield, topShield, bottomShield, none}
        PowerUp powerUp;
        Shield activeShield;
        SoundEffect spawnSound, pickUpSound, wobbleSound, endSound;
        Texture2D overlayTexture, wobbleTexture, shrinkTexture, shieldOverlayTexture, hearthTexture, reversedTexture;
        Texture2D blueCard, redCard, greenCard, yellowCard;
        Texture2D shieldVerticalTexture, shieldHorizontalTexture;
        List<SoundEffectInstance> soundEffects = new List<SoundEffectInstance>();
        float nextPowerUpTime, nextResetTime;
        float resetDuration;

        const float interval = 10f;
        const float swerveDuration = 4.5f, shrinkDuration = 7f, reversedDuration = 7f;
        bool isActive, isVisible;
        bool nextPowerUpTimerNeedsReset;
        bool applyPowerUpTimerNeedsReset;
        public PowerUps(ContentManager _content, string _textureString, Vector2 _startPosition, Pong _pong) : base(_content, _textureString, _startPosition, _pong)
        {
            spawnSound = _content.Load<SoundEffect>("powerUpSpawn");
            pickUpSound = _content.Load<SoundEffect>("powerUpPickup");
            wobbleSound = _content.Load<SoundEffect>("wobbleSound");
            endSound = _content.Load<SoundEffect>("powerUpEnd");
            wobbleTexture = _content.Load<Texture2D>("swerve");
            shrinkTexture = _content.Load<Texture2D>("shrink");
            shieldVerticalTexture = _content.Load<Texture2D>("verticalShield");
            shieldHorizontalTexture = _content.Load<Texture2D>("horizontalShield");
            shieldOverlayTexture = _content.Load<Texture2D>("shieldOverlay");
            hearthTexture = _content.Load<Texture2D>("heartPowerupOverlay");
            blueCard = _content.Load<Texture2D>("ReverseBlue");
            redCard = _content.Load<Texture2D>("ReverseRed");
            greenCard = _content.Load<Texture2D>("ReverseGreen");
            yellowCard = _content.Load<Texture2D>("ReverseYellow");
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
                if (powerUp == PowerUp.reversed)
                    foreach (Player player in pong.Players)
                        player.ResetControls();
                nextPowerUpTimerNeedsReset = true;
                soundEffects.Insert(0, endSound.CreateInstance());
                soundEffects[0].Play();
            }
            if (nextPowerUpTimerNeedsReset)
            {
                nextPowerUpTime = totalSeconds + interval;
                nextPowerUpTimerNeedsReset = false;
            }
            if (totalSeconds > nextPowerUpTime && !isVisible && !isActive)
            {
                switch (Pong.Random.Next(0, 5))
                {
                    case 0:
                        powerUp = PowerUp.swerve;
                        overlayTexture = wobbleTexture;
                        break;
                    case 1:
                        powerUp = PowerUp.shrink;
                        overlayTexture = shrinkTexture;
                        break;
                    case 2:
                        powerUp = PowerUp.heart;
                        overlayTexture = hearthTexture;
                        break;
                    case 3:
                        powerUp = PowerUp.shield;
                        overlayTexture = shieldOverlayTexture;
                        break;
                    case 4:
                        powerUp = PowerUp.reversed;
                        RandomUnoReverseCard();
                        overlayTexture = reversedTexture;
                        break;
                }
                isVisible = true;
                soundEffects.Insert(0, spawnSound.CreateInstance());
                soundEffects[0].Play();
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
            switch (activeShield)
            {
                case Shield.leftShield:
                    spriteBatch.Draw(shieldVerticalTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, new Vector2(1, Pong.screenSize.Y), SpriteEffects.None, 0f);
                    break;
                case Shield.rightShield:
                    spriteBatch.Draw(shieldVerticalTexture, new Vector2(Pong.screenSize.X - shieldVerticalTexture.Width, 0f), null, Color.White, 0f, Vector2.Zero, new Vector2(1, Pong.screenSize.Y), SpriteEffects.None, 0f);
                    break;
                case Shield.topShield:
                    spriteBatch.Draw(shieldHorizontalTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, new Vector2(Pong.screenSize.X, 1), SpriteEffects.None, 0f);
                    break;
                case Shield.bottomShield:
                    spriteBatch.Draw(shieldHorizontalTexture, new Vector2(0f , Pong.screenSize.Y - shieldHorizontalTexture.Height), null, Color.White, 0f, Vector2.Zero, new Vector2(Pong.screenSize.X, 1), SpriteEffects.None, 0f);
                    break;
            }
        }
        public override void Reset()
        {
            foreach(SoundEffectInstance soundEffect in soundEffects)
                soundEffect.Stop();
            soundEffects.Clear();
            nextPowerUpTimerNeedsReset = true;
            isActive = false;
            isVisible = false;
            activeShield = Shield.none;
            if (powerUp == PowerUp.shrink)
                foreach (Player player in pong.Players)
                    player.UnShrink();
            if (powerUp == PowerUp.reversed)
                foreach (Player player in pong.Players)
                    player.ResetControls();
            powerUp = PowerUp.none;
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
            soundEffects.Insert(0, wobbleSound.CreateInstance());
            soundEffects[0].Play();
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
        void ApplyShield(int lastPaddle)
        {
            nextPowerUpTimerNeedsReset = true;
            switch (lastPaddle) 
            {
                case 0:
                    activeShield = Shield.leftShield;
                    break;
                case 1:
                    activeShield = Shield.rightShield;
                    break;
                case 2:
                    activeShield = Shield.topShield;
                    break;
                case 3:
                    activeShield = Shield.bottomShield;
                    break;
            }
        }
        public void BreakShield()
        {
            activeShield = Shield.none;
        }
        void ApplyHealth(int lastPaddle)
        {

            nextPowerUpTimerNeedsReset = true;
            foreach(Player player in pong.Players)
            {
                if (player.PlayerId == lastPaddle)
                    player.HealthUp();
            }

        }
        void ApplyReversed(int lastPaddle)
        {
            applyPowerUpTimerNeedsReset = true;
            resetDuration = reversedDuration;
            isActive = true;
            foreach (Player player in pong.Players)
            {
                if (player.PlayerId != lastPaddle)
                    player.ReverseControls();
            }
        }
        void RandomUnoReverseCard()
        {
            switch (Pong.Random.Next(0, 4))
            {
                case 0:
                    reversedTexture = blueCard;
                    break;
                case 1:
                    reversedTexture = redCard;
                    break;
                case 2:
                    reversedTexture = greenCard;
                    break;
                case 3:
                    reversedTexture = yellowCard;
                    break;

            }         
        }
        public void GetHit(int lastPaddle)
        {
            isVisible = false;
            soundEffects.Insert(0, pickUpSound.CreateInstance());
            soundEffects[0].Play();
            switch (powerUp)
            {
                case PowerUp.swerve:
                    ApplySwerve();
                    break;
                case PowerUp.shrink:
                    ApplyShrink(lastPaddle);
                    break;
                case PowerUp.shield:
                    ApplyShield(lastPaddle);
                    break;
                case PowerUp.heart:
                    ApplyHealth(lastPaddle);
                    break;
                case PowerUp.reversed:
                    ApplyReversed(lastPaddle);
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
        public Shield ActiveShield
        {
            get { return activeShield; }
        }
    }
}
