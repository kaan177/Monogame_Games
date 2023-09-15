using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace pong
{
    public class Pong : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        Paddle player1, player2;
        public static Vector2 screenSize;
        Ball ball;

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
            player1 = new Paddle(new Vector2(0, 0), Content.Load<Texture2D>("rodeSpeler"), Keys.W, Keys.S);
            ball = new Ball(screenSize/2, Content);
        }

        protected override void Update(GameTime gameTime)
        {
            ball.Update();                          
            player1.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            ball.Draw(spriteBatch);
            player1.Draw(spriteBatch);
            spriteBatch.End();            
        }
    }
}