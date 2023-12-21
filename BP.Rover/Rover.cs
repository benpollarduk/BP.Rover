using System;
using System.Collections.Generic;
using System.Drawing;

namespace BP.Rover
{
    /// <summary>
    /// Represents the Rover.
    /// </summary>
    public class Rover
    {
        #region Fields

        private Map map;

        #endregion

        #region Properties

        /// <summary>
        /// Get the position of the rover.
        /// </summary>
        public Point Position { get; protected set; }

        /// <summary>
        /// Get the map this rover is exploring.
        /// </summary>
        protected Map Map
        {
            get { return map; }
            private set
            {
                map = value;

                Position = map.LandingLocation;

                Trail = new List<Point> { Position };
            }
        }

        /// <summary>
        /// Get the locations currently being explored.
        /// </summary>
        public List<Point> LocationsBeingExplored { get; private set; } = new List<Point>();

        /// <summary>
        /// Get the rovers trail.
        /// </summary>
        public List<Point> Trail { get; private set; } = new List<Point>();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the Rover class.
        /// </summary>
        /// <param name="map">The map that the rover will explore.</param>
        public Rover(Map map)
        {
            Map = map;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get if the rover can move to a tile.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <returns>True if the rover can move, else false.</returns>
        protected bool CanMove(TileType tile)
        {
            return tile == TileType.ExploredLand;
        }

        /// <summary>
        /// Get the next location in a specified direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns>The next location.</returns>
        public Point GetNextLocation(Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return new Point(Position.X, Position.Y - 1);
                case Direction.East:
                    return new Point(Position.X + 1, Position.Y);
                case Direction.South:
                    return new Point(Position.X, Position.Y + 1);
                case Direction.West:
                    return new Point(Position.X - 1, Position.Y);
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Get if the rover can move in a specified direction.
        /// </summary>
        /// <param name="direction">The direction to check.</param>
        /// <returns>True if the rover can move, else false.</returns>
        public bool CanMove(Direction direction)
        {
            var next = GetNextLocation(direction);
            return map.IsInBounds(next.X, next.Y) && CanMove(Map[next.X, next.Y]);
        }

        /// <summary>
        /// Move the rover.
        /// </summary>
        /// <param name="direction">The direction to move.</param>
        public void Move(Direction direction)
        {
            if (!CanMove(direction))
                throw new InvalidOperationException($"The rover cannot move {direction}.");

            Position = GetNextLocation(direction);
            Trail.Add(Position);
            ExploreSurroundingArea();
        }

        /// <summary>
        /// Explore the surrounding area.
        /// </summary>
        public void ExploreSurroundingArea()
        {
            // clear previous locations being explored
            LocationsBeingExplored?.Clear();

            // iterate directions
            foreach (var direction in Map.Directions)
            {
                var next = GetNextLocation(direction);

                if ((!Map.IsInBounds(next.X, next.Y)) || (Map[next.X, next.Y] != TileType.UnexploredLand))
                    continue;
                
                Map.MarkTileAsExplored(next.X, next.Y);
                LocationsBeingExplored.Add(new Point(next.X, next.Y));
            }
        }
        
        #endregion
    }
}
