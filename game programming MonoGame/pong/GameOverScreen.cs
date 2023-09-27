using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace pong
{
    internal class GameOverScreen
    {
        SpriteFont standardFont;

        public string dynamicText;
        public Button mainMenuBut, replayBut;

        public Color dynamicCol;

        Vector2 mainMenuPos, replayPos, dynamicTextPos;
        Vector2 buttonSize;
        string mainMenuText, replayText;

        Texture2D buttonTex;


        public GameOverScreen(SpriteFont _standardFont, ContentManager _content) 
        { 
            standardFont = _standardFont;

            buttonTex = _content.Load<Texture2D>("Button");

            buttonSize = new Vector2(buttonTex.Width, buttonTex.Height);

            mainMenuPos = new Vector2(Pong.screenSize.X/3, Pong.screenSize.Y/5 * 3) - buttonSize/2;
            replayPos = new Vector2(Pong.screenSize.X / 3 * 2, Pong.screenSize.Y / 5 * 3) - buttonSize / 2;
            

            mainMenuText = "Main menu";
            replayText = "Play again";

            mainMenuBut = new Button(mainMenuPos, buttonSize, mainMenuText, buttonTex, standardFont);
            replayBut = new Button(replayPos, buttonSize, replayText, buttonTex, standardFont);
        }

        public void Update()
        {
            mainMenuBut.Update();
            replayBut.Update();
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            dynamicTextPos = new Vector2(Pong.screenSize.X / 2, Pong.screenSize.Y / 2) - standardFont.MeasureString(dynamicText) / 2;

            _spriteBatch.DrawString(standardFont, dynamicText, dynamicTextPos, dynamicCol);
            mainMenuBut.Draw(_spriteBatch);
            replayBut.Draw(_spriteBatch);
        }
    }
}
