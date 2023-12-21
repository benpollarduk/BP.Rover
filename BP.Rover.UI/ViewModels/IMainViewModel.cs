using System.Collections.Generic;
using System.Drawing;

namespace BP.Rover.UI.ViewModels
{
    public interface IMainViewModel
    {
        Map Map { get; }
        Rover Rover { get; }
        string MapName { get; }
        int MapWidth { get; }
        int MapHeight { get; }
        double MapPercentageLand { get; }
        double MapPercentageOfLandExplored { get; }
        TileType[,] MapTiles { get; }
        long MovesUsedToExploreMap { get; }
        Point RoverPosition { get; }
        int ExplorationTimeinMs { get; set; }
        bool IsExplorationIsProgress { get; }
        bool IsGeneratedMap { get; }
        List<Point> RoverTrail { get; }
        int MaximumVisitsToAnyTile { get; }
        void LoadMap(string path);
        void Explore(IRouter router);
        void Reset();
        void Cancel();
        void GenerateRandomMap();
        void GenerateRandomMap(int width, int height, double continuityBias);
    }
}
