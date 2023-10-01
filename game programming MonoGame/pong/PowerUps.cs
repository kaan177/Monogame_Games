using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using static pong.PowerUps;

namespace pong
{
    internal class PowerUps : GameObject
    {
        //The class power ups is responsible for spawning a random power up at a specified interval and applying these power ups when hit

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
            //applyPowerUpTimerNeedsReset is set to true when some of the powers activate to start the 'timer' for when the power up needs to stop
            if(applyPowerUpTimerNeedsReset)
            {
                //nextResetTime is the time at which the power up will end, it is the current reset duration added to the current time
                nextResetTime = totalSeconds + resetDuration;
                applyPowerUpTimerNeedsReset = false;
            }
            //stops the ongoing power up at (or right after) the nextResetTime
            if(totalSeconds > nextResetTime && isActive)
            {
                //deactivates the current power up
                isActive = false;

                //resets relevant variables in player based on the current power up
                if (powerUp == PowerUp.shrink)
                    foreach (Player player in pong.Players)
                        player.UnShrink();
                if (powerUp == PowerUp.reversed)
                    foreach (Player player in pong.Players)
                        player.ResetControls();
                //set the variable so that the timer for the next power up spawn starts
                nextPowerUpTimerNeedsReset = true;
                
                //plays a sound to tell the player the power up has ended and creates an instance so it can be stopped
                soundEffects.Insert(0, endSound.CreateInstance());
                soundEffects[0].Play();
            }
            if (nextPowerUpTimerNeedsReset)
            {
                //set the time at which the next power up will spawn to 'interval' seconds from the current time
                nextPowerUpTime = totalSeconds + interval;
                nextPowerUpTimerNeedsReset = false;
            }
            //'spawns' a random power up at a random position
            if (totalSeconds > nextPowerUpTime && !isVisible && !isActive)
            {
                //set the current (not yet active) power up to a random powerup
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
                //make the power up appear, play a sound, and set the location to a random position
                isVisible = true;
                soundEffects.Insert(0, spawnSound.CreateInstance());
                soundEffects[0].Play();
                SetRandomPosition();
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            //draw the power up object if visible
            if (isVisible)
            {
                base.Draw(spriteBatch);
                spriteBatch.Draw(overlayTexture, position - origin, Color.White);
            }
            //draw the active shield depending on the side
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

        //method called when the ball hits one of the players's sides or when the game resets
        public override void Reset()
        {
            //stop all sounds and clear the list of sound instances
            foreach(SoundEffectInstance soundEffect in soundEffects)
                soundEffect.Stop();
            soundEffects.Clear();

            //reset variables to their initial state
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

        //sets the position to a random position within the screen, that is not too close to any edges
        void SetRandomPosition()
        {
            float xMultiplier = Pong.Random.NextSingle() * 0.8f + 0.1f;
            float yMultiplier = Pong.Random.NextSingle() * 0.8f + 0.1f;
            position = new Vector2(Pong.screenSize.X * xMultiplier, Pong.screenSize.Y * yMultiplier);
        }

        //applies the swerve effect
        void ApplySwerve()
        {
            //plays the sound effect for wobbling / swerving
            soundEffects.Insert(0, wobbleSound.CreateInstance());
            soundEffects[0].Play();

            //make sure the effect gets reset
            applyPowerUpTimerNeedsReset = true;
            resetDuration = swerveDuration;

            //sets the power up to active, the actual swerving is then handled by the ball
            isActive = true;
        }

        //applies the shrink effect to all players except for the player who hit the power up object
        void ApplyShrink(int lastPaddle)
        {
            //makes sure the effect gets reset
            applyPowerUpTimerNeedsReset = true;
            resetDuration = shrinkDuration;
            isActive = true;

            //shrink all the players except for the player that hit the power up object
            foreach(Player player in pong.Players)
            {
                if (player.PlayerId != lastPaddle)
                    player.Shrink();
            }
        }

        //gives the player who hit the power up object shield
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

        //method that gets called by Ball to 'break' a shield
        public void BreakShield()
        {
            activeShield = Shield.none;
        }

        //applies the health effect,
        void ApplyHealth(int lastPaddle)
        {  
            //start the timer to spawn a new power up
            nextPowerUpTimerNeedsReset = true;

            //increments the health of  the player that hit the power up
            pong.Players[lastPaddle].HealthUp();
        }

        //applies the reversed effect, inverts the controls of the players except for the player that hit the power up object
        void ApplyReversed(int lastPaddle)
        {
            //makes sure the effect gets reset
            applyPowerUpTimerNeedsReset = true;
            resetDuration = reversedDuration;
            isActive = true;

            //apply the inversed controls to each player except for the player that hit the power up
            foreach (Player player in pong.Players)
            {
                if (player.PlayerId != lastPaddle)
                    player.ReverseControls();
            }
        }

        //sets the texture of the reversed controls power up to a random texture(color)
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

        //applies the right effect when the power up object is hit by the ball
        public void GetHit(int lastPaddle)
        {
            //hides the object
            isVisible = false;

            //play the pick up sound
            soundEffects.Insert(0, pickUpSound.CreateInstance());
            soundEffects[0].Play();

            //apply the current power up
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
    
        //position needs to be accesible by ball for collision detection
        public Vector2 Position
        {
            get { return position; }
        }

        //origin needs to be accesible by ball for collision detection
        public Vector2 Origin
        {
            get { return origin; }
        }
        
        //isVisible needs to be accesible by ball to check if a collision could occur in the first place
        public bool IsVisible
        {
            get { return isVisible; }
        }

        //activeShield needs to be accesible by ball for collision with the sides
        public Shield ActiveShield
        {
            get { return activeShield; }
        }

        //returns the current power up if it is active
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
