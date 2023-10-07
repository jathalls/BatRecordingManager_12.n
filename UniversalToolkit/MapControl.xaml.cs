// *  Copyright 2016 Justin A T Halls
//  *
//  *  This file is part of the Bat Recording Manager Project
//
//         Licensed under the Apache License, Version 2.0 (the "License");
//         you may not use this file except in compliance with the License.
//         You may obtain a copy of the License at
//
//             http://www.apache.org/licenses/LICENSE-2.0
//
//         Unless required by applicable law or agreed to in writing, software
//         distributed under the License is distributed on an "AS IS" BASIS,
//         WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//         See the License for the specific language governing permissions and
//         limitations under the License.

using Microsoft.Maps.MapControl.WPF;
using Shiny;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System.Xml.Linq;

namespace UniversalToolkit;

/// <summary>
///     Interaction logic for MapControl.xaml
/// </summary>
public partial class MapControl : UserControl
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MapControl" /> class.
    /// </summary>
    public MapControl()
    {
        InitializeComponent();
        
        DataContext = this;
        string key = APIKeys.BingMapsLicenseKey;
        this.key = key;
        ThisMap.CredentialsProvider = new ApplicationIdCredentialsProvider(key);
        ThisMap.Focus();
        lastInsertedPinLocation = null;
        LabelMode = ThisMap.Mode;
        
    }

    public string key { get; set; } = "";

    public string BingMapsApiKey { get; } = APIKeys.BingMapsLicenseKey;

    /// <summary>
    ///     Gets or sets the coordinates.
    /// </summary>
    /// <value>
    ///     The coordinates.
    /// </value>
    public Location coordinates
    {
        get => _coordinates;
        set
        {
            _coordinates = value;
            ThisMap.Center = value;
        }
    }

    public void MoveToNamedLocation(string locationName)
    {
        

    }

    public static (double Latitude, double Longitude) GetLocation(string locationName)
    {
        (double latitude, double longitude) result = (0, 0);
        string geoCodeRequest = "http://dev.virtualearth.net/REST/v1/Locations/" + locationName + "?o=xml&key=" + APIKeys.BingMapsLicenseKey ;
        //XmlDocument geoCodeResponse = GetXmlResponse(geoCodeRequest);

        HttpWebResponse response = null;
        HttpWebRequest request = WebRequest.Create(geoCodeRequest) as HttpWebRequest;
        using (response = request.GetResponse() as HttpWebResponse)
        {

            StreamReader reader = new StreamReader(response.GetResponseStream());
            string xml = reader.ReadToEnd();

            XElement root=XElement.Parse(xml); 
            var locels = root.Descendants("Location");
            var decs = root.Descendants();
            var res = from d in decs
                      where d.Name.LocalName=="Location"
                      select d;
            var locs = from des in res.Descendants()
                       where des.Name.LocalName == "Point"
                       select des;

            if(!locs.IsNullOrDefault() && locs.Count() > 0)
            {
                var point = locs.FirstOrDefault();
                var lat = point.Descendants().Where(d=>d.Name.LocalName=="Latitude").Select(d=>d.Value).FirstOrDefault();
                var longit= point.Descendants().Where(d => d.Name.LocalName == "Longitude").Select(d => d.Value).FirstOrDefault();
                if (double.TryParse(lat, out double Lat) && double.TryParse(longit, out double Longit))
                {
                    result = (Lat, Longit);
                    
                }
            }






        }
        

        return (result);
    }

    public Location lastInsertedPinLocation { get; set; }

    /// <summary>
    ///     Adds the push pin.
    /// </summary>
    /// <param name="pinCoordinates">
    ///     The pin coordinates.
    /// </param>
    /// <param name="text">
    ///     The text.
    /// </param>
    public void AddPushPin(Location pinCoordinates, string text, int ordinate = -1)
    {
        var pin = new Pushpin
        {
            Location = pinCoordinates,
            Content = ordinate >= 0 ? ordinate.ToString() : "",
            PositionOrigin = PositionOrigin.BottomCenter
        };
        ToolTip tip = new ToolTip();
        tip.Content = text;
        pin.ToolTip = tip;

        ThisMap.Children.Add(pin);
    }

    public void AddPushPin(Location location)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        //var pin = new Pushpin {Location = location,PositionOrigin = PositionOrigin.BottomCenter};
        var pin = new Pushpin();
        pin.PositionOrigin = PositionOrigin.BottomCenter;
        pin.Location = location;
        
        ThisMap.Children.Add(pin);
    }

    private Location _coordinates;

    private MapMode AerialMode { get; set; }
    private MapMode LabelMode { get; set; }
    private MapMode RoadMode { get; set; }
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

    private void AerialButton_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        ThisMap.Mode = new AerialMode();
    }

    private void AerialLabelButton_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        ThisMap.Mode = new AerialMode(true);
    }

    private void mapControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true;
        ThisMap.Children.Clear();
        var mousePosition = e.GetPosition(ThisMap);
        var pinLocation = ThisMap.ViewportPointToLocation(mousePosition);

        var pin = new Pushpin { PositionOrigin = PositionOrigin.BottomCenter };
        pin.Location = pinLocation;
        lastInsertedPinLocation = pinLocation;
        ThisMap.Children.Add(pin);
    }

    private void RoadButton_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
        ThisMap.Mode = new RoadMode();
    }
}