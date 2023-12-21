using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using BP.Rover.UI.Annotations;

namespace BP.Rover.UI.ViewModels
{
    public class MainViewModel : IMainViewModel, INotifyPropertyChanged
    {
        #region Fields

        private Map map;
        private Rover rover;
        private string lastImportedPath;
        private string mapName = "Unknown";
        private int mapWidth;
        private int mapHeight;
        private double mapPercentageLand;
        private double mapPercentageOfLandExplored;
        private long movesUsedToExploreMap;
        private TileType[,] mapTiles;
        private Point roverPosition;
        private int explorationTimeinMs = 10;
        private bool isExplorationIsProgress;
        private IRouter currentRouter;
        private Point[] previewExplorationPoints;
        private bool isGeneratedMap;
        private readonly Random random = new Random();
        private List<Point> roverTrail;
        private int maximumVisitsToAnyTile;

        #endregion

        #region Constructors

        public MainViewModel()
        {
            GenerateRandomMap();
        }

        #endregion

        #region Methods

        private void CurrentRouter_ExplorationStateChanged(object sender, ExplorationState e)
        {
            RoverPosition = e.CurrentPosition;
            RoverTrail = Rover.Trail;
            MapPercentageOfLandExplored = e.PercentageExplored;
            MovesUsedToExploreMap = e.Moves;

            foreach (var point in previewExplorationPoints ?? new Point[0])
                MapTiles[point.X, point.Y] = Map.Tiles[point.X, point.Y];
            
            foreach (var point in e.PositionsBeingExplored ?? new Point[0])
                MapTiles[point.X, point.Y] = TileType.LandBeingExplored;

            previewExplorationPoints = e.PositionsBeingExplored;
        }

        #endregion

        #region Implementation of IMainViewModel

        public Map Map
        {
            get { return map; }
            protected set
            {
                map = value;

                MapName = value?.Name ?? string.Empty;
                MapWidth = value?.Width ?? 0;
                MapHeight = value?.Height ?? 0;
                MapPercentageLand = value?.PercentageLand ?? 0;
                MapPercentageOfLandExplored = value?.PercentageOfLandExplored ?? 0;
                MapTiles = value?.Tiles.Clone() as TileType[,] ?? new TileType[0, 0];
                MovesUsedToExploreMap = 0;

                OnPropertyChanged();
            }
        }

        public Rover Rover
        {
            get { return rover; }
            protected set
            {
                rover = value;

                RoverPosition = value?.Position ?? new Point(0, 0);
                RoverTrail = value?.Trail ?? new List<Point>();

                OnPropertyChanged();
            }
        }

        public string MapName
        {
            get { return mapName; }
            protected set
            {
                mapName = value; 
                OnPropertyChanged();
            }
        }

        public int MapWidth
        {
            get { return mapWidth; }
            protected set
            {
                mapWidth = value;
                OnPropertyChanged();
            }
        }

        public int MapHeight
        {
            get { return mapHeight; }
            protected set
            {
                mapHeight = value;
                OnPropertyChanged();
            }
        }

        public double MapPercentageLand
        {
            get { return mapPercentageLand; }
            protected set
            {
                mapPercentageLand = value;
                OnPropertyChanged();
            }
        }

        public double MapPercentageOfLandExplored
        {
            get { return mapPercentageOfLandExplored; }
            protected set
            {
                mapPercentageOfLandExplored = value;
                OnPropertyChanged();
            }
        }

        public TileType[,] MapTiles
        {
            get { return mapTiles; }
            protected set
            {
                mapTiles = value; 
                OnPropertyChanged();
            }
        }

        public long MovesUsedToExploreMap
        {
            get { return movesUsedToExploreMap; }
            protected set
            {
                movesUsedToExploreMap = value;
                OnPropertyChanged();
            }
        }

        public Point RoverPosition
        {
            get { return roverPosition; }
            protected set
            {
                roverPosition = value;
                OnPropertyChanged();
            }
        }

        public int ExplorationTimeinMs
        {
            get { return explorationTimeinMs; }
            set
            {
                explorationTimeinMs = value; 
                OnPropertyChanged();
            }
        }

        public bool IsExplorationIsProgress
        {
            get { return isExplorationIsProgress; }
            protected set
            {
                isExplorationIsProgress = value;
                OnPropertyChanged();
            }
        }

        public bool IsGeneratedMap
        {
            get { return isGeneratedMap; }
            protected set
            {
                isGeneratedMap = value; 
                OnPropertyChanged();
            }
        }

        public List<Point> RoverTrail
        {
            get { return roverTrail; }
            protected set
            {
                roverTrail = value;

                if (((value != null)) && (value.Any()))
                {
                    MaximumVisitsToAnyTile = value.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count()).Values.Max();
                }
                else
                {
                    MaximumVisitsToAnyTile = 0;
                }
                
                OnPropertyChanged();
            }
        }

        public int MaximumVisitsToAnyTile
        {
            get { return maximumVisitsToAnyTile; }
            protected set
            {
                maximumVisitsToAnyTile = value;
                OnPropertyChanged();
            }
        }

        public void LoadMap(string path)
        {
            IsGeneratedMap = false;
            RoverTrail = null;

            var newMap = new Map();
            newMap.Deserialize(path);

            Map = newMap;

            lastImportedPath = path;

            Rover = new Rover(Map);
        }

        public void Explore(IRouter router)
        {
            if (IsExplorationIsProgress)
                router.Cancel();

            RoverTrail?.Clear();

            ThreadPool.QueueUserWorkItem(x =>
            {
                try
                {
                    currentRouter = router;
                    currentRouter.ExplorationTimeInMs = ExplorationTimeinMs;
                    currentRouter.ExplorationStateChanged += CurrentRouter_ExplorationStateChanged;

                    IsExplorationIsProgress = true;

                    MovesUsedToExploreMap = router.ExploreMap(Rover, Map);

                    MapPercentageOfLandExplored = Map.PercentageOfLandExplored;
                    MapTiles = Map.Tiles;
                    RoverPosition = Rover.Position;
                    RoverTrail = Rover.Trail;
                }
                finally
                {
                    if (currentRouter != null)
                    {
                        currentRouter.ExplorationStateChanged -= CurrentRouter_ExplorationStateChanged;
                        currentRouter = null;
                    }

                    IsExplorationIsProgress = false;
                }
            });
        }

        public void Reset()
        {
            if (IsGeneratedMap)
                LoadMap(lastImportedPath);
        }

        public void Cancel()
        {
            if (IsExplorationIsProgress)
                currentRouter?.Cancel();
        }

        public void GenerateRandomMap()
        {
            var width = random.Next(20, 60);
            var height = (int)Math.Floor(width * 0.75);
            var continuityBias = random.Next(97, 100);
            GenerateRandomMap(width, height, continuityBias / 100d);
        }

        public void GenerateRandomMap(int width, int height, double continuityBias)
        {
            IsGeneratedMap = true;
            RoverTrail = null;

            Map = Map.Generate("Generated", width, height, continuityBias);
            Rover = new Rover(Map);
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
