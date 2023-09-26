using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pong
{
    internal class MainMenu
    {
        SpriteFont standardFont;

        string gameName = "Pong";
        Vector2 gameNamePosition;

        Button player4Mode, powerUps, bot, start, exit;

        Vector2 buttonSize, exitSize;
        Vector2 player4ModePos, powerUpsPos, botPos, startPos, exitPos;
        string player4ModeStr, powerUpsStr, botStr, startStr, exitStr;
        Texture2D buttonTex, exitTex;

        public bool exitSignal = false;
        public bool startSignal = false;

        



        public MainMenu(SpriteFont _standardFont, ContentManager _content) 
        {
            standardFont = _standardFont;
            

            buttonTex = _content.Load<Texture2D>("Button");
            exitTex = _content.Load<Texture2D>("exitButton");

            buttonSize = new Vector2(buttonTex.Width, buttonTex.Height);
            exitSize = new Vector2(exitTex.Width, exitTex.Height);

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

            player4Mode = new Button(player4ModePos, buttonSize, player4ModeStr, buttonTex, _standardFont);
            powerUps = new Button(powerUpsPos, buttonSize, powerUpsStr, buttonTex, _standardFont);
            bot = new Button(botPos, buttonSize, botStr, buttonTex, _standardFont);
            start = new Button(startPos, buttonSize, startStr, buttonTex, _standardFont);
            exit = new Button(exitPos, buttonSize, exitStr, exitTex, _standardFont);

            gameNamePosition = new Vector2(Pong.screenSize.X / 2, Pong.screenSize.Y / 6) - standardFont.MeasureString(gameName) / 2;

            
        }
        public void Update()
        {
            player4Mode.Update();
            powerUps.Update();
            bot.Update();   
            start.Update();
            exit.Update();

            if (exit.isPressed)
                exitSignal = true;

            if (start.isPressed)
                startSignal = true;
            
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.DrawString(standardFont, gameName, gameNamePosition, Color.White);
            player4Mode.Draw(_spriteBatch);
            powerUps.Draw(_spriteBatch);
            bot.Draw(_spriteBatch);
            start.Draw(_spriteBatch);
            exit.Draw(_spriteBatch);
        }
    }
}
