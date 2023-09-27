using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace pong
{
    internal class Player : GameObject
    {
        //Class variables.
        Texture2D redHeartTex, blueHeartTex, greenHeartTex, pinkHeartTex;
        Vector2 heartOrigin;
        SoundEffect damageSound;
        Keys keyup, keydown;
        KeyboardState keyboard;
        bool isVertical;
        bool isAlive;
        protected int playerId;
        int maxHealth = 3;
        int health;
        protected int speed = 360;


        public Player(Vector2 _startPosition, string _paddleTex, Keys _keyUp, Keys _keyDown, int _playerId, bool _isVertical, ContentManager _content, Pong _pong) : base(_content, _paddleTex, _startPosition, _pong)
        {
            //Constructing variables.        
            keyup = _keyUp;
            keydown = _keyDown;
            playerId = _playerId;
            isVertical = _isVertical;
            isAlive = true;
            health = maxHealth;
            redHeartTex = _content.Load<Texture2D>("hartje");
            blueHeartTex = _content.Load<Texture2D>("blueHeart");
            greenHeartTex = _content.Load<Texture2D>("greenHeart");
            pinkHeartTex = _content.Load<Texture2D>("pinkHeart");
            damageSound = _content.Load<SoundEffect>("damageSound");
            heartOrigin = new Vector2 (redHeartTex.Width, redHeartTex.Height) / 2;

            //offsets the start position to make up for origin offset
            switch(playerId)
            {
                case 0:
                    startPosition.X += origin.X;
                    break;
                case 1:
                    startPosition.X -= origin.X;
                    break;
                case 2:
                    startPosition.Y += origin.Y;
                    break;
                case 3: 
                    startPosition.Y -= origin.Y;
                    break;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (isAlive)
            {
                base.Draw(spriteBatch);

                if (!pong.IsFourPlayers)
                {
                    //drawing the health at the right position based on which player the health belongs to
                    if (playerId == 0)
                    {
                        for (int i = 0; i < health; i++)
                        {
                            spriteBatch.Draw(redHeartTex, new Vector2(20 + i * 2f * heartOrigin.X * 2, 20), null, Color.White * 0.5f, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < health; i++)
                        {
                            spriteBatch.Draw(redHeartTex, new Vector2(Pong.screenSize.X - (i + 1) * 2f * heartOrigin.X * 2 - 20, 20), null, Color.White * 0.5f, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                        }
                    }
                }
                else
                {
                    switch(playerId)
                    {
                        case 0:
                            for (int i = 0; i < health; i++)
                            {
                                spriteBatch.Draw(redHeartTex, new Vector2(Pong.screenSize.X / 2 - heartOrigin.X * 12.5f + i * 4 * heartOrigin.X, Pong.screenSize.Y / 2 - heartOrigin.Y * 2.5f), null, Color.White * 0.3f, 0f, heartOrigin, 2f, SpriteEffects.None, 0);
                            }
                            break;
                        case 1:
                            for (int i = 0; i < health; i++)
                            {
                                spriteBatch.Draw(blueHeartTex, new Vector2(Pong.screenSize.X / 2 + heartOrigin.X * 0.5f + i * 4 * heartOrigin.X, Pong.screenSize.Y / 2 + heartOrigin.Y * 2.5f), null, Color.White * 0.3f, 0f, heartOrigin, 2f, SpriteEffects.None, 0);
                            }
                            break;
                        case 2:
                            for (int i = 0; i < health; i++)
                            {
                                spriteBatch.Draw(greenHeartTex, new Vector2(Pong.screenSize.X / 2 + heartOrigin.X * 0.5f + i * 4 * heartOrigin.X, Pong.screenSize.Y / 2 - heartOrigin.Y * 2.5f), null, Color.White * 0.3f, 0f, heartOrigin, 2f, SpriteEffects.None, 0);
                            }
                            break;
                        case 3:
                            for (int i = 0; i < health; i++)
                            {
                                spriteBatch.Draw(pinkHeartTex, new Vector2(Pong.screenSize.X / 2 - heartOrigin.X * 12.5f + i * 4 * heartOrigin.X, Pong.screenSize.Y / 2 + heartOrigin.Y * 2.5f), null, Color.White * 0.3f, 0f, heartOrigin, 2f, SpriteEffects.None, 0);
                            }
                            break;
                    }
                }
            }
        }
        public override void Update(GameTime gameTime)
        {
            if (isAlive)
            {
                HandleInput();
                base.Update(gameTime);

                // Resolving collision with the screen and other paddles.
                if (isVertical)
                {
                    if (position.Y < origin.Y + 2 * origin.X)
                    {
                        position.Y = origin.Y + 2 * origin.X;
                        velocity.Y = 0f;
                    }
                    if (position.Y > Pong.screenSize.Y - origin.Y - 2 * origin.X)
                    {
                        position.Y = Pong.screenSize.Y - origin.Y - 2 * origin.X;
                        velocity.Y = 0f;
                    }
                }
                else
                {
                    if (position.X < origin.X + 2 * origin.Y)
                    {
                        position.X = origin.X + 2 * origin.Y;
                        velocity.X = 0f;
                    }
                    if (position.X > Pong.screenSize.X - origin.X - 2 * origin.Y)
                    {
                        position.X = Pong.screenSize.X - origin.X - 2 * origin.Y;
                        velocity.X = 0f;
                    }
                }
            }
        }

        protected virtual void HandleInput()
        {
            // Taking player input and setting velocity accordingly.
            keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(keyup) && !keyboard.IsKeyDown(keydown))
            {
                if (isVertical)
                    velocity.Y = -speed;
                else
                    velocity.X = speed;
            }
            else if (keyboard.IsKeyDown(keydown) && !keyboard.IsKeyDown(keyup))
            {
                if (isVertical)
                    velocity.Y = speed;
                else
                    velocity.X = -speed;
            }
            else
                velocity = Vector2.Zero;
        }
        public void TakeDamage(int damage)
        {
            health -= damage;
            damageSound.Play();

            if (health <= 0)
            {
                isAlive = false;
                //game over call only results in the game being over if only one player is alive
                pong.GameOver();
            }
        }
        public void GameReset()
        {
            isAlive = true;
            health = maxHealth;
            position = startPosition;
        }
        public virtual void ReCalculateAfterScreenChange()
        {
            switch (playerId)
            {
                case 0:
                    startPosition = new Vector2(origin.X, Pong.screenSize.Y / 2);
                    break;
                case 1:
                    startPosition = new Vector2(Pong.screenSize.X - origin.X, Pong.screenSize.Y / 2);
                    break;
                case 2:
                    startPosition = new Vector2(Pong.screenSize.X / 2, origin.Y);
                    break;
                case 3:
                    startPosition = new Vector2(Pong.screenSize.X / 2, Pong.screenSize.Y - origin.Y);
                    break;
            }

        }

        public bool IsAlive
        {
            get { return isAlive; }
            set { isAlive = value; }
        }
        public bool IsVertical
        {
            get { return isVertical; }
        }
        public int PlayerId
        {
            get { return playerId; }
        }
        public int Width
        {
            get { return texture.Width; }
        }
        public int Height
        {
            get { return texture.Height; }
        }
        public Vector2 OriginAdjustedPosition
        {
            get { return position - origin; }
        }
        public Vector2 OriginAdjustedLastPosition
        {
            get { return lastPosition - origin; }
        }
        public Vector2 Velocity
        {
            get { return velocity; }
        }
    }
}
