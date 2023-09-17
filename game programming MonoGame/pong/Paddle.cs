using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace pong
{
     class Player
    {
        //Class variables.
        Pong pong;
        Vector2 position, startPosition;
        Texture2D paddleTex, hearthTex;
        Keys keyup, keydown;
        KeyboardState keyboard;
        int playerId;
        int maxHealth = 3;
        int health;
        int speed = 5;
        SoundEffect damageSound;


        
        public Player(Vector2 _startPosition, Texture2D _paddleTex, Keys _keyUp, Keys _keyDown, int _playerId, ContentManager _content, Pong _pong)
        {
            //Constructing variables.        
            pong = _pong;
            startPosition = _startPosition;
            position = startPosition;
            paddleTex = _paddleTex;
            keyup = _keyUp;
            keydown = _keyDown;
            playerId = _playerId;
            health = maxHealth;
            hearthTex = _content.Load<Texture2D>("hartje");
            damageSound = _content.Load<SoundEffect>("damageSound");
        }
        public void Update()
        {
            HandleInput();
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            //Drawing the sprite.
            _spriteBatch.Draw(paddleTex, position, Color.White);
            if (playerId == 1)
            {
                for (int i = 0; i < health; i++)
                {
                    _spriteBatch.Draw(hearthTex, new Vector2(20 + i * 2f * hearthTex.Width, 20), null, Color.White * 0.5f, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                }
            }
            if (playerId == 2)
            {
                for (int i = 0; i < health; i++)
                {
                    _spriteBatch.Draw(hearthTex, new Vector2(Pong.screenSize.X - (i + 1) * 2f * hearthTex.Width - 20, 20), null, Color.White * 0.5f, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                }
            }
        }
        
        private void HandleInput()
        {
            // Taking player input and moving the paddles.
            keyboard = Keyboard.GetState();
            Keys[] key = keyboard.GetPressedKeys();

            if (keyboard.IsKeyDown(keyup))
            {
                position.Y -= speed;
            }

            if (keyboard.IsKeyDown(keydown))
            {
                position.Y += speed;
            }
            // Resolving collision with the screen.
            if (position.Y < 0)
            {
                position.Y = 0;
            }

            if (position.Y > Pong.screenSize.Y - paddleTex.Height)
            {
                position.Y = Pong.screenSize.Y - paddleTex.Height;
            }
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

        public void Reset()
        {
            health = maxHealth;
            position = startPosition;
        }

        public int Height
        {
            get { return paddleTex.Height; }
        }
        public int Width
        {
            get { return paddleTex.Width; }
        }
        public Vector2 Position
        {
            get { return position; }
        }
    }
}
