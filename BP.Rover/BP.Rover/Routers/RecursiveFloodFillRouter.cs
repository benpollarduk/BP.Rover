using System.Threading;

namespace BP.Rover.Routers
{
    /// <summary>
    /// Represents a class for exploring a map using a flood fill algorithm, using recursion.
    /// </summary>
    public class RecursiveFloodFillRouter : Router
    {
        #region Methods

        /// <summary>
        /// Explore the map using the rover.
        /// </summary>
        /// <param name="rover">The rover.</param>
        /// <param name="map">The map.</param>
        /// <param name="checkedLocations">A 2D array of booleans that map the positions already explored by the rover.</param>
        /// <param name="moves">The number of moves used to explore the map.</param>
        private void RecursiveExplore(Rover rover, Map map, bool[,] checkedLocations, ref long moves)
        {
            /*  essentially this is a 'flood fill', but it also reverses the rover out from each move made so that it is executable with a single unit
                the flood fill works by:

                - exit recursion if all land has been explored
                - exit recursion if the current position has already been checked
                - mark that the current position has been checked
                - iterating all directions
                    - if the rover can't move in that direction then skip
                    - if the rover can move in that direction then move it now
                    - explore the new area (recursive)
                    - reverse the rover out of the move
                */

            if (HasBeenCanceled)
                return;

            if (!map.HasRemainingUnexploredLand)
                return;

            if (checkedLocations[rover.Position.X, rover.Position.Y])
                return;

            checkedLocations[rover.Position.X, rover.Position.Y] = true;

            foreach (var direction in Map.Directions)
            {
                if (!rover.CanMove(direction)) 
                    continue;

                rover.Move(direction);
                moves++;
                
                if (DetermineIfUpdateShouldBeIssued())
                    PublishUpdate(new ExplorationState(rover.Position, rover.LocationsBeingExplored.ToArray(), moves, map.PercentageOfLandExplored, rover.Trail));

                // wait for the exploration time, if required
                if (DetermineIfDelayShouldBeInvoked())
                    Thread.Sleep(ExplorationTimeInMs);

                // recursively check this position
                RecursiveExplore(rover, map, checkedLocations, ref moves);

                // move the rover in the reverse direction to allow for continued exploration
                rover.Move(direction.Opposite());

                moves++;

                if (DetermineIfUpdateShouldBeIssued())
                    PublishUpdate(new ExplorationState(rover.Position, null, moves, map.PercentageOfLandExplored, rover.Trail));
            }
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
            HasBeenCanceled = false;
            long moves = 0;

            rover.ExploreSurroundingArea();

            if (DetermineIfUpdateShouldBeIssued())
                PublishUpdate(new ExplorationState(rover.Position, rover.LocationsBeingExplored.ToArray(), 0, map.PercentageOfLandExplored, rover.Trail));

            RecursiveExplore(rover, map, new bool[map.Width, map.Height], ref moves);

            return moves;
        }

        #endregion
    }
}
