using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace pong
{
     class Player
    {   
        //Class variables.
        Vector2 position;
        Texture2D paddleTex, hearthTex;
        Keys keyup, keydown;
        KeyboardState keyboard;
        int playerId;
        int maxHealth = 3;
        int health;
        int speed = 5;


        
        public Player(Vector2 _startPosition, Texture2D _paddleTex, Keys _keyUp, Keys _keyDown, int _playerId, ContentManager _content)
        { 
            //Constructing variables.
            position = _startPosition;
            paddleTex = _paddleTex;
            keyup = _keyUp;
            keydown = _keyDown;
            playerId = _playerId;
            health = maxHealth;
            hearthTex = _content.Load<Texture2D>("hartje");


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
                    _spriteBatch.Draw(hearthTex, new Vector2(i * hearthTex.Width, 0), Color.White);
                }
            }
            if (playerId == 2)
            {
                for (int i = 0; i < health; i++)
                {
                    _spriteBatch.Draw(hearthTex, new Vector2(Pong.screenSize.X - (i + 1) * hearthTex.Width, 0), Color.White);
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
            if (health <= 0)
            {
                //Code to handle dying

            }
        }

        public int Height
        {
            get { return sprite.Height; }
        }
        public int Width
        {
            get { return sprite.Width; }
        }
        public Vector2 Position
        {
            get { return position; }
        }
    }
}
