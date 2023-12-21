using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace BP.Rover.Routers
{
    /// <summary>
    /// Represents a class for exploring a map using a flood fill algorithm, using iteration.
    /// </summary>
    public class IterativeFloodFillRouter : Router
    {
        #region StaticMethods

        /// <summary>
        /// Determine if the rover has any unvisited adjacent areas.
        /// </summary>
        /// <param name="rover">The rover.</param>
        /// <param name="map">The map to check.</param>
        /// <param name="visitedLocations">The visited locations.</param>
        /// <returns>True if there is an adjacent unvisited area, else false.</returns>
        public static bool HasUnvisitedAdjacentArea(Rover rover, Map map, bool[,] visitedLocations)
        {
            return Map.Directions.Any(x => IsRelativeLocationIsVisitableAndUnvisited(rover, x, map, visitedLocations));
        }

        /// <summary>
        /// Determine if position relative to the rover to is able to be visited but hasn't yet been unvisited.
        /// </summary>
        /// <param name="rover">The rover.</param>
        /// <param name="direction">The direction to check.</param>
        /// <param name="map">The map to check.</param>
        /// <param name="visitedLocations">The visited locations.</param>
        /// <returns>True if the location is unvisited.</returns>
        public static bool IsRelativeLocationIsVisitableAndUnvisited(Rover rover, Direction direction, Map map, bool[,] visitedLocations)
        {
            Point position;

            // determine relative position
            switch (direction)
            {
                case Direction.North:
                    position = new Point(rover.Position.X, rover.Position.Y - 1);
                    break;
                case Direction.East:
                    position = new Point(rover.Position.X + 1, rover.Position.Y);
                    break;
                case Direction.South:
                    position = new Point(rover.Position.X, rover.Position.Y + 1);
                    break;
                case Direction.West:
                    position = new Point(rover.Position.X - 1, rover.Position.Y);
                    break;
                default:
                    throw new NotImplementedException();
            }

            // if the point is out of bounds then it cannot be visited anyway
            if (!map.IsInBounds(position.X, position.Y))
                return false;

            // return if the position is land and hasn't been visited
            return Map.IsLand(map[position.X, position.Y]) && (!visitedLocations[position.X, position.Y]);
        }

        #endregion
        
        #region Implementation of IRouter

        /// <summary>
        /// Explore a map with a rover.
        /// </summary>
        /// <param name="rover">The rover.</param>
        /// <param name="map">The map.</param>
        /// <returns>The number of moves used to explore the map.</returns>
        public override long ExploreMap(Rover rover, Map map)
        {
            /*  essentially this is a 'flood fill', but it also reverses the rover out from each move made so that it is executable with a single unit
                the flood fill works by:

                - if there is a move to an adjacent location that has not yet been visited
                    - move there
                    - stack the opposite to the move so that the rover can be reversed out when exploration of the area is complete
                - if there is no move to an adjacent location that has not yet been visited
                    - reverse the rover
                - exit the loop when there are no explorable areas and no reverse moves
                - mark that the current position has been checked
            */

            HasBeenCanceled = false;
            long moves = 0;
            var checkedLocations = new bool[map.Width, map.Height];
            var reverseMoveStack = new Stack<Direction>();
            var hasPossibleMove = true;

            // the surrounding area must be explored
            rover.ExploreSurroundingArea();

            // update status change if required
            if (DetermineIfUpdateShouldBeIssued())
                PublishUpdate(new ExplorationState(rover.Position, rover.LocationsBeingExplored.ToArray(), 0, map.PercentageOfLandExplored, rover.Trail));

            do
            {
                // this position is now checked
                checkedLocations[rover.Position.X, rover.Position.Y] = true;

                // determine if there are any available moves from the current location
                var anyAvailableMovesFromCurrentLocation = Map.Directions.Any(rover.CanMove) && HasUnvisitedAdjacentArea(rover, map, checkedLocations);

                // if available moves
                if (anyAvailableMovesFromCurrentLocation)
                {
                    // an area adjacent to the current position can be moved to

                    foreach (var direction in Map.Directions)
                    {
                        // check that the rover can move in the desired direction
                        if (!rover.CanMove(direction))
                            continue;

                        // check if the position has already been explored
                        if (!IsRelativeLocationIsVisitableAndUnvisited(rover, direction, map, checkedLocations))
                            continue;

                        rover.Move(direction);
                        moves++;

                        // stack up the reverse direction to allow for continued exploration
                        reverseMoveStack.Push(direction.Opposite());

                        break;
                    }
                }
                else
                {
                    // retrace steps until there is something to explore

                    // if there are reverse moves then exploration can continue
                    if (reverseMoveStack.Count != 0)
                    {
                        var direction = reverseMoveStack.Pop();
                        rover.Move(direction);
                        moves++;
                    }
                    else
                    {
                        hasPossibleMove = false;
                    }
                }

                // update status change if required
                if (DetermineIfUpdateShouldBeIssued())
                    PublishUpdate(new ExplorationState(rover.Position, rover.LocationsBeingExplored.ToArray(), moves, map.PercentageOfLandExplored, rover.Trail));

                // wait for the exploration time, if required
                if (DetermineIfDelayShouldBeInvoked())
                    Thread.Sleep(ExplorationTimeInMs);
            } 
            while (hasPossibleMove && !HasBeenCanceled);
            
            return moves;
        }

        #endregion
    }
}
