using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BP.Rover.UI.Converters
{
    /// <summary>
    /// Converts between a 2D array of TileTyle and Point marking the location of the rover and a BitmapSource.
    /// </summary>
    [ValueConversion(typeof(Tuple<TileType[,], System.Drawing.Point>), typeof(BitmapSource))]
    public class MapTilesAndRoverPositionToBitmapConverter : IMultiValueConverter
    {
        #region Implementation of IMultiValueConverter

        /// <summary>Converts source values to a value for the binding target. The data binding engine calls this method when it propagates the values from source bindings to the binding target.</summary>
        /// <param name="values">The array of values that the source bindings in the <see cref="T:System.Windows.Data.MultiBinding" /> produces. The value <see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the source binding has no value to provide for conversion.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>A converted value.If the method returns <see langword="null" />, the valid <see langword="null" /> value is used.A return value of <see cref="T:System.Windows.DependencyProperty" />.<see cref="F:System.Windows.DependencyProperty.UnsetValue" /> indicates that the converter did not produce a value, and that the binding will use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> if it is available, or else will use the default value.A return value of <see cref="T:System.Windows.Data.Binding" />.<see cref="F:System.Windows.Data.Binding.DoNothing" /> indicates that the binding does not transfer the value or use the <see cref="P:System.Windows.Data.BindingBase.FallbackValue" /> or the default value.</returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var tiles = values[0] as TileType[,];

                if (tiles == null)
                    return null;

                var roverLocation = (System.Drawing.Point)values[1];

                var width = tiles.GetLength(0);
                var height = tiles.GetLength(1);
                var bytesPerPixel = 3;
                var stride = width * bytesPerPixel;
                var pixelData = new byte[stride * height];

                var roverColor = (Color)App.Current.FindResource("RoverColor");

                var colors = new Dictionary<TileType, Color>
                {
                    { TileType.Unknown, (Color)App.Current.FindResource("MapUnknownTileColor") },
                    { TileType.Sea, (Color)App.Current.FindResource("MapSeaTileColor") },
                    { TileType.UnexploredLand, (Color)App.Current.FindResource("MapUnexploredLandTileColor") },
                    { TileType.ExploredLand, (Color)App.Current.FindResource("MapExploredLandTileColor") },
                    { TileType.LandBeingExplored, (Color)App.Current.FindResource("MapLandBeingExploredTileColor") }
                };

                for (var i = 0; i < height; i++)
                {
                    for (var j = 0; j < width; j++)
                    {
                        var pixelIndex = (i * stride) + (j * bytesPerPixel);

                        Color color;

                        if ((roverLocation.X == j) && (roverLocation.Y == i))
                            color = roverColor;
                        else
                            color = colors[tiles[j, i]];

                        pixelData[pixelIndex] = color.R;
                        pixelData[pixelIndex + 1] = color.G;
                        pixelData[pixelIndex + 2] = color.B;
                    }
                }

                return BitmapSource.Create(width, height, 96, 96, PixelFormats.Rgb24, null, pixelData, stride);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception caught in conversion: {e.Message}{Environment.NewLine}{e.StackTrace}");
                return null;
            }
        }

        /// <summary>Converts a binding target value to the source binding values.</summary>
        /// <param name="value">The value that the binding target produces.</param>
        /// <param name="targetTypes">The array of types to convert to. The array length indicates the number and types of values that are suggested for the method to return.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>An array of values that have been converted from the target value back to the source values.</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
