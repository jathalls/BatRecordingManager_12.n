using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace UniversalToolkit
{
    /*////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///*/

    #region GPSLocation

    /// <summary>
    ///     A class to hold details of a particular location
    /// </summary>
    public class GPSLocation
    {
        /// <summary>
        ///     Constructor for a Location class.  Paraeters are GPS co-ordinates for
        ///     Latitude and Longitude as doubles, and an optional name and 3 or 4 letter identification code
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="name"></param>
        /// <param name="ID"></param>
        public GPSLocation(double latitude, double longitude, string name = "", string ID = "")
        {
            m_Latitude = latitude;
            m_Longitude = longitude;
            m_GridRef = ConvertGPStoGridRef(latitude, longitude);
            m_Name = name;
            m_ID = ID;
        }

        /// <summary>
        ///     Alternative constructoer for a Location class object.
        ///     The parameters are a strig defining the WGS84 location, and
        ///     an optional name and 3 or 4 letter identification code.
        ///     The string should be in the format:-
        ///     nn.nnnnn,N,mmm.mmmmm,W[,alt]
        /// </summary>
        /// <param name="WGS84AsciiLocation"></param>
        /// <param name="name"></param>
        /// <param name="id"></param>
        public GPSLocation(string WGS84AsciiLocation, string name = "", string id = "")
        {
            if (ConvertToLatLong(WGS84AsciiLocation, out var latitude, out var longitude))
            {
                m_Latitude = latitude;
                m_Longitude = longitude;
                m_GridRef = ConvertGPStoGridRef(latitude, longitude);
                m_Name = name;
                m_ID = id;
            }
        }
        public static (double Latitude,double Longitude) GetLocation(string locationName)
        {
            return(MapControl.GetLocation(locationName));
        }

/// <summary>
///     The UK grid reference for the location if possible, calculated from the
///     latitude and longitude fields
/// </summary>
public string m_GridRef { get; }

        /// <summary>
        ///     A three or four letter ID for the location.  May be null or empty
        /// </summary>
        public string m_ID { get; }

        /// <summary>
        ///     GPS latitude as a double
        /// </summary>
        public double m_Latitude { get; }

        /// <summary>
        ///     GPS longitude as a double
        /// </summary>
        public double m_Longitude { get; }

        /// <summary>
        ///     The common name for the location.  May be null or empty.
        /// </summary>
        public string m_Name { get; set; }

        /// <summary>
        ///     Converts a GPS position in the form of latitude and longitude into a UK grid reference.
        ///     May not be precise because altitude is not take into account in the conversion, but is
        ///     generally close enough.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static string ConvertGPStoGridRef(double latitude, double longitude)
        {
            var result = "";

            if (latitude == 0.0d && longitude == 0.0d) return (result); // not valid for this location

            var nmea2OSG = new NMEA2OSG();
            if (latitude >= -48.0d && latitude <= 63.0d && longitude >= -12.0d && longitude <= 3.0d) // generous limits for UK and Ireland
                // we have valid latitudes and longitudes
                // for now just assume they are in the OS grid ref acceptable area

                if (nmea2OSG.Transform(latitude, longitude, 0.0d))
                    result = nmea2OSG.ngr;

            return result;
        }

        public bool isValidLocation
        {
            get
            {
                return (GPSLocation.IsValidLocation((decimal?)m_Latitude, (decimal?)m_Longitude));
            }
        }

        /// <summary>
        /// Given a pair of decimal? checks to see if these represent a valid GPS location
        /// which is not 0,0
        /// </summary>
        /// <param name="locationGPSLatitude"></param>
        /// <param name="locationGPSLongitude"></param>
        /// <returns></returns>
        public static bool IsValidLocation(decimal? locationGPSLatitude, decimal? locationGPSLongitude)
        {
            if (locationGPSLatitude == null || locationGPSLongitude == null) return false;

            if (Math.Abs(locationGPSLatitude.Value) > 90.0m) return false;
            if (Math.Abs(locationGPSLongitude.Value) > 180.0m) return false;

            if (locationGPSLatitude.Value == 0.0m && locationGPSLongitude.Value == 0.0m) return false;

            return (true);
        }

        public static bool IsValidLocation(double? latitude, double? longitude)
        {
            if(latitude == null || longitude == null) return false;
            if(double.IsNaN(latitude.Value) || double.IsNaN(longitude.Value)) return false;

            if (Math.Abs(latitude.Value) > 90.0d) return false;
            if (Math.Abs(longitude.Value) > 180.0d) return false;

            if (latitude.Value == 0.0d && longitude.Value == 0.0d) return false;

            return (true);
        }

        /// <summary>
        ///     Converts a string in the format "blah nn.nnnnn,N,mmm.mmmmm,W[,alt]
        ///     into a latitude and longitude pair in the form of doubles
        /// </summary>
        /// <param name="wGS84AsciiLocation"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        private bool ConvertToLatLong(string WGS84AsciiLocation, out double latitude, out double longitude)
        {
            var result = false;
            latitude = 200.0d;
            longitude = 200.0d;
            var pattern = @"WGS84,([0-9.-]*),?([NS]?),([0-9.-]*),?([WE]?)"; // e.g. WGS84,51.74607,N,0.26183,W
            var match = Regex.Match(WGS84AsciiLocation, pattern);
            if (match.Success && match.Groups.Count >= 5)
            {
                if (double.TryParse(match.Groups[1].Value, out var dd)) latitude = dd;
                dd = -1.0d;
                if (double.TryParse(match.Groups[3].Value, out dd)) longitude = dd;
                if (match.Groups[2].Value.Contains("S")) latitude = 0.0d - latitude;
                if (match.Groups[4].Value.Contains("W")) longitude = 0.0d - longitude;
            }

            if (latitude < 200.0d && longitude < 200.0d) result = true;
            return result;
        }

        /// <summary>
        /// Displays a map window with a pushpin at the specified location.  if the location is not
        /// valid or NaN then no pushpin is displayed.
        /// Returns a Location? which is either null if the window was dismissed or a the location to
        /// be used - either that supplied or a new added PushPin by the user.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public static Location? displayMapWindow(double latitude, double longitude)
        {
            Location? result = null;
            var mapWindow = new MapWindow(true);

            if (latitude != double.NaN && longitude != double.NaN &&
                Math.Abs(latitude) <= 90.0d &&
                Math.Abs(longitude) <= 180.0d &&
                !(latitude == 0.0d && longitude == 0.0d))
            {
                var oldLocation = new Location(latitude, longitude);

                mapWindow.setLocation(oldLocation);
                mapWindow.SetPushPin(oldLocation);
            }


            mapWindow.Title = mapWindow.Title + " Double-Click to Set new location";
            if (mapWindow.ShowDialog() ?? false)
            {
                result = mapWindow.lastSelectedLocation;

            }
            return (result);
        }
    }

    #endregion GPSLocation
}
