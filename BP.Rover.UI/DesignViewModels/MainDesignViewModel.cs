using System;
using System.Collections.Generic;
using System.Drawing;
using BP.Rover.UI.ViewModels;

namespace BP.Rover.UI.DesignViewModels
{
    public class MainDesignViewModel : IMainViewModel
    {
        #region Implementation of IMainViewModel

        public Map Map { get; } = null;
        public Rover Rover { get; } = null;
        public string MapName { get; } = "Map";
        public int MapWidth { get; } = 5;
        public int MapHeight { get; } = 5;
        public double MapPercentageLand { get; } = 50;
        public double MapPercentageOfLandExplored { get; } = 25;
        public TileType[,] MapTiles { get; } = new TileType[5,5];
        public long MovesUsedToExploreMap { get; } = 0;
        public Point RoverPosition { get; } = new Point(0,0);
        public int ExplorationTimeinMs { get; set; } = 10;
        public bool IsExplorationIsProgress { get; } = false;
        public bool IsGeneratedMap { get; } = false;
        public List<Point> RoverTrail { get; } = new List<Point>();
        public int MaximumVisitsToAnyTile { get; } = 5;

        public void LoadMap(string path)
        {
            throw new NotImplementedException();
        }

        public void Explore(IRouter router)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void GenerateRandomMap()
        {
            throw new NotImplementedException();
        }

        public void GenerateRandomMap(int width, int height, double continuityBias)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
