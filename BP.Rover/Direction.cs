using System;

namespace BP.Rover
{
    /// <summary>
    /// Enumeration of directions.
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// North.
        /// </summary>
        North = 0,
        /// <summary>
        /// East.
        /// </summary>
        East,
        /// <summary>
        /// South.
        /// </summary>
        South,
        /// <summary>
        /// West.
        /// </summary>
        West
    }

    /// <summary>
    /// Provides extension methods for Direction.
    /// </summary>
    public static class DirectionExtensions
    {
        /// <summary>
        /// Get an opposite direction.
        /// </summary>
        /// <param name="direction">The direction to get the opposite of.</param>
        /// <returns>The opposite direction.</returns>
        public static Direction Opposite(this Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return Direction.South;
                case Direction.East:
                    return Direction.West;
                case Direction.South:
                    return Direction.North;
                case Direction.West:
                    return Direction.East;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
