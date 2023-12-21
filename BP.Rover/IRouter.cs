using System;

namespace BP.Rover
{
    /// <summary>
    /// Represents any object that can plot a route for a Rover to explore a Map.
    /// </summary>
    public interface IRouter
    {
        /// <summary>
        /// Get if exploration has been canceled.
        /// </summary>
        bool HasBeenCanceled { get; }

        /// <summary>
        /// Get or set how long exploration takes, in ms.
        /// </summary>
        int ExplorationTimeInMs { get; set; }

        /// <summary>
        /// Occurs when the exploration state changes.
        /// </summary>
        event EventHandler<ExplorationState> ExplorationStateChanged;

        /// <summary>
        /// Explore a map with a rover.
        /// </summary>
        /// <param name="rover">The rover.</param>
        /// <param name="map">The map.</param>
        /// <returns>The number of moves used to explore the map.</returns>
        long ExploreMap(Rover rover, Map map);

        /// <summary>
        /// Cancel exploration.
        /// </summary>
        void Cancel();
    }
}
