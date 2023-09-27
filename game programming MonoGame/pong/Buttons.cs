using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace pong
{
    internal class Button
    {   
        MouseState mouse, previousMouse;
        Vector2 topLeftPosition, textPosition;
        string buttonText;
        public bool isPressed;
        bool multiColorTexture;
        Texture2D buttonTexture;
        Rectangle mouseDetector;
        SpriteFont standardFont;
        Color buttonColor;

        public Button(Vector2 _topLeftPoint, Vector2 _size, string _buttonText, Texture2D _buttonTexture, SpriteFont _standardFont, bool _multiColorTexture)
        {

            //Loading and setting standard variables
            topLeftPosition = _topLeftPoint;
            buttonText = _buttonText;
            buttonTexture = _buttonTexture;
            standardFont = _standardFont;
            multiColorTexture = _multiColorTexture;

            mouseDetector = new Rectangle(_topLeftPoint.ToPoint(), _size.ToPoint());
            textPosition = mouseDetector.Center.ToVector2() - standardFont.MeasureString(buttonText) / 2; //Centering the text in the rectangle
            isPressed = false;
            buttonColor = Color.Green;
            multiColorTexture = _multiColorTexture;
        }
        public void Update()
        {
            //Detecting if the mouse is with in the button and if its pressed
            previousMouse = mouse;
            mouse = Mouse.GetState();
            if (mouseDetector.Contains(mouse.Position))
            {
                buttonColor = Color.Blue;
                if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton != ButtonState.Pressed)
                    isPressed = !isPressed;

            }
            else
            {
                if (isPressed)
                {
                    buttonColor = Color.Green;
                }
                else
                    buttonColor = Color.Red;
            }
        }
        public void Draw(SpriteBatch _spriteBatch)
        {   
            //Drawing the button texture and the text withi
            //n
            if(multiColorTexture)
                _spriteBatch.Draw(buttonTexture, topLeftPosition, new Color(buttonColor.R * 0.1f, buttonColor.G * 0.1f, buttonColor.B * 0.1f, 255));
            else
                _spriteBatch.Draw(buttonTexture, topLeftPosition, buttonColor);
            _spriteBatch.DrawString(standardFont, buttonText, textPosition, Color.Black);
        }


    }
}
