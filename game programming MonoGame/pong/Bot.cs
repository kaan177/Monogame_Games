using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace pong
{
    class Bot : Player
    {
        Vector2 optimalPosition;
        Vector2 lastBallDirection;
        bool needsVelocityRecalculation;
        float lineLeft, lineRight, lineTop, lineBottom;
        public Bot(Vector2 _startPosition, string _paddleTex, Keys _keyUp, Keys _keyDown, int _playerId, bool _isVertical, ContentManager _content, Pong _pong) : base(_startPosition, _paddleTex, _keyUp, _keyDown, _playerId, _isVertical, _content, _pong) 
        {
            ReCalculateVariables();
        }

        public override void Update(GameTime gameTime)
        {
            if (IsAlive)
            {
                CalculateVelocity(gameTime);
                base.Update(gameTime);
                lastBallDirection = pong.Ball.Position - pong.Ball.LastPosition;
            }
        }
        protected override void HandleInput()
        {
            //not needed for a bot
        }
        public override void Reset()
        {
            base.Reset();
            optimalPosition = position;
        }

        public void CalculateVelocity(GameTime gameTime)
        {
            bool ballJustStartedMoving = lastBallDirection == Vector2.Zero && (pong.Ball.Position != pong.Ball.LastPosition);
            bool ballIsSwerving = pong.PowerUps.ActivePowerUp == PowerUps.PowerUp.swerve;
            if (needsVelocityRecalculation || ballIsSwerving || ballJustStartedMoving)
            {
                optimalPosition = CalculateOptimalPosition();
                if (pong.IsExtremeDifficulty)
                {
                    if (!ballIsSwerving)
                    {
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
            Vector2 moveDirection = (optimalPosition - position);
            if (moveDirection.Length() <= speed * (float)gameTime.ElapsedGameTime.TotalSeconds)
                velocity = moveDirection / (float)gameTime.ElapsedGameTime.TotalSeconds;
            else
            {
                moveDirection.Normalize();
                velocity = moveDirection * speed;
            }
        }
        Vector2 CalculateOptimalPosition()
        {
            Vector2 ballPosition = pong.Ball.LastPosition;
            Vector2 optimalPosition = Vector2.Zero;
            Vector2? collisionPosition;
            Vector2 direction = (pong.Ball.Position - ballPosition);
            for (int i = 0; i < 100; i++)
            {
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
                        optimalPosition = collisionPosition.Value;
                        needsVelocityRecalculation =  false;
                        break;
                    }
                    else
                    {
                        direction.X = -direction.X;
                        ballPosition = collisionPosition.Value;
                    }         
                }
                collisionPosition = CollisionHelper.VerticalIntersection(ballPosition, direction, lineRight, lineTop, lineBottom, null);
                if (collisionPosition.HasValue)
                {
                    if (playerId == 1)
                    {
                        optimalPosition = collisionPosition.Value;
                        needsVelocityRecalculation = false;
                        break;
                    }
                    else
                    {
                        direction.X = -direction.X;
                        ballPosition = collisionPosition.Value;
                    }
                }
                collisionPosition = CollisionHelper.HorizontalIntersection(ballPosition, direction, lineTop, lineLeft, lineRight, null);
                if (collisionPosition.HasValue)
                {
                    if (playerId == 2)
                    {
                        optimalPosition = collisionPosition.Value;
                        needsVelocityRecalculation = false;
                        break;
                    }
                    else
                    {
                        direction.Y = -direction.Y;
                        ballPosition = collisionPosition.Value;
                    }
                }
                collisionPosition = CollisionHelper.HorizontalIntersection(ballPosition, direction, lineBottom, lineLeft, lineRight, null);
                if (collisionPosition.HasValue)
                {
                    if (playerId == 3)
                    {
                        optimalPosition = collisionPosition.Value;
                        needsVelocityRecalculation = false;
                        break;
                    }
                    else
                    {
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
        public override void ReCalculateVariables()
        {
            base.ReCalculateVariables();
            lineLeft = pong.Ball.Origin.X;
            lineRight = Pong.screenSize.X - pong.Ball.Origin.X;
            lineTop = pong.Ball.Origin.Y;
            lineBottom = Pong.screenSize.Y - pong.Ball.Origin.Y;
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
        }
        public bool NeedsVelocityRecalculation
        {
            set { needsVelocityRecalculation = value; }
        }
    }
}
