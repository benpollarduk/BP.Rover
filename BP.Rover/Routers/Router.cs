using System;
using System.Threading;

namespace BP.Rover.Routers
{
    /// <summary>
    /// Represents a base class for routers.
    /// </summary>
    public abstract class Router : IRouter
    {
        #region Methods

        /// <summary>
        /// Publish the IRouter.ExplorationStateChanged event.
        /// </summary>
        /// <param name="update">The update.</param>
        protected void PublishUpdate(ExplorationState update)
        {
            ExplorationStateChanged?.Invoke(this, update);
        }

        /// <summary>
        /// Determine if updates should be issues.
        /// </summary>
        /// <returns>True if the update should be issued, else false.</returns>
        protected bool DetermineIfUpdateShouldBeIssued()
        {
            // only invoke updates when some delay time and the exploration has not been canceled
            return (ExplorationTimeInMs > 0) && !HasBeenCanceled;
        }

        /// <summary>
        /// Determine if a delay should be invoked should be issues.
        /// </summary>
        /// <returns>True if the update should be issued, else false.</returns>
        protected bool DetermineIfDelayShouldBeInvoked()
        {
            // only invoke delays when some delay time, the exploration has not been canceled and is running on a background thread
            return (ExplorationTimeInMs > 0) && !HasBeenCanceled && Thread.CurrentThread.IsBackground;
        }

        #endregion

        #region Implementation of IRouter

        /// <summary>
        /// Get if exploration has been canceled.
        /// </summary>
        public virtual bool HasBeenCanceled { get; protected set; }

        /// <summary>
        /// Get or set how long exploration takes, in ms.
        /// </summary>
        public virtual int ExplorationTimeInMs { get; set; }

        /// <summary>
        /// Occurs when the exploration state changes.
        /// </summary>
        public virtual event EventHandler<ExplorationState> ExplorationStateChanged;

        /// <summary>
        /// Explore a map with a rover.
        /// </summary>
        /// <param name="rover">The rover.</param>
        /// <param name="map">The map.</param>
        /// <returns>The number of moves used to explore the map.</returns>
        public virtual long ExploreMap(Rover rover, Map map)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cancel exploration.
        /// </summary>
        public virtual void Cancel()
        {
            HasBeenCanceled = true;
        }

        #endregion
    }
}
