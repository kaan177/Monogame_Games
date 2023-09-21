using System;
using System.Drawing.Imaging;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using pong.Content;

namespace pong
{
    enum GameState { MainMenu, Playing, GameOver };
    public class Pong : Game
    {
        public static Vector2 screenSize;
        Vector2 CenterOfScreen;

        GameState lastGameState, gameState;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont standardFont;

        static Ball ball;
        static Player player1, player2;
        Emotes emotes;

        string dynamicGameOverText, gameOverText, welcomeText;
        Vector2 dynamicGameOverTextOrigin, gameOverTextOrigin, welcomeTextOrigin;

        Color gameOverColor;

        static void Main()
        {
            Pong game = new Pong();
            game.Run();
        }

        public Pong()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Random = new Random();
        }

        protected override void LoadContent()
        {
            gameState = GameState.MainMenu;

            screenSize = new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            CenterOfScreen = new Vector2(screenSize.X, screenSize.Y) / 2;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            standardFont = Content.Load<SpriteFont>("standardFont");

            gameOverText = "Press <Space> to play again, or press <Escape> to return to welcome screen";
            welcomeText = "Welcome to Pong, press <Space> to start";
            gameOverTextOrigin = standardFont.MeasureString(gameOverText) / 2;
            welcomeTextOrigin = standardFont.MeasureString(welcomeText) / 2;

            //Constructing players.

            player1 = new Player(new Vector2(0, screenSize.Y / 2), "rodeSpeler", Keys.W, Keys.S, 0, Content, this);
            player2 = new Player(new Vector2(screenSize.X, screenSize.Y / 2), "blauweSpeler", Keys.Up, Keys.Down, 1, Content, this);

            //Constructing the ball.
            ball = new Ball(screenSize / 2, Content, player1, player2);

            emotes = new Emotes(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            lastGameState = gameState;

            if (gameState == GameState.MainMenu)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    gameState = GameState.Playing;
            }

            if (gameState == GameState.GameOver)
            {
                emotes.HandleInput(gameTime);
                KeyboardState keyboard = Keyboard.GetState();
                if (keyboard.IsKeyDown(Keys.Space))
                    gameState = GameState.Playing;
                if (keyboard.IsKeyDown(Keys.Escape))
                    gameState = GameState.MainMenu;
            }

            if (gameState == GameState.Playing)
            {
                if (lastGameState != gameState)
                {
                    ball.Reset();
                    player1.GameReset();
                    player2.GameReset();
                }

                ball.Update(gameTime);
                player1.Update(gameTime);
                player2.Update(gameTime);
                emotes.HandleInput(gameTime);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            if (gameState == GameState.MainMenu)
            {
                spriteBatch.DrawString(standardFont, welcomeText, CenterOfScreen - welcomeTextOrigin, Color.White);
            }
            if (gameState == GameState.Playing)
            {
                ball.Draw(spriteBatch);
                player1.Draw(spriteBatch);
                player2.Draw(spriteBatch);
                emotes.Draw(spriteBatch);
            }
            if (gameState == GameState.GameOver)
            {
                player1.Draw(spriteBatch);
                player2.Draw(spriteBatch);
                spriteBatch.DrawString(standardFont, dynamicGameOverText, CenterOfScreen - dynamicGameOverTextOrigin - new Vector2(0, gameOverTextOrigin.Y * 1.1f), gameOverColor);
                spriteBatch.DrawString(standardFont, gameOverText, CenterOfScreen - gameOverTextOrigin + new Vector2(0, dynamicGameOverTextOrigin.Y * 1.1f), Color.White);
                emotes.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        public void GameOver(int losingPlayer)
        {
            if (losingPlayer == 1)
            {
                dynamicGameOverText = "Game Over: Blue wins!";
                gameOverColor = Color.Blue;
            }
            else
            {
                dynamicGameOverText = "Game Over: Red wins!";
                gameOverColor = Color.Red;
            }
            gameState = GameState.GameOver;
            dynamicGameOverTextOrigin = standardFont.MeasureString(dynamicGameOverText) / 2;
        }
        public static Random Random
        {
            get; private set;
        }
    }
}