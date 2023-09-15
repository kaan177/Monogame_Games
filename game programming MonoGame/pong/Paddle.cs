using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace pong
{
     class Paddle
    {   
        //Class variables.
        Vector2 position;
        Texture2D sprite;
        Keys keyup, keydown;
        KeyboardState keyboard;
        int speed = 5;


        
        public Paddle(Vector2 _startposition, Texture2D _sprite, Keys _keyup, Keys _keydown)
        { 
            //Constructing variables.
            position = _startposition;
            sprite = _sprite;
            keyup = _keyup;
            keydown = _keydown;
            
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
    }
}
