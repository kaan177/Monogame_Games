using System;
using System.CodeDom;
using System.Diagnostics;
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
        bool isFourPlayers;
        bool isBots;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteFont standardFont;
        Player[] players = new Player[8];
        Ball ball;
        Emotes emotes;

        string dynamicGameOverText, gameOverText1, gameOverText2, welcomeText;
        Vector2 dynamicGameOverTextOrigin, gameOverText1Origin, gameOverText2Origin, welcomeTextOrigin;

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
            isFourPlayers = true;
            isBots = true;

            screenSize = new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            CenterOfScreen = new Vector2(screenSize.X, screenSize.Y) / 2;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            standardFont = Content.Load<SpriteFont>("standardFont");

            gameOverText1 = "Press <Space> to play again";
            gameOverText2 = "press <Escape> to return to welcome screen";
            welcomeText = "Welcome to Pong, press <Space> to start";
            gameOverText1Origin = standardFont.MeasureString(gameOverText1) / 2;
            gameOverText2Origin = standardFont.MeasureString(gameOverText2) / 2;

            welcomeTextOrigin = standardFont.MeasureString(welcomeText) / 2;

            //Constructing GameObjects.
            ball = new Ball(screenSize / 2, Content, this);
            emotes = new Emotes(Content);

            players[0] = new Player(new Vector2(0, screenSize.Y / 2), "rodeSpeler", Keys.W, Keys.S, 0, true, Content, this);
            players[1] = new Player(new Vector2(screenSize.X, screenSize.Y / 2), "blauweSpeler", Keys.Up, Keys.Down, 1, true, Content, this);
            players[2] = new Player(new Vector2(screenSize.X / 2, 0), "groeneSpeler", Keys.D2, Keys.D1, 2, false, Content, this);
            players[3] = new Player(new Vector2(screenSize.X / 2, screenSize.Y), "rozeSpeler", Keys.D0, Keys.D9, 3, false, Content, this);
            players[4] = new Bot(new Vector2(0, screenSize.Y / 2), "rodeSpeler", Keys.A, Keys.A, 0, true, Content, this, true);
            players[5] = new Bot(new Vector2(screenSize.X, screenSize.Y / 2), "blauweSpeler", Keys.A, Keys.A, 1, true, Content, this, true);
            players[6] = new Bot(new Vector2(screenSize.X / 2, 0), "groeneSpeler", Keys.A, Keys.A, 2, false, Content, this, true);
            players[7] = new Bot(new Vector2(screenSize.X / 2, screenSize.Y), "rozeSpeler", Keys.A, Keys.A, 3, false, Content, this, true);
        }

        protected override void Update(GameTime gameTime)
        {
            if (gameState == GameState.MainMenu)
            {
                if(lastGameState != GameState.MainMenu)
                {
                    if (graphics.GraphicsDevice.Viewport.Width != 800)
                    {
                        graphics.PreferredBackBufferWidth = 800;
                        graphics.ApplyChanges();
                        screenSize = new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
                        CenterOfScreen = screenSize / 2;
                    }
                }
                lastGameState = gameState;
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                    gameState = GameState.Playing;
            }

            if (gameState == GameState.GameOver)
            {
                lastGameState = gameState;
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
                    //make the playing field square if in 4 player mode
                    if(isFourPlayers && graphics.GraphicsDevice.Viewport.Width != 480)
                        graphics.PreferredBackBufferWidth = 480;
                    else if (!isFourPlayers && graphics.GraphicsDevice.Viewport.Width != 800)
                        graphics.PreferredBackBufferWidth = 800;
                    graphics.ApplyChanges();
                    screenSize = new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
                    CenterOfScreen = screenSize / 2;

                    ball.ReCalculateStartPosition();
                    ball.Reset();
                    foreach (Player player in players)
                    {
                        player.ReCalculateAfterScreenChange();
                        player.GameReset();
                    }
                    if (isBots)
                    {
                        for(int i = 1; i < 5; i++)
                        {
                            players[i].IsAlive = false;
                        }
                        if (!isFourPlayers)
                        {              
                            players[6].IsAlive = false;
                            players[7].IsAlive = false;
                        }
                    }
                    else
                    {
                        for (int i = 4; i < 8; i++)
                        {
                            players[i].IsAlive = false;
                        }
                        if (!isFourPlayers)
                        {
                            players[2].IsAlive = false;
                            players[3].IsAlive = false;
                        }
                    }
                }
                lastGameState = gameState;
                ball.Update(gameTime);
                foreach (Player player in players)
                {
                    player.Update(gameTime);
                }
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
                foreach (Player player in players)
                {
                    player.Draw(spriteBatch);
                }
                emotes.Draw(spriteBatch);
            }
            if (gameState == GameState.GameOver)
            {
                foreach (Player player in players)
                {
                    player.Draw(spriteBatch);
                }
                spriteBatch.DrawString(standardFont, dynamicGameOverText, CenterOfScreen - dynamicGameOverTextOrigin - new Vector2(0, gameOverText1Origin.Y * 2.2f), gameOverColor);
                spriteBatch.DrawString(standardFont, gameOverText1, CenterOfScreen - gameOverText1Origin, Color.White);
                spriteBatch.DrawString(standardFont, gameOverText2, CenterOfScreen - gameOverText2Origin + new Vector2(0, gameOverText1Origin.Y * 2.2f), Color.White);
                emotes.Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        public void GameOver()
        {
            //game over call only results in the game being over if only one player is alive
            int alivePlayers = 0;
            int livingPlayer = 0;
            foreach (Player player in players)
            {
                if(player.IsAlive)
                {
                    alivePlayers++;
                    livingPlayer = player.PlayerId;
                }
            }
            if(isBots)
            {
                if (!isFourPlayers)
                {
                    if (alivePlayers <= 1)
                    {
                        switch (livingPlayer)
                        {
                            case 0:
                                dynamicGameOverText = "Game Over: Red wins!";
                                gameOverColor = Color.Red;
                                break;
                            case 1:
                                dynamicGameOverText = "Game Over: Blue wins!";
                                gameOverColor = Color.Blue;
                                break;
                        }
                        gameState = GameState.GameOver;
                        dynamicGameOverTextOrigin = standardFont.MeasureString(dynamicGameOverText) / 2;
                    }
                }
                else
                {
                    if (!players[0].IsAlive)
                    {
                        if (alivePlayers <= 1)
                        {
                            switch (livingPlayer)
                            {
                                case 1:
                                    dynamicGameOverText = "Game Over: Blue wins!";
                                    gameOverColor = Color.Blue;
                                    break;
                                case 2:
                                    dynamicGameOverText = "Game Over: Green wins!";
                                    gameOverColor = Color.Green;
                                    break;
                                case 3:
                                    dynamicGameOverText = "Game Over: Pink wins!";
                                    gameOverColor = Color.Magenta;
                                    break;
                            }
                        }
                        else
                        {
                            dynamicGameOverText = "Game Over: Bots win!";
                            gameOverColor = Color.White;    
                        }
                        gameState = GameState.GameOver;
                        dynamicGameOverTextOrigin = standardFont.MeasureString(dynamicGameOverText) / 2;
                    }
                    else if(alivePlayers <=1)
                    {
                        dynamicGameOverText = "Game Over: Red wins!";
                        gameOverColor = Color.Red;
                        gameState = GameState.GameOver;
                        dynamicGameOverTextOrigin = standardFont.MeasureString(dynamicGameOverText) / 2;
                    }
                }
            }
            else
            {
                if (alivePlayers <= 1)
                {
                    switch (livingPlayer)
                    {
                        case 0:
                            dynamicGameOverText = "Game Over: Red wins!";
                            gameOverColor = Color.Red;
                            break;
                        case 1:
                            dynamicGameOverText = "Game Over: Blue wins!";
                            gameOverColor = Color.Blue;
                            break;
                        case 2:
                            dynamicGameOverText = "Game Over: Green wins!";
                            gameOverColor = Color.Green;
                            break;
                        case 3:
                            dynamicGameOverText = "Game Over: Pink wins!";
                            gameOverColor = Color.Magenta;
                            break;
                    }
                    gameState = GameState.GameOver;
                    dynamicGameOverTextOrigin = standardFont.MeasureString(dynamicGameOverText) / 2;
                }
            }

        }
        public bool IsFourPlayers
        {
            get { return isFourPlayers; }
        }
        public bool IsBots
        {
            get { return isBots; }
        }
        public static Random Random
        {
            get; private set;
        }
        internal Ball Ball
        {
            get { return ball; }
        }
        internal Player[] Players
        {
            get { return players; } 
        }

    }
}