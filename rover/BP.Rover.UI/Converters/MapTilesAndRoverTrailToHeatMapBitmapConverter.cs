using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Color = System.Windows.Media.Color;

namespace BP.Rover.UI.Converters
{
    /// <summary>
    /// Converts between a 2D array of TileTyle and a List of points marking the rovers trail and a BitmapSource.
    /// </summary>
    [ValueConversion(typeof(Tuple<TileType[,], List<Point>>), typeof(BitmapSource))]
    public class MapTilesAndRoverTrailToHeatMapBitmapConverter : IMultiValueConverter
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

                var roverTrail = values[1] as List<Point>;

                if (roverTrail == null)
                    return null;

                var width = tiles.GetLength(0);
                var height = tiles.GetLength(1);
                var bytesPerPixel = 3;
                var stride = width * bytesPerPixel;
                var pixelData = new byte[stride * height];

                if (roverTrail.Any())
                {
                    var histogram = roverTrail.ToList().GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
                    var maxIntentsity = histogram.Values.Max();
                    var heatmapColor = (Color)App.Current.FindResource("HeatMapMaxColor");

                    foreach (var key in histogram.Keys)
                    {
                        var instances = histogram[key];
                        var pixelIndex = (key.Y * stride) + (key.X * bytesPerPixel);

                        pixelData[pixelIndex] = (byte)(heatmapColor.R * (1d / maxIntentsity) * instances);
                        pixelData[pixelIndex + 1] = (byte)(heatmapColor.G * (1d / maxIntentsity) * instances);
                        pixelData[pixelIndex + 2] = (byte)(heatmapColor.B * (1d / maxIntentsity) * instances);
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
