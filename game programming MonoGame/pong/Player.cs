using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace pong
{
    internal class Player : MovingGameObject
    {
        /* The class player is responsible for the movement and drawing of the players, as well as keeping track of health and drawing health.
        The class inherits all functionality of MovingGameObject and GameObject. The class also serves as a framework to Bot as it inherets from Player */

        Texture2D regularTexture ,shrunkTexture,redHeartTex, blueHeartTex, greenHeartTex, pinkHeartTex;
        Vector2 heartOrigin;
        SoundEffect damageSound;
        Keys keyup, keydown, keyupStore, keydownStore;
        KeyboardState keyboard;
        bool isVertical;
        bool isAlive;
        protected int playerId;
        int maxHealth = 3;
        int health;
        protected const int speed = 360;


        public Player(Vector2 _startPosition, string _paddleTex, Keys _keyUp, Keys _keyDown, int _playerId, bool _isVertical, ContentManager _content, Pong _pong) : base(_content, _paddleTex, _startPosition, _pong)
        {
            //Constructing variables.        
            keyup = _keyUp;
            keydown = _keyDown;
            keyupStore = _keyUp;
            keydownStore = _keyDown;
            playerId = _playerId;
            isVertical = _isVertical;
            shrunkTexture = _content.Load<Texture2D>("kleine_" + _paddleTex);
            redHeartTex = _content.Load<Texture2D>("hartje");
            blueHeartTex = _content.Load<Texture2D>("blueHeart");
            greenHeartTex = _content.Load<Texture2D>("greenHeart");
            pinkHeartTex = _content.Load<Texture2D>("pinkHeart");
            damageSound = _content.Load<SoundEffect>("damageSound");
            heartOrigin = new Vector2 (redHeartTex.Width, redHeartTex.Height) / 2;
            regularTexture = texture;

            //offsets the start position to make up for origin offset
            switch(playerId)
            {
                case 0:
                    startPosition.X += origin.X;
                    break;
                case 1:
                    startPosition.X -= origin.X;
                    break;
                case 2:
                    startPosition.Y += origin.Y;
                    break;
                case 3: 
                    startPosition.Y -= origin.Y;
                    break;
            }

            //calls full reset to make sure all variables are in their initial state
            GameReset();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //only draws if the player is active / alive
            if (isAlive)
            {
                //draws the main texture
                base.Draw(spriteBatch);

                //draws at different positions based on the if the game is in 4 player mode or not
                if (!pong.IsFourPlayers)
                {
                    //drawing the health at the right position based on which player the health belongs to
                    if (playerId == 0)
                    {
                        for (int i = 0; i < health; i++)
                        {
                            //draws from top left to the right
                            spriteBatch.Draw(redHeartTex, new Vector2(20 + i * 2f * heartOrigin.X * 2, 20), null, Color.White * 0.5f, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < health; i++)
                        {
                            //draws from top right to left
                            spriteBatch.Draw(redHeartTex, new Vector2(Pong.screenSize.X - (i + 1) * 2f * heartOrigin.X * 2 - 20, 20), null, Color.White * 0.5f, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0);
                        }
                    }
                }
                else
                {
                    //drawing the health at the right position based on which player the health belongs to
                    switch (playerId)
                    {
                        case 0:
                            for (int i = 0; i < health; i++)
                            {
                                //draws above and to the left of the centre the screen
                                spriteBatch.Draw(redHeartTex, new Vector2(Pong.screenSize.X / 2 - heartOrigin.X * 12.5f + i * 4 * heartOrigin.X, Pong.screenSize.Y / 2 - heartOrigin.Y * 2.5f), null, Color.White * 0.3f, 0f, heartOrigin, 2f, SpriteEffects.None, 0);
                            }
                            break;
                        case 1:
                            for (int i = 0; i < health; i++)
                            {
                                //draws above and to the right of the centre the screen
                                spriteBatch.Draw(blueHeartTex, new Vector2(Pong.screenSize.X / 2 + heartOrigin.X * 0.5f + i * 4 * heartOrigin.X, Pong.screenSize.Y / 2 + heartOrigin.Y * 2.5f), null, Color.White * 0.3f, 0f, heartOrigin, 2f, SpriteEffects.None, 0);
                            }
                            break;
                        case 2:
                            for (int i = 0; i < health; i++)
                            {
                                //draws below and to the left of the centre the screen
                                spriteBatch.Draw(greenHeartTex, new Vector2(Pong.screenSize.X / 2 + heartOrigin.X * 0.5f + i * 4 * heartOrigin.X, Pong.screenSize.Y / 2 - heartOrigin.Y * 2.5f), null, Color.White * 0.3f, 0f, heartOrigin, 2f, SpriteEffects.None, 0);
                            }
                            break;
                        case 3:
                            for (int i = 0; i < health; i++)
                            {
                                //draws below and to the right of the centre the screen
                                spriteBatch.Draw(pinkHeartTex, new Vector2(Pong.screenSize.X / 2 - heartOrigin.X * 12.5f + i * 4 * heartOrigin.X, Pong.screenSize.Y / 2 + heartOrigin.Y * 2.5f), null, Color.White * 0.3f, 0f, heartOrigin, 2f, SpriteEffects.None, 0);
                            }
                            break;
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            //only draws if the player is active or alive
            if (isAlive)
            {
                //HandleInput and move the player(base) after velocity is updated by HandleInput
                HandleInput();
                base.Update(gameTime);

                // Resolving collision with the screen and other paddles.
                if (isVertical)
                {
                    //limit the movement between bottom and top of screen. With an offset of the width of the player to make sure it doesnt collide with players at the top of bottom of it 
                    if (position.Y < origin.Y + 2 * origin.X)
                    {
                        position.Y = origin.Y + 2 * origin.X;
                        velocity.Y = 0f;
                    }
                    if (position.Y > Pong.screenSize.Y - origin.Y - 2 * origin.X)
                    {
                        position.Y = Pong.screenSize.Y - origin.Y - 2 * origin.X;
                        velocity.Y = 0f;
                    }
                }
                else
                {
                    //limit the movement between left and right of screen. With an offset of the height of the player to make sure it doesnt collide with players at the left of right of it 
                    if (position.X < origin.X + 2 * origin.Y)
                    {
                        position.X = origin.X + 2 * origin.Y;
                        velocity.X = 0f;
                    }
                    if (position.X > Pong.screenSize.X - origin.X - 2 * origin.Y)
                    {
                        position.X = Pong.screenSize.X - origin.X - 2 * origin.Y;
                        velocity.X = 0f;
                    }
                }
            }
        }

        // Taking player input and setting velocity accordingly.
        protected virtual void HandleInput()
        {
            keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(keyup) && !keyboard.IsKeyDown(keydown))
            {
                if (isVertical)
                    velocity.Y = -speed;
                else
                    velocity.X = speed;
            }
            else if (keyboard.IsKeyDown(keydown) && !keyboard.IsKeyDown(keyup))
            {
                if (isVertical)
                    velocity.Y = speed;
                else
                    velocity.X = -speed;
            }
            else
                velocity = Vector2.Zero;
        }

        //TakeDamage is called by ball when the health of a player should decrement
        public void TakeDamage(int damage)
        {
            health -= damage;
            damageSound.Play();

            if (health <= 0)
            {
                isAlive = false;
                //game over call only results in the game being over if only one player is alive
                pong.CheckGameOver();
            }
        }

        //resets all variables that could change during the game , called at the start of the program or when the game is restarted via the main menu or game over screen
        public void GameReset()
        {
            isAlive = true;
            health = maxHealth;
            Reset();
        }

        //Make the players smaller 
        public void Shrink()
        {
            texture = shrunkTexture;
            origin = new Vector2(Width, Height) / 2f;

            //makes sure relevant variables are recalculated when the texture and origin changes
            ReCalculateVariables();
        }

        //Resets players to their original size
        public void UnShrink()
        {
            texture = regularTexture;
            origin = new Vector2(Width, Height) / 2f;

            //makes sure relevant variables are recalculated when the texture and origin changes
            ReCalculateVariables();
        }

        //Increments health if not already at full health, called by PowerUps when the health power up is hit
        public void HealthUp()
        {
            if (!(health == maxHealth))
            {
                health++;
            }
        }

        //inverts controls by rebinding the up and down key to the opposite values that were provided in the constructor. Called by PowerUps when the reverse controls power up is hit
        public void ReverseControls()
        {
            keyup = keydownStore;
            keydown = keyupStore;
        }

        //resets the controls to the default by rebinding the up and down key to the default values that were provided in the constructor. Called by PowerUps when the reverse controls power up ends
        public void ResetControls()
        {
            keyup = keyupStore;
            keydown = keydownStore;
        }

        //recalculates variables to make sure everything works correctly after a texture change
        public virtual void ReCalculateVariables()
        {
            switch (playerId)
            {
                case 0:
                    startPosition = new Vector2(origin.X, Pong.screenSize.Y / 2);
                    position.X = startPosition.X;
                    break;
                case 1:
                    startPosition = new Vector2(Pong.screenSize.X - origin.X, Pong.screenSize.Y / 2);
                    position.X = startPosition.X;
                    break;
                case 2:
                    startPosition = new Vector2(Pong.screenSize.X / 2, origin.Y);
                    position.Y = startPosition.Y;
                    break;
                case 3:
                    startPosition = new Vector2(Pong.screenSize.X / 2, Pong.screenSize.Y - origin.Y);
                    position.Y = startPosition.Y;
                    break;
            }
        }

        //used to check if the instance of Player is active by many different classes, so its needs to be accesible
        public bool IsAlive
        {
            get { return isAlive; }
            set { isAlive = value; }
        }

        //used to check the orientation of the paddle, mainly for collision detection by Ball
        public bool IsVertical
        {
            get { return isVertical; }
        }

        //needs to be accesed by many classes for checking which player (left 0 or 4, right 1 or 5, top 2 or 6,  bottom 3 or 7) the instance is
        public int PlayerId
        {
            get { return playerId; }
        }

        //needs to be accesible by ball for collision detection
        public int Width
        {
            get { return texture.Width; }
        }

        //needs to be accesible by ball for collision detection
        public int Height
        {
            get { return texture.Height; }
        }

        //returns the absolute position of the player(the top left point), needs to be accesible by ball for collision detection
        public Vector2 OriginAdjustedPosition
        {
            get { return position - origin; }
        }

        //returns the absolute position of the player(the top left point) from the last frame, needs to be accesible by ball for collision detection
        public Vector2 OriginAdjustedLastPosition
        {
            get { return lastPosition - origin; }
        }
    }
}
