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

        public Button player4ModeBut, powerUpsBut, botEasyBut, botHardBut, musicBut, startBut, exitBut;

        Vector2 buttonSize;
        Vector2 player4ModePos, powerUpsPos, botEasyPos, botHardPos, musicPos, startPos, exitPos;
        string player4ModeStr, powerUpsStr, botEasyStr,botHardStr, musicStr, startStr, exitStr;
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

            player4ModePos = new Vector2(Pong.screenSize.X/2, Pong.screenSize.Y/7*2) - buttonSize/2;
            powerUpsPos = new Vector2(Pong.screenSize.X / 2, Pong.screenSize.Y / 7 * 3) - buttonSize / 2;
            botEasyPos = new Vector2(Pong.screenSize.X / 40 * 15, Pong.screenSize.Y / 7 * 4) - buttonSize / 2;
            botHardPos = new Vector2(Pong.screenSize.X / 40 * 25, Pong.screenSize.Y / 7 * 4) - buttonSize / 2;
            musicPos = new Vector2(Pong.screenSize.X / 2, Pong.screenSize.Y / 7 * 5) - buttonSize / 2;
            startPos = new Vector2(Pong.screenSize.X / 2, Pong.screenSize.Y / 7 * 6) - buttonSize / 2;
            exitPos = new Vector2(Pong.screenSize.X,0) - new Vector2(exitTex.Width, 0);

            player4ModeStr = "4 player";
            powerUpsStr = "Power Ups";
            botEasyStr = "Bot's";
            botHardStr = "Hard Bot's";
            musicStr = "Music";
            startStr = "Start";
            exitStr = "";

            player4ModeBut = new Button(player4ModePos, buttonSize, player4ModeStr, buttonTex, _standardFont, Color.Green, Color.Red);
            powerUpsBut = new Button(powerUpsPos, buttonSize, powerUpsStr, buttonTex, _standardFont, Color.Green, Color.Red);
            botEasyBut = new Button(botEasyPos, buttonSize, botEasyStr, buttonTex, _standardFont, Color.Green, Color.Red);
            botHardBut = new Button(botHardPos, buttonSize, botHardStr, buttonTex, _standardFont, Color.Green, Color.Red);
            musicBut = new Button(musicPos, buttonSize, musicStr, buttonTex, _standardFont, Color.Green, Color.Red);
            startBut = new Button(startPos, buttonSize, startStr, buttonTex, _standardFont, Color.Green, Color.Green);
            exitBut = new Button(exitPos, buttonSize, exitStr, exitTex, _standardFont, Color.Blue, Color.Blue);

            gameNamePosition = new Vector2(Pong.screenSize.X / 2, Pong.screenSize.Y / 6) - standardFont.MeasureString(gameName) / 2;

            
        }
        public void Update()
        {   
            //Updating Buttons
            player4ModeBut.Update();
            powerUpsBut.Update();
            botEasyBut.Update();   
            botHardBut.Update();
            musicBut.Update();
            startBut.Update();
            exitBut.Update();            
        }

        public void Draw(SpriteBatch _spriteBatch)
        {   
            //Drawing Buttons and Text
            _spriteBatch.DrawString(standardFont, gameName, gameNamePosition, Color.White);
            player4ModeBut.Draw(_spriteBatch);
            powerUpsBut.Draw(_spriteBatch);
            botEasyBut.Draw(_spriteBatch);
            botHardBut.Draw(_spriteBatch);
            musicBut.Draw(_spriteBatch);
            startBut.Draw(_spriteBatch);
            exitBut.Draw(_spriteBatch);
        }
    }
}
