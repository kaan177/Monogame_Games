﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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

        Texture2D player1Tex, player2Tex;
        SpriteFont standardFont;

        Ball ball;
        Player player1, player2;

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
            IsMouseVisible = true;
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
            player1Tex = Content.Load<Texture2D>("rodeSpeler");
            player2Tex = Content.Load<Texture2D>("blauweSpeler");

            player1 = new Player(new Vector2(0, screenSize.Y / 2 - player1Tex.Height / 2), player1Tex, Keys.W, Keys.S, 1, Content, this);
            player2 = new Player(new Vector2(screenSize.X - player2Tex.Width, screenSize.Y / 2 - player2Tex.Height / 2), player2Tex, Keys.Up, Keys.Down, 2, Content, this);

            //Constructing the ball.
            ball = new Ball(screenSize / 2, Content, player1, player2);
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
                    player1.Reset();
                    player2.Reset();
                }

                ball.Update(gameTime);
                player1.Update();
                player2.Update();
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
            }
            if (gameState == GameState.GameOver)
            {
                player1.Draw(spriteBatch);
                player2.Draw(spriteBatch);
                spriteBatch.DrawString(standardFont, dynamicGameOverText, CenterOfScreen - dynamicGameOverTextOrigin - new Vector2(0, gameOverTextOrigin.Y * 1.1f), gameOverColor);
                spriteBatch.DrawString(standardFont, gameOverText, CenterOfScreen - gameOverTextOrigin + new Vector2(0, dynamicGameOverTextOrigin.Y * 1.1f), Color.White);
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
    }
}