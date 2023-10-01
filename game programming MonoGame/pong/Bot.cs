using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace pong
{
    class Bot : Player
    {
        //The class Bot inherits from player as its a type of player, instead of handling input it calculates an optimal position and moves towards it.
        //On high diffuclty the bots perfectly calculate this position and choose a random side to always hit the ball with a corner, on regular difficulty a random value is added to the optimal position so they sometimes miss.

        Vector2 optimalPosition;
        Vector2 lastBallDirection;
        bool needsVelocityRecalculation;
        float lineLeft, lineRight, lineTop, lineBottom;
        public Bot(Vector2 _startPosition, string _paddleTex, Keys _keyUp, Keys _keyDown, int _playerId, bool _isVertical, ContentManager _content, Pong _pong) : base(_startPosition, _paddleTex, _keyUp, _keyDown, _playerId, _isVertical, _content, _pong) 
        {
            //set initial variables
            ReCalculateVariables();
        }

        //calcultate the desired direction and move towards it if the player/bot is alive/active. Also save the direction of the ball in the last frame for later comparisons
        public override void Update(GameTime gameTime)
        {
            if (IsAlive)
            {
                CalculateVelocity(gameTime);
                base.Update(gameTime);
                lastBallDirection = pong.Ball.Position - pong.Ball.LastPosition;
            }
        }

        //not needed for a bot, by overiding and then leaving the body empty the method is effectively disabled
        protected override void HandleInput() { }

        //reset the position, lastPosition and set the optimal position to this reset position
        public override void Reset()
        {
            base.Reset();
            optimalPosition = position;
        }

        //Calculate the right velocity by determining the optimal position and capping the length of the vector
        public void CalculateVelocity(GameTime gameTime)
        {
            //only update the optimal position if: the ball just started moving, the ball is swerving or the bool needsVelocityRecalculation has been set to true(done by ball)
            bool ballJustStartedMoving = lastBallDirection == Vector2.Zero && (pong.Ball.Position != pong.Ball.LastPosition);
            bool ballIsSwerving = pong.PowerUps.ActivePowerUp == PowerUps.PowerUp.swerve;
            if (needsVelocityRecalculation || ballIsSwerving || ballJustStartedMoving)
            {
                //calculate the actual optimalposition
                optimalPosition = CalculateOptimalPosition();

                //add values to this actual position based on difficulty to either miss sometimes(regular diff) or always try to hit the ball on the corner(hard diff)
                if (pong.IsExtremeDifficulty)
                {
                    //only does these extras if the ball is not swerving(power up), as they could cause the bot to miss in the case of swerving
                    if (!ballIsSwerving)
                    {
                        //50/50 chance to choose either side
                        int coinFlip = Pong.Random.Next(0, 2);
                        if (IsVertical)
                        {
                            if (coinFlip == 0)
                                optimalPosition.Y += origin.Y * 0.9f;
                            else
                                optimalPosition.Y -= origin.Y * 0.9f;
                        }
                        else
                        {
                            if (coinFlip == 0)
                                optimalPosition.X += origin.X * 0.9f;
                            else
                                optimalPosition.X -= origin.X * 0.9f;
                        }
                    }
                }
                else
                {
                    float randomNegativeIncludedFloat = Pong.Random.NextSingle() * 2f - 1;
                    if (IsVertical)
                        optimalPosition.Y += randomNegativeIncludedFloat * 1.15f * origin.Y + pong.Ball.Origin.X;
                    else
                        optimalPosition.X += randomNegativeIncludedFloat * 1.15f * origin.X + pong.Ball.Origin.X;
                }
                //makes sure the paddles only move along the right axis as the lines on which the optimal position is calculated are slightly offset
                switch (playerId)
                {
                    case 0:
                        optimalPosition.X -= Width + pong.Ball.Origin.X - origin.X;
                        break;
                    case 1:
                        optimalPosition.X += Width + pong.Ball.Origin.X - origin.X;
                        break;
                    case 2:
                        optimalPosition.Y -= Height + pong.Ball.Origin.Y - origin.Y;
                        break;
                    case 3:
                        optimalPosition.Y += Height + pong.Ball.Origin.Y - origin.Y;
                        break;
                }
            }
            //determines the direction of the velocity
            Vector2 moveDirection = (optimalPosition - position);

            //checks if the desired movement distance is more than the max it is allowed to travel, if not, the velocity is based on the delta time to avoid overshoot
            if (moveDirection.Length() <= speed * (float)gameTime.ElapsedGameTime.TotalSeconds)
                velocity = moveDirection / (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
            {
                //sets the velocity to the calculated direction with the right speed
                moveDirection.Normalize();
                velocity = moveDirection * speed;
            }
        }
        Vector2 CalculateOptimalPosition()
        {
            //saves the balls current position and direction so its position only has to be accesed once
            Vector2 ballPosition = pong.Ball.LastPosition;
            Vector2 direction = (pong.Ball.Position - ballPosition);

            //initialises helper variables
            Vector2 optimalPosition = Vector2.Zero;
            Vector2? collisionPosition;

            //calculates the optimal position by simulating the ball bouncing around based on intersection with the sides
            //If the optimal position has not been found after 100 iterations the ball's current position is used to avoid unnecessary calculations
            for (int i = 0; i < 100; i++)
            {
                //when the ball is not moving, only its position is taken into account
                if (direction == Vector2.Zero)
                {
                    needsVelocityRecalculation = false;
                    switch (playerId)
                    {
                        case 0:
                            return new Vector2(lineLeft, ballPosition.Y);
                        case 1:
                            return new Vector2(lineRight, ballPosition.Y);
                        case 2:
                            return new Vector2(ballPosition.X, lineTop);
                        case 3:
                            return new Vector2(ballPosition.X, lineBottom);
                        default:
                            Debug.WriteLine("playerId out of range");
                            return Vector2.Zero;
                    }
                }
                collisionPosition = CollisionHelper.VerticalIntersection(ballPosition, direction, lineLeft, lineTop, lineBottom, null);
                if (collisionPosition.HasValue)
                {
                    if (playerId == 0)
                    {
                        //if the side corresponds to the side of the instance of Bot, save the optimal position
                        optimalPosition = collisionPosition.Value;
                        needsVelocityRecalculation =  false;
                        break;
                    }
                    else
                    {
                        //otherwise virtually bounce and keep calculating
                        direction.X = -direction.X;
                        ballPosition = collisionPosition.Value;
                    }         
                }
                collisionPosition = CollisionHelper.VerticalIntersection(ballPosition, direction, lineRight, lineTop, lineBottom, null);
                if (collisionPosition.HasValue)
                {
                    if (playerId == 1)
                    {
                        //if the side corresponds to the side of the instance of Bot, save the optimal position
                        optimalPosition = collisionPosition.Value;
                        needsVelocityRecalculation = false;
                        break;
                    }
                    else
                    {
                        //otherwise virtually bounce and keep calculating
                        direction.X = -direction.X;
                        ballPosition = collisionPosition.Value;
                    }
                }
                collisionPosition = CollisionHelper.HorizontalIntersection(ballPosition, direction, lineTop, lineLeft, lineRight, null);
                if (collisionPosition.HasValue)
                {
                    if (playerId == 2)
                    {
                        //if the side corresponds to the side of the instance of Bot, save the optimal position
                        optimalPosition = collisionPosition.Value;
                        needsVelocityRecalculation = false;
                        break;
                    }
                    else
                    {
                        //otherwise virtually bounce and keep calculating
                        direction.Y = -direction.Y;
                        ballPosition = collisionPosition.Value;
                    }
                }
                collisionPosition = CollisionHelper.HorizontalIntersection(ballPosition, direction, lineBottom, lineLeft, lineRight, null);
                if (collisionPosition.HasValue)
                {
                    if (playerId == 3)
                    {
                        //if the side corresponds to the side of the instance of Bot, save the optimal position
                        optimalPosition = collisionPosition.Value;
                        needsVelocityRecalculation = false;
                        break;
                    }
                    else
                    {
                        //otherwise virtually bounce and keep calculating
                        direction.Y = -direction.Y;
                        ballPosition = collisionPosition.Value;            
                    }
                }
                
                //this switch is so that the optimal position will be following the ball if the loop hasn't found anything after 100 iterations
                switch (playerId)
                {
                    case 0:
                        optimalPosition = new Vector2(lineLeft, ballPosition.Y);
                        break;
                    case 1:
                        optimalPosition = new Vector2(lineRight, ballPosition.Y);
                        break;
                    case 2:
                        optimalPosition = new Vector2(ballPosition.X, lineTop);
                        break;
                    case 3:
                        optimalPosition = new Vector2(ballPosition.X, lineBottom);
                        break;
                    default:
                        Debug.WriteLine("playerId out of range");
                        optimalPosition = Vector2.Zero;
                        break;
                }
            }
            return optimalPosition;
        }

        //recalculate all variables used by the optimal position finder and base a new optimal position around these recalculated variables. Usually called after texture and origin change
        public override void ReCalculateVariables()
        {
            base.ReCalculateVariables();

            //set the lines used to calculate the optimal position to the right values
            lineLeft = pong.Ball.Origin.X;
            lineRight = Pong.screenSize.X - pong.Ball.Origin.X;
            lineTop = pong.Ball.Origin.Y;
            lineBottom = Pong.screenSize.Y - pong.Ball.Origin.Y;

            //based on the side of the instance of the class an extra value needs to be added to make up for the fact that the paddle hits the ball at a different line than its position
            switch (playerId)
            {
                case 0:
                    lineLeft += Width;
                    break;
                case 1:
                    lineRight -= Width;
                    break;
                case 2:
                    lineTop += Height;
                    break;
                case 3:
                    lineBottom -= Height;
                    break;
                default:
                    Debug.WriteLine("playerId out of range");
                    break;  
            }

            //recalculate optimal position with new values
            optimalPosition = CalculateOptimalPosition();
            switch (playerId)
            {
                case 0:
                    optimalPosition.X -= Width + pong.Ball.Origin.X - origin.X;
                    break;
                case 1:
                    optimalPosition.X += Width + pong.Ball.Origin.X - origin.X;
                    break;
                case 2:
                    optimalPosition.Y -= Height + pong.Ball.Origin.Y - origin.Y;
                    break;
                case 3:
                    optimalPosition.Y += Height + pong.Ball.Origin.Y - origin.Y;
                    break;
            }
            //change values to make up for slight offset
        }

        //NeedsVelocityRecalculation needs to be accesible by ball to update it after ball has bounced
        public bool NeedsVelocityRecalculation
        {
            set { needsVelocityRecalculation = value; }
        }
    }
}
