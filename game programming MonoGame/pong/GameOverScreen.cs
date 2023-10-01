using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pong
{
    internal class GameOverScreen
    {
        SpriteFont standardFont;

        public string victoryText;
        public Button mainMenuBut, replayBut;

        public Color victoryCol;

        Vector2 mainMenuPos, replayPos, victoryTextPos;
        Vector2 buttonSize;
        string mainMenuText, replayText;

        Texture2D buttonTex;


        public GameOverScreen(SpriteFont _standardFont, ContentManager _content) 
        {
            //Loading and setting standard variables
            standardFont = _standardFont;

            buttonTex = _content.Load<Texture2D>("Button");

            //Defaulding the victory tex and color to a default value for testing purpuses
            victoryText = "No one won";
            victoryCol = Color.White;


            //Calculating and setting button parameters
            //The position of the ui elements are based on fractions of the total screen lenght and screen height
            buttonSize = new Vector2(buttonTex.Width, buttonTex.Height);

            //Button positions
            mainMenuPos = new Vector2(Pong.screenSize.X/5, Pong.screenSize.Y/5 * 3) - buttonSize/2;
            replayPos = new Vector2(Pong.screenSize.X / 5 * 3, Pong.screenSize.Y / 5 * 3) - buttonSize/2;
            
            //Button text's
            mainMenuText = "Main menu";
            replayText = "Play again";

            //Initializing Buttons
            mainMenuBut = new Button(mainMenuPos, buttonSize, mainMenuText, buttonTex, standardFont, Color.Green, Color.Green);
            replayBut = new Button(replayPos, buttonSize, replayText, buttonTex, standardFont, Color.Green, Color.Green);
        }

        public void Update()
        {   
            //Updating the button positions of the buttons in case we come from the four player screen
            if (Pong.screenSize.X == 480) 
            {
                mainMenuBut.UpdatePosition(new Vector2(Pong.screenSize.X / 10 * 3, Pong.screenSize.Y / 5 * 3) - buttonSize / 2);
                replayBut.UpdatePosition(new Vector2(Pong.screenSize.X / 10 * 7, Pong.screenSize.Y / 5 * 3) - buttonSize / 2);
            }
            else
            {
                mainMenuBut.UpdatePosition(new Vector2(Pong.screenSize.X / 3, Pong.screenSize.Y / 5 * 3) - buttonSize / 2);
                replayBut.UpdatePosition(new Vector2(Pong.screenSize.X / 3 * 2, Pong.screenSize.Y / 5 * 3) - buttonSize / 2);
            }

            //Updating the buttons
            mainMenuBut.Update();
            replayBut.Update();
        }

        public void Draw(SpriteBatch _spriteBatch)
        {   
            //Updating the victory text position because the position can change deppending on the text that is written
            victoryTextPos = new Vector2(Pong.screenSize.X / 2, Pong.screenSize.Y / 2) - standardFont.MeasureString(victoryText) / 2;

            //Drawing the text and the buttons
            _spriteBatch.DrawString(standardFont, victoryText, victoryTextPos, victoryCol);
            mainMenuBut.Draw(_spriteBatch);
            replayBut.Draw(_spriteBatch);
        }
    }
}
