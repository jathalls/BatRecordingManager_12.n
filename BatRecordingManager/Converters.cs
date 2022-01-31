/*##################################################
 * This file contains miscellaneous converter classes, which used to be scattered about or in the Tools.cs file.
 * Having them here is tidier, but not all may have been transferred.
 *
 * The Converters are then tagged for use as static resources in the file
 *
 *      BatStyleDictionary.xaml
 *
 * */

using System;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BatRecordingManager
{
    #region CallDisplayEnabledConverter (ValueConverter)

    public class CallDisplayEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                if (value != null && value is LabelledSegment)
                {
                    var ls = value as LabelledSegment;
                    if (ls != null && !ls.SegmentCalls.IsNullOrEmpty())
                    {
                        return (true);
                    }
                }
                return (false);
            }
            catch
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    #endregion CallDisplayEnabledConverter (ValueConverter)

    #region BatLatinNameConverter (ValueConverter)

    /// <summary>
    /// </summary>
    public class BatLatinNameConverter : IValueConverter
    {
        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                var bat = value as Bat;
                var latinName = "";
                if (bat != null) latinName = bat.Batgenus + " " + bat.BatSpecies;
                return latinName;
            }
            catch (Exception ex)
            {
                Tools.ErrorLog(ex.Message);
                Debug.WriteLine(ex);
                return value;
            }
        }

        /// <summary>
        ///     Converts the back.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    #endregion BatLatinNameConverter (ValueConverter)

    #region BatTagSortConverter (ValueConverter)

    /// <summary>
    /// </summary>
    public class BatTagSortConverter : IValueConverter
    {
        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.

                var tagList = value as EntitySet<BatTag>;
                var sortedTagList = (from tag in tagList
                                     orderby tag.SortIndex
                                     select tag);


                return sortedTagList;
            }
            catch (Exception ex)
            {
                Tools.ErrorLog(ex.Message);
                Debug.WriteLine(ex);
                return value;
            }
        }

        /// <summary>
        ///     Converts the back.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    #endregion BatTagSortConverter (ValueConverter)

    #region DivideBy2Converter (ValueConverter)

    /// <summary>
    ///     Converter to divide numeric values by 2
    /// </summary>
    public class DivideBy2Converter : IValueConverter

    {
        /// <summary>
        ///     Converter to divide values by 2
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                return (double)value / 2;
            }
            catch
            {
                return value;
            }
        }

        /// <summary>
        ///     convert back not implemented
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    #endregion DivideBy2Converter (ValueConverter)

    #region TimeSpanConverter (ValueConverter)

    /// <summary>
    ///     Converter class for displaying a timespan object as a string
    /// </summary>
    public class TimeSpanConverter : IValueConverter
    {
        /// <summary>
        ///     Converts a Timespan object into a formatted string
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                if (!(value is TimeSpan ts)) return "";
                var result = Tools.FormattedTimeSpan(ts);

                return result;
            }
            catch
            {
                return value;
            }
        }

        /// <summary>
        ///     Converts the back.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    #endregion TimeSpanConverter (ValueConverter)

    #region CallParametersConverter (ValueConverter)

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    public class CallParametersConverter : IValueConverter
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                var summary = "";
                var segment = value as LabelledSegment;
                if (segment.SegmentCalls != null && segment.SegmentCalls.Count > 0)
                    foreach (var call in segment.SegmentCalls)
                    {
                        if (!string.IsNullOrWhiteSpace(summary))
                            summary = summary + @"
";
                        else
                            summary = "";
                        summary = summary +
                                  $"{call.Call.StartFrequency,5:##0.0},{call.Call.EndFrequency,5:##0.0},{call.Call.PeakFrequency,5:##0.0}kHz {call.Call.PulseDuration,5:##0.0},{call.Call.PulseInterval,5:##0.0}mS";
                    }

                return summary;
            }
            catch
            {
                return value;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            // Not implemented
            return null;
        }
    }

    #endregion CallParametersConverter (ValueConverter)

    #region RecordingToGPSConverter (ValueConverter)

    /// <summary>
    ///     Converter to extract GPS data from a recording instance and format it into a string
    /// </summary>
    public class RecordingToGpsConverter : IValueConverter
    {
        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                var result = "";
                if (value is Recording recording)
                {
                    if (!string.IsNullOrWhiteSpace(recording.RecordingGPSLatitude) &&
                        !string.IsNullOrWhiteSpace(recording.RecordingGPSLongitude))
                    {
                        result = recording.RecordingGPSLatitude + ", " + recording.RecordingGPSLongitude;
                    }
                    else
                    {
                        if (recording.RecordingEndTime != null && recording.RecordingStartTime != null)
                            result = recording.RecordingStartTime.Value.ToString(@"hh\:mm\:ss") + " - " +
                                     recording.RecordingEndTime.Value.ToString(@"hh\:mm\:ss");
                    }
                }

                return result;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        ///     Not implemented
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    #endregion RecordingToGPSConverter (ValueConverter)

    #region RecordingDetailsConverter (ValueConverter)

    /// <summary>
    ///     Converts the essential details of a Recording instance to a string
    /// </summary>
    public class RecordingDetailsConverter : IValueConverter
    {
        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                var result = "";

                if (value is Recording recording)
                {
                    var dur = new TimeSpan();

                    dur = Tools.GetRecordingDuration(recording);

                    var durStr = dur.ToString(@"dd\:hh\:mm\:ss");
                    while (durStr.StartsWith("00:")) durStr = durStr.Substring(3);
                    var recDate = "";
                    if (recording.RecordingDate != null)
                    {
                        recDate = recording.RecordingDate.Value.ToShortDateString();
                        if (recording.RecordingDate.Value.Hour > 0 || recording.RecordingDate.Value.Minute > 0 ||
                            recording.RecordingDate.Value.Second > 0)
                            recDate = recDate + " " + recording.RecordingDate.Value.ToShortTimeString();
                    }

                    result = recording.RecordingName + " " + recDate + " " + durStr;
                }

                return result;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        ///     ConvertBack not implemented
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    #endregion RecordingDetailsConverter (ValueConverter)

    #region RecordingDurationConverter (ValueConverter)

    /// <summary>
    ///     Converts the essential details of a Recording instance to a string
    /// </summary>
    public class RecordingDurationConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns <see langword="null" />, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                var result = "";

                if (value is Recording recording)
                {
                    var dur = new TimeSpan();

                    dur = Tools.GetRecordingDuration(recording);

                    var durStr = dur.ToString(@"dd\:hh\:mm\:ss");
                    while (durStr.StartsWith("00:")) durStr = durStr.Substring(3);
                    var recDate = "";
                    if (recording.RecordingDate != null)
                    {
                        recDate = recording.RecordingDate.Value.ToShortDateString();
                        if (recording.RecordingDate.Value.Hour > 0 || recording.RecordingDate.Value.Minute > 0 ||
                            recording.RecordingDate.Value.Second > 0)
                            recDate = recDate + " " + recording.RecordingDate.Value.ToShortTimeString();
                    }

                    result = durStr;
                }

                return result;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Converts a value.
        /// </summary>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns <see langword="null" />, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    #endregion RecordingDurationConverter (ValueConverter)

    #region RecordingPassSummaryConverter (ValueConverter)

    /// <summary>
    ///     From an instance of Recording provides a list of strings summarising the number of
    ///     passes organised by type of bat
    /// </summary>
    public class RecordingPassSummaryConverter : IValueConverter
    {
        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                var result = "";
                if (value is Recording recording)
                {
                    var summary = recording.GetStats();
                    if (summary != null && summary.Count > 0)
                        foreach (var batType in summary)
                            result = result + "\n" + Tools.GetFormattedBatStats(batType, false);
                }

                return result;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        ///     ConvertBack not implemented
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    #endregion RecordingPassSummaryConverter (ValueConverter)

    /// <summary>
    /// Uses a converterparameter to increase or decrease the numerical (double) value in the object
    /// </summary>
    public class AddValueConverter : IValueConverter

    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)

        {
            try
            {
                double? dValue = value as double?;
                // Here's where you put the code to handle the value conversion.
                double.TryParse(parameter as string, out var factor);
                if (double.IsNaN(dValue ?? double.NaN) || (dValue ?? 0.0d) < 0.0d)
                {
                    dValue = 0.0d;
                }
                var result = (double)(dValue ?? 0.0d) + factor;
                if (result < 0.0d) result = 0.0d;
                return (result);
            }
            catch
            {
                return value;
            }
        }

        /// <summary>
        /// convertback not implemented
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    public class BSPassesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                if (value is BatStats)
                {
                    BatStats bs = value as BatStats;
                    string result = bs.passes + "/" + bs.segments;
                    return (result);
                }

                return (" - ");
            }
            catch
            {
                return "ERR";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    public class DebugBreak : IValueConverter
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                //Debug.WriteLine("&&& DebugBreakConverter:- " + value == null ? "null" : (value.ToString()));
                return value;
            }
            catch
            {
                return value;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    /// format converter for decimal values to and from a formatted string, format
    /// specified in the parameter field without the curly braces
    /// </summary>
    public class DecimalToStringConverter : IValueConverter

    {
        /// <summary>
        /// converter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)

        {
            try
            {
                if (value == null) return ("");
                decimal dblValue = (decimal)value;

                string format = "{0.00}";
                if (!string.IsNullOrWhiteSpace((parameter as string)))
                {
                    format = "{" + (parameter as string) + "}";
                }
                // Here's where you put the code do handle the value conversion.
                var str = "";

                str = string.Format(format, dblValue);

                return str;
            }
            catch
            {
                return value.ToString();
            }
        }

        /// <summary>
        /// unconverter
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)

        {
            decimal.TryParse((string)value, out var d);

            return d;
        }
    }

    public class DivideConverter : IValueConverter
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                if (value != null && parameter != null)
                {
                    var val = (double)value;
                    var parm = (double)parameter;
                    return val / parm;
                }

                return (double)value / 2;
            }
            catch
            {
                return value;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            // Not implemented
            return null;
        }
    }

    public class DoubleStringConverter : IValueConverter
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                double dblValue = (double)value;

                string format = "{0.00}";
                if (!string.IsNullOrWhiteSpace((parameter as string)))
                {
                    format = "{" + (parameter as string) + "}";
                }
                // Here's where you put the code do handle the value conversion.
                var str = "";

                str = string.Format(format, dblValue);

                return str;
            }
            catch
            {
                return value.ToString();
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            // Not implemented
            double.TryParse((string)value, out var d);
            if (d < 0) return null;

            return d;
        }
    }

    #region TimeSpanDateConverter (ValueConverter)

    /// <summary>
    /// Converter to return a red brush if the directory in value does not exist and a black brush if it does or in the event of any error
    /// </summary>
    public class FilePathBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                if (value is string)
                {
                    string folder = value as string;
                    if (!string.IsNullOrWhiteSpace(folder))
                    {
                        if (Directory.Exists(folder))
                        {
                            return (new SolidColorBrush(Colors.Black));
                        }
                    }
                }
            }
            catch
            {
                return new SolidColorBrush(Colors.Red);
            }

            return (new SolidColorBrush(Colors.Red));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    public class GPSConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                if (value is RecordingSession)
                {
                    RecordingSession session = value as RecordingSession;
                    return (session.LocationGPSLatitude.ToString() + ", " + session.LocationGPSLongitude.ToString());
                }

                return ("");
            }
            catch
            {
                return "ERR";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Used to set the height of a scale grid inside a canvas of variable size.
    ///     The converter is passed to bound values the height of the parent canvas and a
    ///     scale factor.  it returns a value of the height multiplied by the scale factor.
    /// </summary>
    public class GridScaleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2) return 0.0d as double?;
            if (values.Length == 1) return values[0] as double?;
            var height = values[0] as double?;
            var scale = values[1] as double?;
            return height * scale;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///     ImageConverter converts either a Bitmap or a BitmapImage to a BitmapSource suitable for
    ///     display in a wpf Image control.  The BitmapImage is first converted to a Bitmap then to
    ///     a BitmapSource.
    /// </summary>
    public class ImageConverter : IValueConverter
    {
        //[DllImport("gdi32.dll")]
        //private static extern bool DeleteObject(IntPtr hObject);

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                //<summary>
                // Converts System.Drawing.Bitmap to BitmapSource
                //</summary>
                //
                if (value == null) return null;

                var bmp = new Bitmap(10, 10);
                //No conversion to be done if value is null

                if (value is BitmapImage)
                    try
                    {
                        var bmi = value as BitmapImage;
                        return bmi;

                        /*
                        using (var stream = new MemoryStream())
                        {
                            BitmapEncoder enc = new BmpBitmapEncoder();
                            try
                            {
                                // NB Known to throw a SystemNotSupported Exception which is not an error but a WPF 'Feature'
                                enc.Frames.Add(BitmapFrame.Create(bmi));
                            }catch(Exception)
                            {
                                return (null);
                            }
                            enc.Save(stream);
                            bmp = new Bitmap(stream);
                        }
                        */
                    }
                    catch (Exception ex)
                    {
                        Tools.ErrorLog(ex.Message);
                        return null;
                    }

                //Validate object being converted
                if (value is Bitmap)
                {
                    if (value == null) value = new Bitmap(10, 10);
                    bmp = value as Bitmap;
                }

                return bmp?.ToBitmapSource();
                /*IntPtr HBitmap = bmp.GetHbitmap();
                    try
                    {
                        System.Windows.Media.Imaging.BitmapSizeOptions sizeOptions =
                            System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions();

                        BitmapSource bmps= System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                            HBitmap, IntPtr.Zero, Int32Rect.Empty, sizeOptions);
                        return (bmps);
                    }finally
                    {
                        DeleteObject(HBitmap);
                    }
                    */
            }
            catch
            {
                return value;
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            // Not implemented
            return null;
        }
    }

    public class MapRefConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                if (value is RecordingSession)
                {
                    RecordingSession session = value as RecordingSession;

                    if (session.hasGPSLocation)
                    {
                        var lat = (double)session.LocationGPSLatitude;
                        var longit = (double)session.LocationGPSLongitude;
                        var gridRef = GPSLocation.ConvertGPStoGridRef(lat, longit);
                        return (gridRef);
                    }
                    else
                    {
                        return (" - ");
                    }
                }

                return (" - ");
            }
            catch
            {
                return "ERR";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Converter class for scaling height or width of an image
    /// </summary>
    public class MultiscaleConverter : IMultiValueConverter

    {
        /// <summary>
        ///     Forward scale converter
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)

        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                if (values != null && values.Length >= 2)
                {
                    var height = 1000.0d;
                    var factor = 1.0d;

                    if (values[0] is string)
                    {
                        var strHeight = values[0] == null ? string.Empty : values[0].ToString();
                        double.TryParse(strHeight, out height);
                    }

                    if (values[0] is double) height = ((double?)values[0]).Value;

                    if (values[1] is string)
                    {
                        var strFactor = values[1] == null ? string.Empty : values[1].ToString();
                        double.TryParse(strFactor, out factor);
                    }
                    else if (values[1] is double) factor = ((double?)values[1]).Value;
                    else if (values[1] is int) factor = ((double?)values[1]).Value;

                    return height * factor;
                }

                return 1000.0d;
            }
            catch
            {
                return 1000.0d;
            }
        }

        /// <summary>
        ///     Reverse converter not used
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Converts a LabelledSegment into a Text form as 'start - end comment'
    ///     and appends an asterisk if the segnent has associated images
    /// </summary>
    public class SegmentToTextConverter : IValueConverter
    {
        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null) return "";
                var segment = value as LabelledSegment;
                var result = Tools.FormattedTimeSpan(segment.StartOffset) + " - " +
                             Tools.FormattedTimeSpan(segment.EndOffset) +
                             "  " + segment.Comment;
                while (result.Trim().EndsWith("*")) result = result.Substring(0, result.Length - 1);
                var pattern = @"\(\s*[0-9]*\s*images?\s*\)";
                result = Regex.Replace(result, pattern, "");
                result = result.Trim();
                if (segment.SegmentDatas.Count > 0) result = result + " (" + segment.SegmentDatas.Count + " images )";
                return result;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        ///     Converts the back.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;
            var modifiedSegment = new LabelledSegment { Comment = text };

            return modifiedSegment;
        }
    }

    public class SessionEndDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                if (value is RecordingSession)
                {
                    RecordingSession session = value as RecordingSession;
                    string result = ((session.EndDate ?? session.SessionDate).Date +
                                     (session.SessionEndTime ?? new TimeSpan(23, 59, 0))).ToString();
                    return (result);
                }

                return (" - ");
            }
            catch
            {
                return "ERR";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    public class SessionStartDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                if (value is RecordingSession)
                {
                    RecordingSession session = value as RecordingSession;
                    string result = (session.SessionDate.Date +
                                     (session.SessionStartTime ?? new TimeSpan(18, 0, 0))).ToString();
                    return (result);
                }

                return (" - ");
            }
            catch
            {
                return "ERR";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Converts a nullable DateTime to a short date string safely even for null values
    /// </summary>
    public class ShortDateConverter : IValueConverter
    {
        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                var dateToDisplay = value as DateTime? ?? DateTime.Now;
                return dateToDisplay.ToShortDateString();
            }
            catch
            {
                return value.ToString();
            }
        }

        /// <summary>
        ///     Converts the back.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;
            var result = new DateTime();
            DateTime.TryParse(text, out result);
            return result;
        }
    }

    /// <summary>
    ///     Converts a nullable DateTime to a short date string safely even for null values
    /// </summary>
    public class ShortTimeConverter : IValueConverter
    {
        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.

                if (value as TimeSpan? == null) return "--:--:--";

                var timeToDisplay = value as TimeSpan? ?? new TimeSpan();

                return timeToDisplay.ToString(@"hh\hmm\mss\s");
            }
            catch
            {
                return value.ToString();
            }
        }

        /// <summary>
        ///     Converts the back.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;
            var result = new DateTime();
            DateTime.TryParse(text, out result);
            return result;
        }
    }

    public class TextColourConverter : IValueConverter
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                if (value is LabelledSegment)
                {
                    LabelledSegment seg = value as LabelledSegment;
                    if (seg.isConfidenceLow)
                    {
                        return new SolidColorBrush(System.Windows.Media.Colors.LightPink);
                    }

                    string text = seg.Comment.Trim();
                    //String text = (value as LabelledSegment).Comment;

                    if (string.IsNullOrWhiteSpace(text)) return new SolidColorBrush(System.Windows.Media.Colors.LightCyan);
                    if (text.EndsWith("H"))
                        return new SolidColorBrush(System.Windows.Media.Colors.LightGreen);
                    if (text.EndsWith("M"))
                        return new SolidColorBrush(System.Windows.Media.Colors.LightGoldenrodYellow);
                    if (text.EndsWith("L")) return new SolidColorBrush(System.Windows.Media.Colors.LightPink);
                    return new SolidColorBrush(System.Windows.Media.Colors.LightCyan);
                }
            }
            catch
            {
                return new SolidColorBrush(System.Windows.Media.Colors.LightCyan);
            }
            return new SolidColorBrush(System.Windows.Media.Colors.LightCyan);
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            // Not implemented
            return null;
        }
    }

    public class Times2Converter : IValueConverter
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                double.TryParse(parameter as string, out var factor);
                return (double)value * factor;
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Converts a nullable Timespan into a DateTime of the same number of ticks, or a
    ///     DateTime.Now if it is null
    /// </summary>
    public class TimeSpanDateConverter : IValueConverter
    {
        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null) return DateTime.Now;
                var time = value as TimeSpan? ?? new TimeSpan();
                var result = new DateTime(time.Ticks);
                return result;
            }
            catch
            {
                return value;
            }
        }

        /// <summary>
        ///     Converts the back.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new TimeSpan((value as DateTime? ?? DateTime.Now).Ticks);
            return result;
        }
    }

    #endregion TimeSpanDateConverter (ValueConverter)

    #region TextColourConverter (ValueConverter)

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    #endregion TextColourConverter (ValueConverter)

    #region DebugBreak (ValueConverter)

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    #endregion DebugBreak (ValueConverter)

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    /// <summary>
    /// Universal wait cursor class
    /// </summary>
    public class WaitCursor : IDisposable
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// creates and displays a wait cursor which will revert when the class instance is disposed.
        /// Allows for nested calls.
        /// </summary>
        /// <param name="status"></param>
        /// <param name="caller"></param>
        /// <param name="linenumber"></param>
        public WaitCursor(string status = "null", [CallerMemberName] string caller = null, [CallerLineNumber] int linenumber = 0)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                /* if (status != "null")
                 {
                     //(App.Current.MainWindow as MainWindow).Dispatcher.Invoke((Action)delegate
                     //var currentMainWindow = Application.Current.MainWindow;
                     //MainWindow window = (currentMainWindow as MainWindow);
                     //window.Dispatcher.Invoke(delegate
                     //{
                         Debug.WriteLine("-=-=-=-=-=-=-=-=- "+status+" -=-=-=-=-=-=-=-=-");
                         _oldStatus = MainWindow.SetStatusText(status);
                         //Debug.WriteLine("old Status=" + oldStatus);
                         _previousCursor = Mouse.OverrideCursor;
                         //Debug.WriteLine("old cursor saved");
                         Mouse.OverrideCursor = Cursors.Wait;
                         //Debug.WriteLine("Wait cursor set");

                     //});
                 }
                 else
                 {*/

                if (Mouse.OverrideCursor == null)
                {
                    var mw = (App.Current.MainWindow as MainWindow);
                    if (mw != null)
                    {
                        mw.Dispatcher.Invoke(delegate
                        {
                            _previousCursor = Mouse.OverrideCursor;
                            Mouse.OverrideCursor = Cursors.Wait;
                            Debug.WriteLine(
                                $"%%%%%%%%%%%%%%%%%%%%%%%%%    WAIT - from {caller} at {linenumber} - {DateTime.Now.ToLongTimeString()}");
                        });
                    }
                }
                else
                {
                    Depth = 1;
                    Debug.WriteLine($"No wait cursor set from {caller}");
                }

                //Application.Current.MainWindow.Dispatcher.InvokeAsync(() => { Mouse.OverrideCursor = _previousCursor; },
                //System.Windows.Threading.DispatcherPriority.ApplicationIdle);
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine("%%%%%%%%%%%%%%%%%  WaitCursor failed for \"" + status + "\":-" + ex.Message);
            }
        }

        /// <summary>
        /// Suspends the current wait cursor, replacing it with a standard arrow cursor
        /// </summary>
        public void Suspend()
        {
            var mw = (App.Current.MainWindow as MainWindow);
            if (mw != null)
            {
                mw.Dispatcher.Invoke(delegate
                {
                    
                    Mouse.OverrideCursor = Cursors.Arrow;
                    Debug.WriteLine($"%%%%%%%%%%%%%%%%%%%%%%%%%    Wait cursor suspended");
                });
            }
        }

        public void Restore()
        {
            var mw = (App.Current.MainWindow as MainWindow);
            if (mw != null)
            {
                mw.Dispatcher.Invoke(delegate
                {

                    Mouse.OverrideCursor = Cursors.Wait;
                    Debug.WriteLine($"%%%%%%%%%%%%%%%%%%%%%%%%%    Wait cursor restored");
                });
            }
        }

        public void Dispose()
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool all)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                if (Depth == 0)
                {
                    var mw = (App.Current.MainWindow as MainWindow);
                    if (mw != null)
                    {
                        mw.Dispatcher.Invoke(delegate
                        {
                            //Mouse.OverrideCursor = _previousCursor ?? Cursors.Arrow;
                            Mouse.OverrideCursor = null;
                            Debug.WriteLine(
                                $"%-%-%-%-%-%-%_%-%-%-%-%-%-- RESUME {Mouse.OverrideCursor} at {DateTime.Now.ToLongTimeString()}");
                        });
                    }
                    else
                    {
                        Debug.WriteLine("No Main Window, failed to reset cursor");
                    }
                }
                else
                {
                    Debug.WriteLine("No cursor reset");
                }
                /*
                if (_oldStatus != "null")
                    //(App.Current.MainWindow as MainWindow).Dispatcher.Invoke((Action)delegate

                    MainWindow.SetStatusText(_oldStatus);*/
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error disposing of wait cursor:- " + ex.Message);
            }
        }

        //private readonly string _oldStatus = "null";
        private readonly int Depth = 0;
        private Cursor _previousCursor = Cursors.Arrow;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    }

    #region DivideConverter (ValueConverter)

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    #endregion DivideConverter (ValueConverter)

    #region Times2Converter (ValueConverter)

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    #endregion Times2Converter (ValueConverter)

    #region HGridLineConverter (ValueConverter)

    /// <summary>
    ///     Converts a LabelledSegment instance to an intelligible string for display
    /// </summary>
    public class BatCallConverter : IValueConverter
    {
        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                var result = new Call();
                if (value is LabelledSegment segment)
                {
                    result = DBAccess.GetSegmentCall(segment) ?? new Call();
                }

                return result;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        ///     ConvertBack not implemented
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Converter class for scaling height or width of an image
    ///     It is passed the location of the line in the stored image and a copy of
    ///     the displayImageCanvas it is to be written and the storedImage it relates to
    /// </summary>
    public class BottomMarginConverter : IMultiValueConverter

    {
        /// <summary>
        ///     Forward scale converter
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)

        {
            try
            {
                var width = values[0] as double?;
                var height = values[1] as double?;

                if (width != null && height != null && values[2] is StoredImage si)
                {
                    //Debug.WriteLine("============================================================================================");
                    var hscale = width.Value / si.image.Width;
                    var vscale = height.Value / si.image.Height;
                    var actualScale = Math.Min(hscale, vscale);

                    var rightAndLeftMargins = Math.Abs(width.Value - si.image.Width * actualScale);
                    var topAndBottomMargins = Math.Abs(height.Value - si.image.Height * actualScale);

                    return topAndBottomMargins / 2 + si.image.Height * actualScale;
                }

                return 0.0d;
            }
            catch
            {
                return 0.0d;
            }
        }

        /// <summary>
        ///     Reverse converter not used
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Converter to get the number of images associated with a bat and return that value
    ///     as a string for display in a DataItem Text Column
    /// </summary>
    public class ConvertGetNumberOfImages : IValueConverter
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                if (value != null)
                {
                    var bat = value as Bat;
                    var cnt = 0;
                    if (bat.BatPictures != null) cnt = bat.BatPictures.Count;
                    return cnt.ToString();
                }

                return "-";
            }
            catch
            {
                return "-";
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Converter class for scaling height or width of an image
    ///     It is passed the location of the line in the stored image and a copy of
    ///     the displayImageCanvas it is to be written and the storedImage it relates to
    /// </summary>
    public class HGridLineConverter : IMultiValueConverter

    {
        /// <summary>
        ///     Forward scale converter
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)

        {
            try
            {
                int.TryParse(values[0] as string, out var indexToGridline);
                var width = values[1] as double?;
                var height = values[2] as double?;

                var si = values[3] as StoredImage;
                //Debug.WriteLine("HGridLineConverter:- storedValue=" + si.HorizontalGridlines[indexToGridline]);

                if (width == null || height == null || indexToGridline < 0 ||
                    indexToGridline >= si.HorizontalGridlines.Count || si == null) return null;

                var displayPosition =
                    DisplayStoredImageControl.FindHScaleProportion(si.HorizontalGridlines[indexToGridline], width.Value,
                        height.Value, si) * height;
                //Debug.WriteLine("      DisplayedPosition=" + displayPosition);

                return displayPosition;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("HGridLineConverter error:- " + ex.Message);
                return null;
            }
        }

        /// <summary>
        ///     Reverse converter not used
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    #endregion HGridLineConverter (ValueConverter)

    #region VGridLineConverter (ValueConverter)

    /// <summary>
    ///     converter getting images for all recordings
    /// </summary>
    public class ImagesForAllRecordingsConverter : IMultiValueConverter
    {
        /// <summary>
        ///     converter - takes an array of 2 objects.  object[0] is a ObservableCollection
        ///     of Recordings and  object[1] is a bat.  It returns a string representation of the
        ///     number of images in all recordings that include that bat.
        ///     i.e. the number of images linked to labelled segments for these recordings that
        ///     include the named bat
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                if (values == null || values.Length < 2) return "-";

                var numberOfImages = 0;
                if (values[1] == null) return "-";
                var bat = values[1] as Bat;
                if (!(values[0] is ObservableCollection<Recording> recordings) || recordings.Count <= 0) return "-";

                numberOfImages = (from rec in recordings.AsParallel()
                                  from seg in rec.LabelledSegments.AsParallel()
                                  from link in seg.BatSegmentLinks.AsParallel()
                                  where !(link.ByAutoID ?? false) && link.BatID == bat.Id
                                  select seg.SegmentDatas.Count).Sum();

                /*
                foreach(var rec in recordings)
                {
                    if(!rec.LabelledSegments.IsNullOrEmpty())
                    {
                        bool RecordingHasBat = false;
                        numberOfImages+=rec.GetImageCount(bat,out RecordingHasBat);
                    }
                }*/

                return numberOfImages.ToString();
            }
            catch
            {
                return "-";
            }
        }

        /// <summary>
        ///     unconverter not used
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Converter which takes a StoredImage and returns the image component overlaid with horizontal and vertical
    ///     grid lines as defined in the StoredImage lists.
    /// </summary>
    public class ImageWithGridConverter : IValueConverter
    {
        /// <summary>
        ///     Converter to add the grid lines to the image component and reutrn it
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var result = new WriteableBitmap(new BitmapImage());
            try
            {
                // Here's where you put the code do handle the value conversion.
                var sImage = value as StoredImage;
                result = new WriteableBitmap(sImage.image);
                return result;
            }
            catch
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    /// class determines visibility by a boolean - true is hidden, false is visible
    /// </summary>
    public class InVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// converts a boolean to visibility true=hidden false=visible default visible
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is bool)
                {
                    bool? b = value as bool?;
                    if (b ?? false)
                    {
                        return (Visibility.Hidden);
                    }
                    else
                    {
                        return (Visibility.Visible);
                    }
                }
                return Visibility.Visible;
            }
            catch
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Converts a LabelledSegment instance to an intelligible string for display
    /// </summary>
    public class LabelledSegmentConverter : IMultiValueConverter
    {
        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                var result = "";
                bool offsets = true;
                LabelledSegment segment = null;
                if (values == null) return (result);
                if (values.Length < 2) return (result);

                if (values[0] != null)
                {
                    if (values[0] is ToggleButton)
                    {
                        var bt = values[0] as ToggleButton;
                        if (bt.IsEnabled)
                        {
                            offsets = !bt.IsChecked ?? true;
                        }
                        else
                        {
                            offsets = true;
                        }
                    }
                }

                if (values[1] is LabelledSegment)
                {
                    segment = (LabelledSegment)values[1];
                }

                if (segment != null)
                {
                    result = Tools.FormattedSegmentLine(segment, offsets);
                    while (result.Trim().EndsWith("*")) result = result.Substring(0, result.Length - 1);
                    result = result.Trim();
                    if (!result.EndsWith(")") && segment.SegmentDatas.Count > 0)
                        result = result + " (" + segment.SegmentDatas.Count + " images )";
                }

                return result;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        ///     ConvertBack not implemented
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="targetType">
        ///     Type of the target.
        /// </param>
        /// <param name="parameter">
        ///     The parameter.
        /// </param>
        /// <param name="culture">
        ///     The culture.
        /// </param>
        /// <returns>
        /// </returns>
        public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Converter class for scaling height or width of an image
    ///     It is passed the location of the line in the stored image and a copy of
    ///     the displayImageCanvas it is to be written and the storedImage it relates to
    /// </summary>
    public class LeftMarginConverter : IMultiValueConverter

    {
        /// <summary>
        ///     Forward scale converter
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)

        {
            try
            {
                var width = values[0] as double?;
                var height = values[1] as double?;

                if (width != null && height != null && values[2] is StoredImage si)
                {
                    //Debug.WriteLine("============================================================================================");
                    var hscale = width.Value / si.image.Width;
                    var vscale = height.Value / si.image.Height;
                    var actualScale = Math.Min(hscale, vscale);

                    var rightAndLeftMargins = Math.Abs(width.Value - si.image.Width * actualScale);
                    var topAndBottomMargins = Math.Abs(height.Value - si.image.Height * actualScale);

                    return rightAndLeftMargins / 2;
                }

                return 0.0d;
            }
            catch
            {
                return 0.0d;
            }
        }

        /// <summary>
        ///     Reverse converter not used
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Is passed an EntitySet of Recordings and calculates the total number of images
    ///     associated with those recordings, returning the value as a string
    /// </summary>
    public class NumberOfImagesConverter : IValueConverter
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            try
            {
                // Here's where you put the code do handle the value conversion.
                if (value == null) return "0";
                var recordings = value as EntitySet<Recording>;
                if (recordings.Count <= 0) return "0";
                var imgs = 0;

                imgs = (from rec in recordings
                        from seg in rec.LabelledSegments
                        select seg.SegmentDatas.Count).Sum();
                /*
                foreach(var rec in recordings)
                {
                    if(rec.LabelledSegments!=null && rec.LabelledSegments.Count > 0)
                    {
                        foreach(var seg in rec.LabelledSegments)
                        {
                            imgs+=seg.SegmentDatas.Count;
                        }
                    }
                }*/
                return imgs.ToString();
            }
            catch
            {
                return "0";
            }
        }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Converter class for scaling height or width of an image
    ///     It is passed the location of the line in the stored image and a copy of
    ///     the displayImageCanvas it is to be written and the storedImage it relates to
    /// </summary>
    public class RightMarginConverter : IMultiValueConverter

    {
        /// <summary>
        ///     Forward scale converter
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)

        {
            try
            {
                var width = values[0] as double?;
                var height = values[1] as double?;

                if (width != null && height != null && values[2] is StoredImage si)
                {
                    //Debug.WriteLine("============================================================================================");
                    var hscale = width.Value / si.image.Width;
                    var vscale = height.Value / si.image.Height;
                    var actualScale = Math.Min(hscale, vscale);

                    var rightAndLeftMargins = Math.Abs(width.Value - si.image.Width * actualScale);
                    var topAndBottomMargins = Math.Abs(height.Value - si.image.Height * actualScale);

                    return rightAndLeftMargins / 2 + si.image.Width * actualScale;
                }

                return 0.0d;
            }
            catch
            {
                return 0.0d;
            }
        }

        /// <summary>
        ///     Reverse converter not used
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Converter class for scaling height or width of an image
    ///     It is passed the location of the line in the stored image and a copy of
    ///     the displayImageCanvas it is to be written and the storedImage it relates to
    /// </summary>
    public class TopMarginConverter : IMultiValueConverter

    {
        /// <summary>
        ///     Forward scale converter
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)

        {
            try
            {
                var width = values[0] as double?;
                var height = values[1] as double?;

                if (width != null && height != null && values[2] is StoredImage si)
                {
                    //Debug.WriteLine("============================================================================================");
                    var hscale = width.Value / si.image.Width;
                    var vscale = height.Value / si.image.Height;
                    var actualScale = Math.Min(hscale, vscale);

                    var rightAndLeftMargins = Math.Abs(width.Value - si.image.Width * actualScale);
                    var topAndBottomMargins = Math.Abs(height.Value - si.image.Height * actualScale);

                    return topAndBottomMargins / 2;
                }

                return 0.0d;
            }
            catch
            {
                return 0.0d;
            }
        }

        /// <summary>
        ///     Reverse converter not used
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    /// <summary>
    ///     Converter class for scaling height or width of an image
    ///     It is passed the location of the line in the stored image and a copy of
    ///     the displayImageCanvas it is to be written and the storedImage it relates to
    /// </summary>
    public class VGridLineConverter : IMultiValueConverter

    {
        /// <summary>
        ///     Forward scale converter
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)

        {
            try
            {
                int.TryParse(values[0] as string, out var indexToGridline);
                var width = values[1] as double?;
                var height = values[2] as double?;

                var si = values[3] as StoredImage;

                if (width == null || height == null || indexToGridline < 0 ||
                    indexToGridline >= si.VerticalGridLines.Count || si == null) return null;

                var displayPosition =
                    DisplayStoredImageControl.FindVScaleProportion(si.VerticalGridLines[indexToGridline], width.Value,
                        height.Value, si) * width;

                return displayPosition;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///     Reverse converter not used
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    #endregion VGridLineConverter (ValueConverter)

    #region VisibilityConverter (ValueConverter)

    /// <summary>
    /// converter class for boolean to visibility
    /// </summary>
    public class VisibilityConverter : IValueConverter
    {
        /// <summary>
        /// converts a bool to visibility true=visible false=hidden default visible
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (value is bool || value is bool?)
                {
                    bool? b = value as bool?;
                    if (b ?? false)
                    {
                        return (Visibility.Visible);
                    }
                    else
                    {
                        return (Visibility.Hidden);
                    }
                }
                return Visibility.Visible;
            }
            catch
            {
                return Visibility.Visible;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            // Not implemented
            return null;
        }
    }

    #endregion VisibilityConverter (ValueConverter)


}