namespace BP.Rover
{
    /// <summary>
    /// Enumeration of types of tiles.
    /// </summary>
    public enum TileType
    {
        /// <summary>
        /// Unknown.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// Sea.
        /// </summary>
        Sea,
        /// <summary>
        /// Unexplored land.
        /// </summary>
        UnexploredLand,
        /// <summary>
        /// Explored land.
        /// </summary>
        ExploredLand,
        /// <summary>
        /// Land currently being explored.
        /// </summary>
        LandBeingExplored
    }
}
