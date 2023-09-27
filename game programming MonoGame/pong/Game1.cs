using System;
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

        Ball ball;
        Player player1, player2;
        Emotes emotes;

        MainMenu mainMenu;
        GameOverScreen gameOverScreen;

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

            //Constructing Objects.

            player1 = new Player(new Vector2(0, screenSize.Y / 2), "rodeSpeler", Keys.W, Keys.S, 0, Content, this);
            player2 = new Player(new Vector2(screenSize.X, screenSize.Y / 2), "blauweSpeler", Keys.Up, Keys.Down, 1, Content, this);

            mainMenu = new MainMenu(standardFont, Content);
            gameOverScreen = new GameOverScreen(standardFont, Content);

            ball = new Ball(screenSize / 2, Content, player1, player2);

            emotes = new Emotes(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            lastGameState = gameState;

            if (gameState == GameState.MainMenu)
            {   
                IsMouseVisible = true;
                mainMenu.Update();
                if (mainMenu.start.isPressed)
                    gameState = GameState.Playing; mainMenu.start.isPressed = false;

                if (mainMenu.exit.isPressed)
                    Exit();
            }

            if (gameState == GameState.GameOver)
            {   
                gameOverScreen.Update();

                emotes.HandleInput(gameTime);
                IsMouseVisible = true;
                if (gameOverScreen.replayBut.isPressed)
                    gameState = GameState.Playing; gameOverScreen.replayBut.isPressed = false;

                if (gameOverScreen.mainMenuBut.isPressed)
                    gameState = GameState.MainMenu; gameOverScreen.mainMenuBut.isPressed = false;
            }

            if (gameState == GameState.Playing)
            {
                IsMouseVisible = false;
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
                mainMenu.Draw(spriteBatch);
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
                gameOverScreen.Draw(spriteBatch);
                emotes.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        public void GameOver(int losingPlayer)
        {
            if (losingPlayer == 1)
            {
                gameOverScreen.dynamicText = "Game Over: Red wins!";
                gameOverScreen.dynamicCol = Color.Red;
            }
            else
            {
                gameOverScreen.dynamicText = "Game Over: Blue wins!";
                gameOverScreen.dynamicCol = Color.Blue;
            }
            gameState = GameState.GameOver;
        }
        public static Random Random
        {
            get; private set;
        }
    }
}