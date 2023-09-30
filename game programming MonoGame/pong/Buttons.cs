using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace pong
{
    internal class Button
    {   
        MouseState mouse, previousMouse;
        Vector2 topLeftPosition, textPosition, size;
        string buttonText;
        public bool isPressed;
        Texture2D buttonTexture;
        Rectangle mouseDetector;
        SpriteFont standardFont;
        Color isPressedCol, isNotPressedCol;
        float colorHoverdFactor;

        public Button(Vector2 _topLeftPoint, Vector2 _size, string _buttonText, Texture2D _buttonTexture, SpriteFont _standardFont, Color _isPressedCol, Color _isNotPressedCol)
        {

            //Loading and setting standard variables
            buttonText = _buttonText;
            buttonTexture = _buttonTexture;
            standardFont = _standardFont;
            isPressedCol = _isPressedCol;
            isNotPressedCol = _isNotPressedCol;
            size = _size;

            topLeftPosition = _topLeftPoint;
            mouseDetector = new Rectangle(_topLeftPoint.ToPoint(), _size.ToPoint());
            textPosition = mouseDetector.Center.ToVector2() - standardFont.MeasureString(buttonText) / 2; //Centering the text in the rectangle
            isPressed = false;
            colorHoverdFactor = 0.5f;
        }
        public void Update()
        {
            //Detecting if the mouse is with in the button and if its pressed
            previousMouse = mouse;
            mouse = Mouse.GetState();
            if (mouseDetector.Contains(mouse.Position))
            {
                colorHoverdFactor = 1.0f;
                if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton != ButtonState.Pressed)
                    isPressed = !isPressed;

            }
            else
            {
                colorHoverdFactor = 0.5f;
            }
            
        }
        public void Draw(SpriteBatch _spriteBatch)
        {   
            //Drawing the button texture and the text
            if (isPressed) 
            { 
                _spriteBatch.Draw(buttonTexture, topLeftPosition, isPressedCol * colorHoverdFactor);
            }
            else
            {
                _spriteBatch.Draw(buttonTexture, topLeftPosition, isNotPressedCol * colorHoverdFactor);
            }
            _spriteBatch.DrawString(standardFont, buttonText, textPosition, Color.Black);
        }
        public void UpdatePosition(Vector2 position)
        {
            topLeftPosition = position;
            mouseDetector = new Rectangle(topLeftPosition.ToPoint(), size.ToPoint());
            textPosition = mouseDetector.Center.ToVector2() - standardFont.MeasureString(buttonText) / 2;

        }

    }
}
