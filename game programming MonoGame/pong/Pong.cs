/*
This code implements the game of pong

CONTROLS:
    w/s: Red player
    AroowUp/ArrowDown: Blue player
    1/2: Green player
    9/0: Magenta player
    e/numpad 7: Emotes for Red player and Blue player

This verion of pong has several optional game modes.

* Four player mode is activated by the isFourPlayers boolean. 
    When four player mode is activated the screen shrinks to a cube and two extra players are spawned on the top and bottem of the screen.
   

* The bots mode replaces all players except the red player with bots
    Bots mode is activated by the isBots boolean
    When bots mode is on extreme difficulty can be selected for even harder bots

* The powerups mode enables powerups to randomly spawn on the screen.
    If the ball hits them the powerup is apllied to either the last player or the ball or all other players.
    The powerups are handeled by the PowerUps class.

GAME STATES
Game states are switched in the update method.

*Main Menu
    The main menu consists of a couple of buttons which allow the different game modes to be selected.
    When the start button is pressed the game state is switched to Playing

*Playing
    When the Playing state is active the players and bal are drawn and updated.
    When a Player wins the game state transitions to Game Over

*Game Over
    This game state has two buttons and a text that displays who has won
    The replay button switches back to the Playing state, and the main menu button switches to the Main Menu state

*/
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using pong.Content;

namespace pong
{
    enum GameState { MainMenu, Playing, GameOver };
    public class Pong : Game
    {
        public static Vector2 screenSize;
        Vector2 CenterOfScreen;

        GameState lastGameState, gameState;
        // The different game mode stettings
        bool isExtremeDifficulty;
        bool isFourPlayers;
        bool isBots;
        bool isPowerUps;
        bool playMusic;

        //Helper variables
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont standardFont;
        Player[] players = new Player[8];
        Ball ball;
        Emotes emotes;
        PowerUps powerUps;
        Song music;

        //The diiferent UI screens
        MainMenu mainMenu;
        GameOverScreen gameOverScreen;

        //Initializing pong
        static void Main()
        {
            Pong game = new Pong();
            game.Run();
        }

        //Pong constructor
        public Pong()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Random = new Random();
            IsMouseVisible = true;
        }

        //Loading and creating different objects, textures, varaibles.
        protected override void LoadContent()
        {
            //Loading in the music
            music = Content.Load<Song>("shittyMusic");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.3f;

            //Setting the start Game State
            gameState = GameState.MainMenu;
            isFourPlayers = false;
            isBots = false;
            isPowerUps = false;
            playMusic = false;
            isExtremeDifficulty = false;

            //Setting helper variables
            screenSize = new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            CenterOfScreen = new Vector2(screenSize.X, screenSize.Y) / 2;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Loading the font.
            standardFont = Content.Load<SpriteFont>("standardFont");

            //Instantiating objects.
            ball = new Ball(screenSize / 2, Content, this);
            emotes = new Emotes(Content);
            powerUps = new PowerUps(Content, "powerUp", Vector2.Zero, this);
            mainMenu = new MainMenu(standardFont, Content);
            gameOverScreen = new GameOverScreen(standardFont, Content);

            //Instanitiating the players and the bots
            players[0] = new Player(new Vector2(0, screenSize.Y / 2), "rodeSpeler", Keys.W, Keys.S, 0, true, Content, this);
            players[1] = new Player(new Vector2(screenSize.X, screenSize.Y / 2), "blauweSpeler", Keys.Up, Keys.Down, 1, true, Content, this);
            players[2] = new Player(new Vector2(screenSize.X / 2, 0), "groeneSpeler", Keys.D2, Keys.D1, 2, false, Content, this);
            players[3] = new Player(new Vector2(screenSize.X / 2, screenSize.Y), "rozeSpeler", Keys.D0, Keys.D9, 3, false, Content, this);
            players[4] = new Bot(new Vector2(0, screenSize.Y / 2), "rodeSpeler", Keys.A, Keys.A, 0, true, Content, this);
            players[5] = new Bot(new Vector2(screenSize.X, screenSize.Y / 2), "blauweSpeler", Keys.A, Keys.A, 1, true, Content, this);
            players[6] = new Bot(new Vector2(screenSize.X / 2, 0), "groeneSpeler", Keys.A, Keys.A, 2, false, Content, this);
            players[7] = new Bot(new Vector2(screenSize.X / 2, screenSize.Y), "rozeSpeler", Keys.A, Keys.A, 3, false, Content, this);
        }

//--------------------- UPDATE METHOD ----------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------------------------------------------
        protected override void Update(GameTime gameTime)
        {   //Playing the music
            if (playMusic)
            {
                if (MediaPlayer.State == MediaState.Stopped)
                    MediaPlayer.Play(music);
                if (MediaPlayer.State == MediaState.Paused)
                    MediaPlayer.Resume();
            }
            else
                if (MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Pause();

            //Calling Update in the different game states and switching between game states
            
            if (gameState == GameState.MainMenu) //Gamestate: Main Menu
            {
                //Setting variables and settings when entering the main menu
                if (lastGameState != GameState.MainMenu)
                {
                    IsMouseVisible = true;
                    if (graphics.GraphicsDevice.Viewport.Width != 800)
                    {
                        graphics.PreferredBackBufferWidth = 800;
                        graphics.ApplyChanges();
                        screenSize = new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
                        CenterOfScreen = screenSize / 2;
                    }
                }
                //Updating main menu.
                mainMenu.Update();

                //Setting the settings.
                isFourPlayers = mainMenu.player4ModeBut.isPressed;
                isBots = mainMenu.botEasyBut.isPressed;
                playMusic = mainMenu.musicBut.isPressed;
                isPowerUps = mainMenu.powerUpsBut.isPressed;

                if (mainMenu.botHardBut.isPressed)
                {
                    if (!isBots)
                    {
                        mainMenu.botHardBut.isPressed = false;
                    }
                    else
                    {
                        isExtremeDifficulty = true; 
                    }
                }
                else
                    isExtremeDifficulty = false;

                //Switching the game state if the start button is pressed.
                if (mainMenu.startBut.isPressed)
                {
                    gameState = GameState.Playing;
                    mainMenu.startBut.isPressed = false;
                }

                if (mainMenu.exitBut.isPressed)
                    Exit();
            }

            if (gameState == GameState.GameOver) //Gamestate: Game Over
            {
                //Setting variables and settings when entering the Game Over state
                if (lastGameState != GameState.GameOver)
                {
                    IsMouseVisible = true;
                }
                lastGameState = gameState;
                gameOverScreen.Update();

                //Enable emotes only in two player mode.
                if (!isFourPlayers)
                    emotes.HandleInput(gameTime);

                //Switching to Playing state when replay button is pressed.
                if (gameOverScreen.replayBut.isPressed)
                {
                    gameState = GameState.Playing; 
                    gameOverScreen.replayBut.isPressed = false;
                }

                //Switching to Main Menu state when main menu button is pressed.
                if (gameOverScreen.mainMenuBut.isPressed)
                {
                    gameState = GameState.MainMenu; 
                    gameOverScreen.mainMenuBut.isPressed = false;
                }
            }

            if (gameState == GameState.Playing) //Gamestate: Playing
            {
                //Setting variables and settings when entering the Playing state
                if (lastGameState != gameState)
                {
                    //make the playing field square if in 4 player mode
                    IsMouseVisible = false;
                    if (isFourPlayers && graphics.GraphicsDevice.Viewport.Width != 480)
                        graphics.PreferredBackBufferWidth = 480;
                    else if (!isFourPlayers && graphics.GraphicsDevice.Viewport.Width != 800)
                        graphics.PreferredBackBufferWidth = 800;
                    graphics.ApplyChanges();
                    screenSize = new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
                    CenterOfScreen = screenSize / 2;

                    //Reseting the ball, powerups and players.
                    ball.ReCalculateStartPosition();
                    ball.Reset();
                    powerUps.Reset();
                    foreach (Player player in players)
                    {
                        player.ReCalculateVariables();
                        player.GameReset();
                    }
                    //Turning on and off the players and bots depending on if the bots are enabled.
                    if (isBots)//Bots on
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
                    else//Bots off
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
                //Updating the ball, powerups and players.
                ball.Update(gameTime);
                if(isPowerUps)
                    powerUps.Update(gameTime);
                foreach (Player player in players)
                {
                    player.Update(gameTime);
                }

                //Enable emotes only in two player mode
                if (!isFourPlayers)
                    emotes.HandleInput(gameTime);
            }
        }

//--------------------- DRAW METHOD ----------------------------------------------------------------------------------------------------------------------
//----------------------------------------------------------------------------------------------------------------------------------------------------------
        protected override void Draw(GameTime gameTime)
        {
            //Making the Background black and removing all previous sprites.
            GraphicsDevice.Clear(Color.Black);
            //Begining the proces of drawing all the sprites
            spriteBatch.Begin();

            //Drawing the different Game States and its different components.
            if (gameState == GameState.MainMenu)
            {
                mainMenu.Draw(spriteBatch);
            }
            if (gameState == GameState.Playing)
            {
                if(isPowerUps)
                    powerUps.Draw(spriteBatch);
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
                gameOverScreen.Draw(spriteBatch);
                emotes.Draw(spriteBatch);
            }

            spriteBatch.End();
            //Ending the drawing proces
        }
//--------------------CHECK GAME OVER METHOD-----------------------------------------------------------------------------------------------------------------------------------------------
//-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        public void CheckGameOver()
        {
            //Collecting information about which and how many players are still alive.
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
                if (!isFourPlayers)//When the game mode is two players
                {
                    if (alivePlayers <= 1)
                    {
                        switch (livingPlayer)
                        {
                            case 0:
                                gameOverScreen.victoryText = "Game Over: Red wins!";
                                gameOverScreen.victoryCol = Color.Red;
                                break;
                            case 1:
                                gameOverScreen.victoryText = "Game Over: Blue wins!";
                                gameOverScreen.victoryCol = Color.Blue;
                                break;
                        }
                        gameState = GameState.GameOver;
                    }
                }
                else// Wanneer de game mode vier spelers is
                {
                    if (!players[0].IsAlive)
                    {
                        if (alivePlayers <= 1)
                        {
                            switch (livingPlayer)
                            {
                                case 1:
                                    gameOverScreen.victoryText = "Game Over: Blue wins!";
                                    gameOverScreen.victoryCol = Color.Blue;
                                    break;
                                case 2:
                                    gameOverScreen.victoryText = "Game Over: Green wins!";
                                    gameOverScreen.victoryCol = Color.Green;
                                    break;
                                case 3:
                                    gameOverScreen.victoryText = "Game Over: Pink wins!";
                                    gameOverScreen.victoryCol = Color.Magenta;
                                    break;
                            }
                        }
                        else
                        {
                            gameOverScreen.victoryText = "Game Over: Bots win!";
                            gameOverScreen.victoryCol = Color.White;    
                        }
                        gameState = GameState.GameOver;
                    }
                    else if(alivePlayers <=1)//When the player has won from the bots
                    {
                        gameOverScreen.victoryText = "Game Over: Red wins!";
                        gameOverScreen.victoryCol = Color.Red;
                        gameState = GameState.GameOver;
                    }
                }
            }
            else//When bots are disabled.
            {
                if (alivePlayers <= 1)
                {
                    switch (livingPlayer)
                    {
                        case 0:
                            gameOverScreen.victoryText = "Game Over: Red wins!";
                            gameOverScreen.victoryCol = Color.Red;
                            break;
                        case 1:
                            gameOverScreen.victoryText = "Game Over: Blue wins!";
                            gameOverScreen.victoryCol = Color.Blue;
                            break;
                        case 2:
                            gameOverScreen.victoryText = "Game Over: Green wins!";
                            gameOverScreen.victoryCol = Color.Green;
                            break;
                        case 3:
                            gameOverScreen.victoryText = "Game Over: Pink wins!";
                            gameOverScreen.victoryCol = Color.Magenta;
                            break;
                    }
                    gameState = GameState.GameOver;
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
        public bool IsExtremeDifficulty
        {
            get { return isExtremeDifficulty; }
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
        internal PowerUps PowerUps
        {
            get { return powerUps; }
        }

    }
}