using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace pong
{
    class Bot : Player
    {
        float lineLeft, lineRight, lineTop, lineBottom;
        bool isExtremeDifficulty;
        public Bot(Vector2 _startPosition, string _paddleTex, Keys _keyUp, Keys _keyDown, int _playerId, bool _isVertical, ContentManager _content, Pong _pong, bool _isExtremeDifficulty) : base(_startPosition, _paddleTex, _keyUp, _keyDown, _playerId, _isVertical, _content, _pong) 
        {
            isExtremeDifficulty = _isExtremeDifficulty;
            ReCalculateVariables();
        }

        public override void Update(GameTime gameTime)
        {
            if (IsAlive)
            {
                CalculateVelocity(gameTime);
                base.Update(gameTime);
            }
        }
        protected override void HandleInput()
        {
            //not needed for a bot
        }

        public void CalculateVelocity(GameTime gameTime)
        {
            Vector2 optimalPosition = CalculateOptimalPosition();
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
                    }
                }
                collisionPosition = CollisionHelper.VerticalIntersection(ballPosition, direction, lineLeft, lineTop, lineBottom, null);
                if (collisionPosition.HasValue)
                {
                    if (playerId == 0)
                    {
                        optimalPosition = collisionPosition.Value;
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
            }
        }
    }
}
