using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pong
{
    internal class Button
    {   
        MouseState mouse, previousMouse;
        Vector2 topLeftPoint, textPosition;
        string buttonText, name;
        public bool isPressed = false;
        Texture2D buttonTexture;
        Rectangle rectangle;
        SpriteFont standardFont;
        Color buttonColor = Color.Green;

        public Button(Vector2 _topLeftPoint, Vector2 _size, string _buttonText, Texture2D _buttonTexture, SpriteFont _standardFont)
        {   
            topLeftPoint = _topLeftPoint;
            buttonText = _buttonText;
            buttonTexture = _buttonTexture;
            rectangle = new Rectangle(_topLeftPoint.ToPoint(), _size.ToPoint());
            standardFont = _standardFont;
            textPosition = rectangle.Center.ToVector2() - standardFont.MeasureString(buttonText) / 2;
            
        }
        public void Update()
        {
            previousMouse = mouse;
            mouse = Mouse.GetState();
            if (rectangle.Contains(mouse.Position))
            {
                buttonColor = Color.Blue;
                if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton != ButtonState.Pressed)
                    isPressed = !isPressed;

            }
            else
                buttonColor = Color.Green;
        }
        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(buttonTexture, topLeftPoint, buttonColor);
            _spriteBatch.DrawString(standardFont, buttonText, textPosition, Color.Black);
        }


    }
}
