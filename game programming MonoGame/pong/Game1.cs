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
        Paddle player1, player2;
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

            player1 = new Paddle(new Vector2(0, screenSize.Y/2 - player1Tex.Height/2), player1Tex, Keys.W, Keys.S);
            player2 = new Paddle(new Vector2(screenSize.X - player2Tex.Width, screenSize.Y/2 - player2Tex.Height/2), player2Tex, Keys.Up, Keys.Down);

            //Constructing the ball.
            ball = new Ball(screenSize/2, Content, player1, player2);
        }

        protected override void Update(GameTime gameTime)
        {   
            //Updating game objects.
            ball.Update(gameTime);
            
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