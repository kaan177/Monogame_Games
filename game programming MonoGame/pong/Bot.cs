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
            lineLeft = pong.Ball.Origin.X;
            lineRight = Pong.screenSize.X - pong.Ball.Origin.X;
            lineTop = pong.Ball.Origin.Y;
            lineBottom = Pong.screenSize.Y - pong.Ball.Origin.Y;
        }
        
        protected override void HandleInput()
        {
            //I chose to repurpose the handle input method for moving the bot, as for the bot the optimal position is a form of input
            Vector2 optimalPosition = CalculateOptimalPosition();
            Vector2 moveDirection = (optimalPosition - position);
            if (moveDirection != Vector2.Zero)
            {
                moveDirection.Normalize();
                velocity = moveDirection * speed;
            }
            else
            {
                velocity = Vector2.Zero;
            }
        }

        Vector2 CalculateOptimalPosition()
        {
            Vector2 ballPosition = pong.Ball.LastPosition;
            Vector2 optimalPosition;
            Vector2? collisionPosition;
            Vector2 direction = (pong.Ball.Position - ballPosition);
            for (; ;)
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
            }
            return optimalPosition;
        }
        public override void ReCalculateAfterScreenChange()
        {
            base.ReCalculateAfterScreenChange();
            lineLeft = pong.Ball.Origin.X;
            lineRight = Pong.screenSize.X - pong.Ball.Origin.X;
            lineTop = pong.Ball.Origin.Y;
            lineBottom = Pong.screenSize.Y - pong.Ball.Origin.Y;
        }
    }
}
