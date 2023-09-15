using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace pong
{
     class Player
    {   
        //Class variables.
        Vector2 position;
        Texture2D sprite;
        Keys keyup, keydown;
        KeyboardState keyboard;
        int playerId;
        int maxHealth = 3;
        int health;
        int speed = 5;


        
        public Player(Vector2 _startPosition, Texture2D _sprite, Keys _keyUp, Keys _keyDown, int _playerId)
        { 
            //Constructing variables.
            position = _startPosition;
            sprite = _sprite;
            keyup = _keyUp;
            keydown = _keyDown;
            playerId = _playerId;
            health = maxHealth;
            
        }
        public void Update()
        {
            HandleInput();
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            //Drawing the sprite.
            _spriteBatch.Draw(sprite, position, Color.White);
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
                Debug.Print("keydown werkt");
            }
            // Resolving collision with the screen.
            if (position.Y < 0)
            {
                position.Y = 0;
            }

            if (position.Y > Pong.screenSize.Y - sprite.Height)
            {
                position.Y = Pong.screenSize.Y - sprite.Height;
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
    }
}
