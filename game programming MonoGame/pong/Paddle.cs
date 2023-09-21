using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace pong
{
     class Player : GameObject
    {
        //Class variables.
        Pong pong;
        Texture2D hearthTex;
        SoundEffect damageSound;
        Keys keyup, keydown;
        KeyboardState keyboard;
        int playerId;
        int maxHealth = 3;
        int health;
        int speed = 6;
        

        public Player(Vector2 _startPosition, string _paddleTex, Keys _keyUp, Keys _keyDown, int _playerId, ContentManager _content, Pong _pong) : base(_content, _paddleTex, _startPosition)
        {
            //Constructing variables.        
            pong = _pong;
            keyup = _keyUp;
            keydown = _keyDown;
            playerId = _playerId;
            health = maxHealth;
            hearthTex = _content.Load<Texture2D>("hartje");
            damageSound = _content.Load<SoundEffect>("damageSound");

            //offsets the start position to make up for origin offset
            if (playerId == 0)
                startPosition.X += origin.X;
            else
                startPosition.X -= origin.X;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            //drawing the health at the right position based on which player the health belongs to
            if (playerId == 0)
            {
                for (int i = 0; i < health; i++)
                {
                    spriteBatch.Draw(hearthTex, new Vector2(20 + i * 2f * hearthTex.Width, 20), null, Color.White * 0.5f, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                }
            }
            else
            {
                for (int i = 0; i < health; i++)
                {
                    spriteBatch.Draw(hearthTex, new Vector2(Pong.screenSize.X - (i + 1) * 2f * hearthTex.Width - 20, 20), null, Color.White * 0.5f, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                }
            }
        }        
        public override void Update(GameTime gameTime)
        {
            HandleInput();
            base.Update(gameTime);

            // Resolving collision with the screen.
            if (position.Y < origin.Y)
            {
                position.Y = origin.Y;
            }
            if (position.Y > Pong.screenSize.Y - origin.Y)
            {
                position.Y = Pong.screenSize.Y - origin.Y;
            }
        }

        void HandleInput()
        {
            // Taking player input and setting velocity accordingly.
            keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(keyup) && !keyboard.IsKeyDown(keydown))
            {
                velocity.Y = -360f;
            }
            else if (keyboard.IsKeyDown(keydown) && !keyboard.IsKeyDown(keyup))
            {
                velocity.Y = 360f;
            }
            else
                velocity.Y = 0;
           

        }
        public void TakeDamage(int damage)
        {
            health -= damage;
            damageSound.Play();

            if (health <= 0)
            {
                pong.GameOver(playerId);
            }
        }
        public void GameReset()
        {
            health = maxHealth;
            position = startPosition;
        }

        public int Width
        {
            get { return texture.Width; }
        }
        public int Height
        {
            get { return texture.Height; }
        }
        public Vector2 CollisionPosition
        {
            get { return position - origin; }
        }
    }
}
