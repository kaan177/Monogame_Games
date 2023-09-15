using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace pong
{
    public class Pong : Game
    {   //Class variables.
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        Texture2D player1Tex, player2Tex;
        public static Vector2 screenSize;
        Ball ball;
        Player player1, player2;
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
            screenSize = new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Constructing players.
            player1Tex = Content.Load<Texture2D>("rodeSpeler");
            player2Tex = Content.Load<Texture2D>("blauweSpeler");

            player1 = new Player(new Vector2(0, screenSize.Y/2 - player1Tex.Height/2), player1Tex, Keys.W, Keys.S, 1);
            player2 = new Player(new Vector2(screenSize.X - player2Tex.Width, screenSize.Y/2 - player2Tex.Height/2), player2Tex, Keys.Up, Keys.Down, 2);

            //Constructing the ball.
            ball = new Ball(screenSize/2, Content);
        }

        protected override void Update(GameTime gameTime)
        {   
            //Updating game objects.
            ball.Update();
            
            player1.Update();
            player2.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();

            //Drawing game objects.
            ball.Draw(spriteBatch);

            player1.Draw(spriteBatch); 
            player2.Draw(spriteBatch);

            spriteBatch.End();            
        }
    }
}