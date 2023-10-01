using Microsoft.Xna.Framework;

namespace pong
{
    public static class CollisionHelper
    {
        //The class collisionHelper is accesible to all classes as it contains handy methods for checking collisions and finding the intersection of lines, Ball uses it for collisions and Bot uses it for calculating its optimal position.

        public static Vector2? VerticalIntersection(Vector2 supportVector, Vector2 directionVector, float xCoordinate, float? minY, float? maxY, float? maxLambda)
        {
            //Check if directionVector.X is not zero(it would break the code and it would never collide with the Y axis anyways)
            if (directionVector.X == 0f)
                return null;

            //Vector representation of line x = sV.X + lambda * dV.X 
            //So: lambda = (x = sV.X) / sV.X
            float lambda = (xCoordinate - supportVector.X) / directionVector.X;

            //make sure to only check in the direction of the Vector(not in the opposite direction, and not on the exact support Vector) as well as make sure the intersection doesn't lie beyond the current position
            if (lambda <= 0)
                return null;
            else if (maxLambda.HasValue)
            {
                if (lambda > maxLambda)
                    return null;
            }

            // y = sV.Y + lambda * dV.Y
            float yCoordinate = (supportVector.Y + lambda * directionVector.Y);

            //check if ycoordinate is within the specified range and if a range is given
            if (minY.HasValue && maxY.HasValue)
            {
                if (minY <= yCoordinate && yCoordinate <= maxY)
                    return new Vector2(xCoordinate, yCoordinate);
                else
                    return null;
            }
            else
                return new Vector2(xCoordinate, yCoordinate);
        }
        public static Vector2? HorizontalIntersection(Vector2 supportVector, Vector2 directionVector, float yCoordinate, float? minX, float? maxX, float? maxLambda)
        {
            //Check if directionVector.Y is not zero(it would break the code and it would never collide with the X axis anyways)
            if (directionVector.Y == 0f)
                return null;

            //Vector representation of line y = sV.Y + lambda * dV.Y
            //So: lambda = (Y - sV.Y) / sV.Y
            float lambda = (yCoordinate - supportVector.Y) / directionVector.Y;

            //make sure to only check in the direction of the Vector(not in the opposite direction, and not on the exact support Vector) as well as make sure the intersection doesn't lie beyond the current position
            if (lambda <= 0)
                return null;
            else if(maxLambda.HasValue)
            {
                if(lambda > maxLambda) 
                    return null;
            }

            //x = sV.X + lambda * dV.X 
            float xCoordinate = (supportVector.X + lambda * directionVector.X);

            //check if ycoordinate is within the specified range and if a range is given
            if (minX.HasValue && maxX.HasValue)
            {
                if (minX <= xCoordinate && xCoordinate <= maxX)
                    return new Vector2(xCoordinate, yCoordinate);
                else
                    return null;
            }
            else
                return new Vector2(xCoordinate, yCoordinate);
        }
        public static Vector2? BoxIntersection(Vector2 supportVector, Vector2 directionVector, Vector2 topLeftBound, Vector2 bottomRightBound, float? maxLambda)
        {
            //cheeck all possible intersections(all four sides of the box)
            Vector2? possiblePos1 = VerticalIntersection(supportVector, directionVector, topLeftBound.X, topLeftBound.Y, bottomRightBound.Y, maxLambda);
            Vector2? possiblePos2 = VerticalIntersection(supportVector, directionVector, bottomRightBound.X, topLeftBound.Y, bottomRightBound.Y, maxLambda);
            Vector2? possiblePos3 = HorizontalIntersection(supportVector, directionVector, topLeftBound.Y, topLeftBound.X, bottomRightBound.X, maxLambda);
            Vector2? possiblePos4 = HorizontalIntersection(supportVector, directionVector, bottomRightBound.Y, topLeftBound.X, bottomRightBound.X, maxLambda);

            //create local Variables for setting the right collision position and telling if a collision has occurred  
            Vector2? collisionPosition = null;

            //instantiate floats with a large value assigned so that they will not be the smallest distance when left unassigned
            float distance1 = 100000f;
            float distance2 = 100000f;
            float distance3 = 100000f;

            //choose the collision that is closest to the last position
            if (possiblePos1.HasValue)
            {
                collisionPosition = possiblePos1.Value;
                distance1 = (possiblePos1.Value - supportVector).Length();
            }
            if (possiblePos2.HasValue)
            {
                distance2 = (possiblePos2.Value - supportVector).Length();
                if (distance2 < distance1)
                    collisionPosition = possiblePos2.Value;
            }
            if (possiblePos3.HasValue)
            {
                distance3 = (possiblePos3.Value - supportVector).Length();
                if (distance3 < distance2 && distance3 < distance1)
                    collisionPosition = possiblePos3.Value;
            }
            if (possiblePos4.HasValue)
            {
                float distance4 = (possiblePos4.Value - supportVector).Length();
                if (distance4 < distance3 && distance4 < distance2 && distance4 < distance1)
                    collisionPosition = possiblePos4.Value;
            }
            return collisionPosition;
        }
    }
}
