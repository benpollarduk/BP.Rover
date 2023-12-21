using System.Collections.Generic;
using System.Drawing;

namespace BP.Rover
{
    /// <summary>
    /// Represents a structure for holding a state of an exploration.
    /// </summary>
    public struct ExplorationState
    {
        /// <summary>
        /// Get an array containing the positions being explored.
        /// </summary>
        public Point[] PositionsBeingExplored { get; }

        /// <summary>
        /// Get the current position of the rover.
        /// </summary>
        public Point CurrentPosition { get; }

        /// <summary>
        /// Get the moves taken during exploration so far.
        /// </summary>
        public long Moves { get; }

        /// <summary>
        /// Get the percentage explored so far.
        /// </summary>
        public double PercentageExplored { get; }
        
        /// <summary>
        /// Get the rovers trail.
        /// </summary>
        public List<Point> Trail { get; }

        /// <summary>
        /// Initializes a new instance of the ExplorationState struct.
        /// </summary>
        /// <param name="currentPosition">The current position of the rover.</param>
        /// <param name="positionsBeingExplored">An array containing the positions being explored.</param>
        /// <param name="moves">The moves taken during exploration so far.</param>
        /// <param name="percentageExplored">The percentage explored so far.</param>
        /// <param name="trail">The rovers trail.</param>
        public ExplorationState(Point currentPosition, Point[] positionsBeingExplored, long moves, double percentageExplored, List<Point> trail)
        {
            PositionsBeingExplored = positionsBeingExplored;
            CurrentPosition = currentPosition;
            Moves = moves;
            PercentageExplored = percentageExplored;
            Trail = trail;
        }
    }
}
