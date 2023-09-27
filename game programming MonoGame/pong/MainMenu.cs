using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pong
{
    internal class MainMenu
    {
        SpriteFont standardFont;

        string gameName = "Pong";
        Vector2 gameNamePosition;

        public Button player4ModeBut, powerUpsBut, botBut, startBut, exitBut;

        Vector2 buttonSize;
        Vector2 player4ModePos, powerUpsPos, botPos, startPos, exitPos;
        string player4ModeStr, powerUpsStr, botStr, startStr, exitStr;
        Texture2D buttonTex, exitTex;
              



        public MainMenu(SpriteFont _standardFont, ContentManager _content) 
        {   
            //Loading and setting standard variables
            standardFont = _standardFont;
            
            buttonTex = _content.Load<Texture2D>("Button");
            exitTex = _content.Load<Texture2D>("exitButton");

            //Calculating and setting button and text parameters
            //The position of the ui elements are based on fractions of the total screen lenght and screen height
            buttonSize = new Vector2(buttonTex.Width, buttonTex.Height);

            player4ModePos = new Vector2(Pong.screenSize.X/2, Pong.screenSize.Y/6*2) - buttonSize/2;
            powerUpsPos = new Vector2(Pong.screenSize.X / 2, Pong.screenSize.Y / 6 * 3) - buttonSize / 2;
            botPos = new Vector2(Pong.screenSize.X / 2, Pong.screenSize.Y / 6 * 4) - buttonSize / 2;
            startPos = new Vector2(Pong.screenSize.X / 2, Pong.screenSize.Y / 6 * 5) - buttonSize / 2;
            exitPos = new Vector2(0,0);

            player4ModeStr = "4 player";
            powerUpsStr = "Power Ups";
            botStr = "Bot";
            startStr = "Start";
            exitStr = "";

            player4ModeBut = new Button(player4ModePos, buttonSize, player4ModeStr, buttonTex, _standardFont);
            powerUpsBut = new Button(powerUpsPos, buttonSize, powerUpsStr, buttonTex, _standardFont);
            botBut = new Button(botPos, buttonSize, botStr, buttonTex, _standardFont);
            startBut = new Button(startPos, buttonSize, startStr, buttonTex, _standardFont);
            exitBut = new Button(exitPos, buttonSize, exitStr, exitTex, _standardFont);

            gameNamePosition = new Vector2(Pong.screenSize.X / 2, Pong.screenSize.Y / 6) - standardFont.MeasureString(gameName) / 2;

            
        }
        public void Update()
        {   
            //Updating Buttons
            player4ModeBut.Update();
            powerUpsBut.Update();
            botBut.Update();   
            startBut.Update();
            exitBut.Update();            
        }

        public void Draw(SpriteBatch _spriteBatch)
        {   
            //Drawing Buttons and Text
            _spriteBatch.DrawString(standardFont, gameName, gameNamePosition, Color.White);
            player4ModeBut.Draw(_spriteBatch);
            powerUpsBut.Draw(_spriteBatch);
            botBut.Draw(_spriteBatch);
            startBut.Draw(_spriteBatch);
            exitBut.Draw(_spriteBatch);
        }
    }
}
