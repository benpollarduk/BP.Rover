using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace BP.Rover
{
    /// <summary>
    /// Represents a map.
    /// </summary>
    public class Map
    {
        #region Constants

        /// <summary>
        /// Get the file extension.
        /// </summary>
        public const string FileExtension = ".map";

        /// <summary>
        /// Get the prefix for denoting that a map has been explored.
        /// </summary>
        public  const string ExploredMapPrefix = "explored-";

        #endregion

        #region StaticProperties

        /// <summary>
        /// Get a dictionary containing the characters to use for serializing and deserializing map data.
        /// </summary>
        protected static readonly Dictionary<TileType, char> FileHandlingTileCharacters = new Dictionary<TileType, char>
        {
            { TileType.Sea, Convert.ToChar(0x2e) },
            { TileType.UnexploredLand, Convert.ToChar(0x3a) },
            { TileType.ExploredLand, Convert.ToChar(0x40) }
        };

        /// <summary>
        /// Get the available directions.
        /// </summary>
        public static readonly Direction[] Directions = { Direction.North, Direction.East, Direction.South, Direction.West };

        #endregion

        #region Properties

        /// <summary>
        /// Get the name of this map.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Get the tiles.
        /// </summary>
        public TileType[,] Tiles { get; protected set; }

        /// <summary>
        /// Get the width of the map.
        /// </summary>
        public int Width => Tiles?.GetLength(0) ?? 0;

        /// <summary>
        /// Get the height of the map.
        /// </summary>
        public int Height => Tiles?.GetLength(1) ?? 0;

        /// <summary>
        /// Get the percentage of the map that is land.
        /// </summary>
        public double PercentageLand => (100d / (Width * Height)) * (Tiles?.Cast<TileType>().Count(IsLand) ?? 0);

        /// <summary>
        /// Get the percentage of the land that has been explored.
        /// </summary>
        public double PercentageOfLandExplored => (100d / Tiles.Cast<TileType>().Count(IsLand)) * Tiles.Cast<TileType>().Count(x => x == TileType.ExploredLand);

        /// <summary>
        /// Get if there is some remaining unexplored land.
        /// </summary>
        public bool HasRemainingUnexploredLand => Tiles?.Cast<TileType>().Any(x => x == TileType.UnexploredLand) ?? false;

        /// <summary>
        /// Get the landing location.
        /// </summary>
        public Point LandingLocation { get; protected set; }

        /// <summary>
        /// Get the type of tile at a given location.
        /// </summary>
        /// <param name="x">The x location.</param>
        /// <param name="y">The y location.</param>
        /// <returns>The type of tile.</returns>
        public TileType this[int x, int y] => Tiles[x, y];

        #endregion

        #region Methods

        /// <summary>
        /// Get if a position is in the bounds of the map.
        /// </summary>
        /// <param name="coloumn">The column of the tile.</param>
        /// <param name="row">The row of the tile.</param>
        public bool IsInBounds(int coloumn, int row)
        {
            // check grid
            if ((coloumn < 0) || (row < 0) || (coloumn >= Width) || (row >= Height))
                return false;

            // check not unknown
            return Tiles[coloumn, row] != TileType.Unknown;
        }

        /// <summary>
        /// Mark a tile as explored.
        /// </summary>
        /// <param name="coloumn">The column of the tile.</param>
        /// <param name="row">The row of the tile.</param>
        public void MarkTileAsExplored(int coloumn, int row)
        {
            if (!IsLand(Tiles[coloumn, row]))
                throw new InvalidOperationException($"The tile at {coloumn},{row} is not land.");

            Tiles[coloumn, row] = TileType.ExploredLand;
        }
        
        /// <summary>
        /// Serialize the map to a file.
        /// </summary>
        /// <param name="path">The path that points to the location of the map file.</param>
        public void Serialize(string path)
        {
            if (!path.EndsWith(FileExtension))
                throw new InvalidOperationException($"The path must end with {FileExtension}.");

            var mapData = new StringBuilder();

            // populate the data
            for (var i = 0; i < Tiles.GetLength(1); i++)
            {
                for (var j = 0; j < Tiles.GetLength(0); j++)
                    mapData.Append(FileHandlingTileCharacters[Tiles[j, i]]);

                mapData.Append(Environment.NewLine);
            }

            using (var writer = new StreamWriter(path))
                writer.Write(mapData.ToString());
        }

        /// <summary>
        /// Deserialize the map from a file.
        /// </summary>
        /// <param name="path">The path that points to the location of the map file.</param>
        public void Deserialize(string path)
        {
            if (!path.EndsWith(FileExtension))
                throw new InvalidOperationException($"The path must end with {FileExtension}.");

            Name = Path.GetFileName(path).Replace(FileExtension, string.Empty);

            // read the data, splitting it into rows at the newline character
            var mapData = File.ReadAllText(path).Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            Tiles = new TileType[mapData.Max(x => x.Length), mapData.Length];

            for (var i = 0; i < mapData.Length; i++)
                for (var j = 0; j < mapData[i].Length; j++)
                    Tiles[j, i] = FileHandlingTileCharacters.First(x => x.Value == mapData[i][j]).Key;

            var landingLocations = Tiles.Cast<TileType>().Count(x => x == TileType.ExploredLand);

            if (landingLocations == 0)
                throw new InvalidOperationException("There is no landing location.");

            if (landingLocations > 1)
                throw new InvalidOperationException($"There are {landingLocations} possible landing locations.");

            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    if (Tiles[j, i] != TileType.ExploredLand)
                        continue;
                    
                    LandingLocation = new Point(j, i);
                    return;
                }
            }
        }

        #endregion

        #region StaticMethods

        /// <summary>
        /// Generate a new map.
        /// </summary>
        /// <param name="name">The name of the map.</param>
        /// <param name="width">The width of the map.</param>
        /// <param name="height">The height of the map.</param>
        /// <param name="continuityBias">The bias towards continuity, should be specified as a value between 0-1, where 0 represents no bias and 1 is a strong bias.</param>
        /// <returns>The generated map.</returns>
        public static Map Generate(string name, int width, int height, double continuityBias)
        {
            var random = new Random();
            var tiles = new TileType[width, height];

            // randomly set one unexplored tile as the landing location
            for (var i = 0; i < height; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    // get the influencing tiles - north and east
                    var north = i > 0 ? tiles[j, i - 1] : TileType.Unknown;
                    var east = j > 0 ? tiles[j - 1, i] : TileType.Unknown;
                    var influencingTiles = new[] { north, east }.Where(x => ((x == TileType.UnexploredLand) || (x == TileType.Sea))).Distinct().ToArray();

                    // next step is depending on count of tiles
                    switch (influencingTiles.Length)
                    {
                        case 0:
                        case 2:

                            // randomly pick the next tile
                            tiles[j, i] = random.Next(0, 2) == 0 ? TileType.Sea : TileType.UnexploredLand;

                            break;

                        case 1:

                            // determine if keeping the previous tile type
                            var randomDouble = random.NextDouble();
                            var keepSameType = ((randomDouble + continuityBias) / 2) > 0.5;

                            if (keepSameType)
                                tiles[j, i] = influencingTiles[0];
                            else
                                tiles[j, i] = influencingTiles[0] == TileType.Sea ? TileType.UnexploredLand : TileType.Sea;

                            break;

                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            // ensure at least one tile of unexplored land
            if (tiles.Cast<TileType>().All(x => x != TileType.UnexploredLand))
            {
                // add in some unexplored land at a random location
                var x = random.Next(0, width);
                var y = random.Next(0, height);

                tiles[x, y] = TileType.UnexploredLand;
            }
            
            var unexploredLandIndexes = tiles.Cast<TileType>().Select((t, i) => new { t, i })
                                                              .Where(x => x.t == TileType.UnexploredLand)
                                                              .Select(x => x.i).ToArray();

            var landingLocationIndex = random.Next(0, unexploredLandIndexes.Length);
            var landingLocation = new Point(unexploredLandIndexes[landingLocationIndex] % width, unexploredLandIndexes[landingLocationIndex] / width);

            tiles[landingLocation.X, landingLocation.Y] = TileType.ExploredLand;

            return new Map
            {
                Name = name,
                Tiles = tiles,
                LandingLocation = landingLocation
            };
        }

        /// <summary>
        /// Determine if a tile is land.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>True if the type if land, else false.</returns>
        public static bool IsLand(TileType type)
        {
            switch (type)
            {
                case TileType.ExploredLand:
                case TileType.UnexploredLand:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Convert a path representing an unexplored map to a path representing an explored map.
        /// </summary>
        /// <param name="path">The unexplored map path.</param>
        /// <returns>The explored map path.</returns>
        public static string ConvertUnexploredMapPathToExploredMapPath(string path)
        {
            var fileName = Path.GetFileName(path);
            var directory = Path.GetDirectoryName(path);
            return Path.Combine(directory ?? string.Empty, $"{ExploredMapPrefix}{fileName}");
        }

        #endregion
    }
}
