using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace pong
{
    internal class Button
    {   
        /*Buttons are used in the Main Menu state and the Game Over state. The buttons are toggle butons meaning they can either be on or of. 
            This is represented by the public isPressed boolean. */
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
            //Centering the text in the rectangle.
            textPosition = mouseDetector.Center.ToVector2() - standardFont.MeasureString(buttonText) / 2; 
            isPressed = false;
            colorHoverdFactor = 0.5f;
        }
        public void Update()
        {
            //Detecting if the mouse is with in the button and if its pressed.
            previousMouse = mouse;
            mouse = Mouse.GetState();
            if (mouseDetector.Contains(mouse.Position))
            {// if the button is hovered by the mouse the button color is the full brightnes.
                colorHoverdFactor = 1.0f;
                //Check if the left button is being clicked and making sure that the button does not switch when the left button is held.
                if (mouse.LeftButton == ButtonState.Pressed && previousMouse.LeftButton != ButtonState.Pressed)
                    isPressed = !isPressed;

            }
            else
            {//when the button is not hovered the color brightnes is reduced.
                colorHoverdFactor = 0.5f;
            }
            
        }
        public void Draw(SpriteBatch _spriteBatch)
        {   
            //Switching the color depending on if its pressed or not
            if (isPressed) 
            {   
                _spriteBatch.Draw(buttonTexture, topLeftPosition, isPressedCol * colorHoverdFactor);
            }
            else
            {
                _spriteBatch.Draw(buttonTexture, topLeftPosition, isNotPressedCol * colorHoverdFactor);
            }
            //Drawing the text with in the button.
            _spriteBatch.DrawString(standardFont, buttonText, textPosition, Color.Black);
        }

        //This method updates the position for when the screensize is changed.
        public void UpdatePosition(Vector2 position)
        {
            topLeftPosition = position;
            mouseDetector = new Rectangle(topLeftPosition.ToPoint(), size.ToPoint());
            textPosition = mouseDetector.Center.ToVector2() - standardFont.MeasureString(buttonText) / 2;

        }

    }
}
