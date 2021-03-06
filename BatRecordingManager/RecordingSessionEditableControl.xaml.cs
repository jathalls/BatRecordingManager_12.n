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
 
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BatRecordingManager
{
    /// <summary>
    ///     Interaction logic for RecordingSessionEditableControl.xaml
    /// </summary>
    public partial class RecordingSessionEditableControl : UserControl
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RecordingSessionEditableControl" /> class.
        /// </summary>
        public RecordingSessionEditableControl()
        {
            selectedFolder = "";
            _equipmentList = DBAccess.GetEquipmentList();
            _microphoneList = DBAccess.GetMicrophoneList();
            _operatorList = DBAccess.GetOperators();
            _locationList = DBAccess.GetLocationList();

            InitializeComponent();
            DataContext = this;
            MicrophoneComboBox.ItemsSource = _microphoneList;
            EquipmentComboBox.ItemsSource = _equipmentList;
            OperatorComboBox.ItemsSource = _operatorList;
            LocationComboBox.ItemsSource = _locationList;
        }

        /// <summary>
        ///     Verifies the form contents.
        /// </summary>
        /// <returns>
        /// </returns>
        /// <exception cref="System.NotImplementedException">
        /// </exception>
        public string VerifyFormContents()
        {
            var result = "";

            if (string.IsNullOrWhiteSpace(recordingSession.SessionTag)) result = "Must have a valid session Tag";
            DateTime? date = recordingSession.SessionDate;
            if (date == null)
            {
                result = "Must have a valid date";
            }
            else
            {
                if (date.Value.Year < 1950) result = "Must have a valid date later than 1950";
                if (date.Value > DateTime.Now) result = "Must have a valid date earlier than now";
            }

            if (string.IsNullOrWhiteSpace(recordingSession.Location)) result = "Must have a valid Location";

            return result;
        }

        private  ObservableCollection<string> _equipmentList = new ObservableCollection<string>();
        private  ObservableCollection<string> _locationList = new ObservableCollection<string>();
        private ObservableCollection<string> _microphoneList = new ObservableCollection<string>();
        private  ObservableCollection<string> _operatorList = new ObservableCollection<string>();
        private bool appendWeather = false;

        private void FolderBrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var fileBrowser = new FileBrowser();
            fileBrowser.SelectHeaderTextFile();
            FolderTextBox.Text = fileBrowser.WorkingFolder;
        }

        /// <summary>
        /// finds a time for the weather forecast, on the first day of the session and half way between the start
        /// and end times if a single day session, or halfway between the start time and midnight if a multiday
        /// session.  uses 21:00 if there is insufficient date aand time information in the session.
        /// </summary>
        /// <param name="recordingSession"></param>
        /// <returns></returns>
        private TimeSpan getMidSessionTime(RecordingSession session)
        {
            TimeSpan result = new TimeSpan(21, 0, 0); // default value if all else fails.
            var startDate = session.SessionDate.Date;
            var startTime = session.SessionStartTime ?? session.SessionDate.TimeOfDay;
            if (startTime.Hours < 12)
            {
                // we dont have a valid start time, so try to guess at something reasonable
                if (session.Sunset == null)
                {
                    SunsetCalcButton_Click(this, new RoutedEventArgs());
                }
                if (session.Sunset != null && session.Sunset.Value.Hours > 12)
                {
                    startTime = session.Sunset.Value; // first guess is to use sunset
                }
                else
                {
                    startTime = new TimeSpan(18, 0, 0); // second guess is to use 18:00 hours
                }
            }

            var endDate = session.SessionEnd.Date;
            TimeSpan endTime = TimeSpan.FromMinutes(1440);
            if (endDate <= startDate)
            {// we have a single day session
                if (session.SessionEndTime != null && session.SessionEndTime.Value > startTime)
                {
                    endTime = session.SessionEndTime.Value;
                }
            }

            TimeSpan weatherTime = TimeSpan.FromMinutes((startTime.TotalMinutes + endTime.TotalMinutes) / 2);
            return weatherTime;
        }

        private void GPSLatitudeTextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Location coordinates;
            if (!double.TryParse(GpsLatitudeTextBox.Text, out var lat)) return;
            if (!double.TryParse(GpsLongitudeTextBox.Text, out var longit)) return;
            if (Math.Abs(lat) > 90.0 || Math.Abs(longit) > 180.0d) return;
            coordinates = new Location(lat, longit);

            var mapWindow = new MapWindow(false) { MapControl = { coordinates = coordinates } };
            mapWindow.Show();
            if (recordingSession?.Recordings != null && recordingSession.Recordings.Count > 0)
            {
                var i = 0;
                foreach (var rec in recordingSession.Recordings)
                {
                    i++;
                    if (double.TryParse(rec.RecordingGPSLatitude, out var latitude))
                        if (double.TryParse(rec.RecordingGPSLongitude, out var longitude))
                            if (latitude <= 90.0 && latitude >= -90.0 && longitude <= 180.0 && longitude >= -180.0 &&
                                !(latitude == 0.0 && longitude == 0.0))
                                mapWindow.MapControl.AddPushPin(new Location(latitude, longitude), i.ToString());
                }
            }
        }

        /// <summary>
        ///     Handles the Click event of the GPSMapButton control. Displays the map form and sets
        ///     the GPS co-ordinates to the last location pinned with a double click
        /// </summary>
        /// <param name="sender">
        ///     The source of the event.
        /// </param>
        /// <param name="e">
        ///     The <see cref="RoutedEventArgs" /> instance containing the event data.
        /// </param>
        private void GPSMapButton_Click(object sender, RoutedEventArgs e)
        {
            var mapWindow = new MapWindow(true);
            if (!string.IsNullOrWhiteSpace(GpsLatitudeTextBox.Text) &&
                !string.IsNullOrWhiteSpace(GpsLongitudeTextBox.Text))
            {
                double.TryParse(GpsLatitudeTextBox.Text, out var lat);
                double.TryParse(GpsLongitudeTextBox.Text, out var longit);
                if (Math.Abs(lat) <= 90.0d && Math.Abs(longit) <= 180.0d && !(lat == 0.0d && longit == 0.0d))
                {
                    var oldLocation = new Location(lat, longit);

                    mapWindow.MapControl.ThisMap.Center = oldLocation;
                    mapWindow.MapControl.AddPushPin(oldLocation);
                }
            }

            mapWindow.Title = mapWindow.Title + " Double-Click to Set new location";
            if (mapWindow.ShowDialog() ?? false)
            {
                var lastSelectedLocation = mapWindow.MapControl.lastInsertedPinLocation;
                if (lastSelectedLocation != null)
                {
                    recordingSession.LocationGPSLatitude = (decimal)lastSelectedLocation.Latitude;
                    recordingSession.LocationGPSLongitude = (decimal)lastSelectedLocation.Longitude;
                    //GpsLatitudeTextBox.Text = lastSelectedLocation.Latitude.ToString();
                    //GpsLongitudeTextBox.Text = lastSelectedLocation.Longitude.ToString();
                }
            }
        }

        private void LocationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            string newloc = "";
            if (e.AddedItems.Count > 0) newloc = (string)e.AddedItems[0];
            string oldloc = "";
            if (e.RemovedItems.Count > 0) oldloc = (string)e.RemovedItems[0];
            if (string.IsNullOrWhiteSpace(newloc) || newloc == oldloc)
            {
                return;
            }
            
            if (!string.IsNullOrWhiteSpace(GpsLatitudeTextBox.Text) && GpsLatitudeTextBox.Text != "0" &&
                !string.IsNullOrWhiteSpace(GpsLongitudeTextBox.Text) && GpsLongitudeTextBox.Text != "0")
            {
                // we already have a valid GPS location defined
                return;
            }

            if (_locationList.Contains(newloc))
            {
                GPSLocation gpsLocation = DBAccess.GetGPSForLocation(newloc);
                if (gpsLocation != null)
                {
                    GpsLatitudeTextBox.Text = gpsLocation.m_Latitude.ToString();
                    GpsLongitudeTextBox.Text = gpsLocation.m_Longitude.ToString();
                }
            }

        }

        private void SunsetCalcButton_Click(object sender, RoutedEventArgs e)
        {
            //DateTime? sessionDate = SessionDatePicker.SelectedDate;
            var sessionDate = SessionStartDateTime.SelectedDate;
            double.TryParse(GpsLatitudeTextBox.Text, out var lat);
            double.TryParse(GpsLongitudeTextBox.Text, out var longit);

            if (sessionDate != null && sessionDate.Year > 1970)
                if (lat < 200 && longit < 200 && !(lat == 0.0 && longit == 0.0))
                {
                    var sunset = SessionManager.CalculateSunset(sessionDate, (decimal?)lat, (decimal?)longit);
                    if (sunset != null && sunset.Value.Hours > 0)
                    {
                        recordingSession.Sunset = sunset;
                        DBAccess.UpdateSunset(recordingSession);
                        SunsetTimePicker.SelectedDate = new DateTime(recordingSession.SessionDate.Year,
                            recordingSession.SessionDate.Month, recordingSession.SessionDate.Day,
                            sunset.Value.Hours, sunset.Value.Minutes, sunset.Value.Seconds);
                    }
                }
        }

        #region recordingSession

        /// <summary>
        ///     recordingSession Dependency Property
        /// </summary>
        public static readonly DependencyProperty recordingSessionProperty =
            DependencyProperty.Register(nameof(recordingSession), typeof(RecordingSession),
                typeof(RecordingSessionEditableControl),
                new FrameworkPropertyMetadata(new RecordingSession()));

        /// <summary>
        ///     Gets or sets the recordingSession property. This dependency property indicates ....
        /// </summary>
        public RecordingSession recordingSession
        {
            get
            {
                var session = (RecordingSession)GetValue(recordingSessionProperty);
                if (session != null)
                    try
                    {
                        session.SessionTag = SessionTagTextBlock.Text;
                        session.SessionDate = SessionStartDateTime.SelectedDate;
                        session.SessionStartTime = session.SessionDate.TimeOfDay;

                        session.SessionEndTime = SessionEndDateTime.SelectedDate.TimeOfDay;
                        session.EndDate = SessionEndDateTime.SelectedDate;

                        //session.SessionDate = SessionStartDatePicker.SelectedDate ?? new DateTime();
                        //session.SessionStartTime = (StartTimePicker.Value ?? new DateTime()).TimeOfDay;
                        //session.SessionEndTime = (EndTimePicker.Value ?? new DateTime()).TimeOfDay;
                        session.Temp = (short?)TemperatureIntegerUpDown.Value;
                        session.Weather = WeatherTextBox.Text;
                        session.Sunset = (SunsetTimePicker.SelectedDate).TimeOfDay;
                        session.Equipment = EquipmentComboBox.Text;
                        session.Microphone = MicrophoneComboBox.Text;
                        session.OriginalFilePath = FolderTextBox.Text;

                        session.Location = LocationComboBox.Text;

                        //decimal.TryParse(GpsLatitudeTextBox.Text, out var value);
                        //session.LocationGPSLatitude = value;
                        //value = 0.0m;
                        //decimal.TryParse(GpsLongitudeTextBox.Text, out value);
                        //session.LocationGPSLongitude = value;

                        session.Operator = OperatorComboBox.Text;
                        session.SessionNotes = SessionNotesRichtextBox.Text;
                        session.TimeCorrection();
                    }
                    catch (Exception ex)
                    {
                        Tools.ErrorLog(ex.Message);
                        Debug.WriteLine("RecordingSessionEditableControl - Get Session:- " + ex);
                    }

                return session;
            }
            set
            {
                if (value != null)
                    try
                    {
                        selectedFolder = value.SessionTag;
                        SessionTagTextBlock.Text = value.SessionTag;

                        //SessionStartDateTime.Value =
                        value.SessionDate = value.SessionDate.Date + (value.SessionStartTime ?? new TimeSpan());
                        //RecordingSessionStartDateTime = value.SessionDate;
                        SessionStartDateTime.SelectedDate = value.SessionDate;

                        if (value.SessionEndTime == null)
                            value.SessionEndTime = value.SessionStartTime ?? new TimeSpan();
                        if (value.EndDate == null)
                            value.EndDate = value.SessionDate.Date +
                                            (value.SessionEndTime ?? (value.SessionStartTime ?? new TimeSpan()));
                        SessionEndDateTime.SelectedDate = value.EndDate ?? new DateTime();

                        //StartTimePicker.Value = new DateTime() + (value.SessionStartTime ?? new TimeSpan());

                        //EndTimePicker.Value = new DateTime() + (value.SessionEndTime ?? new TimeSpan());
                        SunsetTimePicker.SelectedDate = value.SessionDate.Date + (value.Sunset ?? new TimeSpan());
                        WeatherTextBox.Text = value.Weather;
                        EquipmentComboBox.ItemsSource = DBAccess.GetEquipmentList();
                        EquipmentComboBox.Text = value.Equipment;
                        EquipmentComboBox.SelectedItem = value.Equipment;

                        MicrophoneComboBox.ItemsSource = DBAccess.GetMicrophoneList();
                        MicrophoneComboBox.Text = value.Microphone;
                        MicrophoneComboBox.SelectedItem = value.Microphone;

                        //_locationList.Clear();
                        _locationList=new ObservableCollection<string>(DBAccess.GetLocationList());
                        LocationComboBox.ItemsSource = _locationList;
                        LocationComboBox.Text = value.Location;
                        LocationComboBox.SelectedItem = value.Location;

                        OperatorComboBox.ItemsSource = DBAccess.GetOperators();
                        OperatorComboBox.Text = value.Operator;
                        OperatorComboBox.SelectedItem = value.Operator;

                        GpsLatitudeTextBox.Text = (value.LocationGPSLatitude ?? 0.0m).ToString();
                        GpsLongitudeTextBox.Text = (value.LocationGPSLongitude ?? 0.0m).ToString();

                        if (string.IsNullOrWhiteSpace(GpsLatitudeTextBox.Text) || string.IsNullOrWhiteSpace(GpsLongitudeTextBox.Text) || !GPSLocation.IsValidLocation(value.LocationGPSLatitude, value.LocationGPSLongitude))
                        {
                            if (!string.IsNullOrWhiteSpace(value.Location) && _locationList.Contains(value.Location))
                            {
                                var gpsLocation = DBAccess.GetGPSForLocation(value.Location);
                                if (gpsLocation != null && gpsLocation.isValidLocation)
                                {
                                    value.LocationGPSLatitude = (decimal)gpsLocation.m_Latitude;
                                    value.LocationGPSLongitude = (decimal)gpsLocation.m_Longitude;
                                    GpsLatitudeTextBox.Text = value.LocationGPSLatitude.ToString();
                                    GpsLongitudeTextBox.Text = value.LocationGPSLongitude.ToString();
                                }
                            }
                        }
                        value.TimeCorrection();

                        SessionNotesRichtextBox.Text = value.SessionNotes;

                        //SessionDatePicker.DisplayDate = value.SessionDate;
                        //SessionDatePicker.SelectedDate = value.SessionDate;
                        TemperatureIntegerUpDown.Value = value.Temp ?? 0;
                        FolderTextBox.Text = value.OriginalFilePath;
                        /*
                        if (string.IsNullOrWhiteSpace(WeatherTextBox.Text))
                        {
                            if (value.LocationGPSLatitude >= -90.0m && value.LocationGPSLatitude <= 90.0m &&
                                value.LocationGPSLongitude >= -180.0m && value.LocationGPSLongitude <= 180.0m)
                            {
                                Weather weather=new Weather();
                                weather.weatherReceived += Weather_weatherReceived;

                                var res=weather.GetWeatherHistory((double) value.LocationGPSLatitude,
                                    (double) value.LocationGPSLongitude, value.SessionDate.Date+(value.SessionStartTime??new TimeSpan(20,0,0)));
                                WeatherTextBox.Text = res;
                            }
                        }*/
                    }
                    catch (Exception ex)
                    {
                        Tools.ErrorLog(ex.Message);
                        Debug.WriteLine("RecordingSessionEditableControl-Set RecordingSession:- " + ex);
                    }
                else
                    try
                    {
                        SessionTagTextBlock.Text = "";
                        SessionStartDateTime.SelectedDate = DateTime.Now;
                        //RecordingSessionStartDateTime = DateTime.Now;
                        SessionEndDateTime.SelectedDate = DateTime.Now;
                        //StartTimePicker.Value = DateTime.Now;
                        //EndTimePicker.Value = DateTime.Now;

                        EquipmentComboBox.Text = "";
                        MicrophoneComboBox.Text = "";
                        LocationComboBox.Text = "";
                        GpsLatitudeTextBox.Text = "";
                        GpsLongitudeTextBox.Text = "";
                        OperatorComboBox.Text = "";
                        SessionNotesRichtextBox.Text = "";
                        //SessionDatePicker.DisplayDate = DateTime.Now;
                        //SessionDatePicker.SelectedDate = DateTime.Now;
                        TemperatureIntegerUpDown.Value = 0;
                        FolderTextBox.Text = "";
                    }
                    catch (Exception ex)
                    {
                        Tools.ErrorLog(ex.Message);
                        Debug.WriteLine("RecordingSessionEditableControl-Clear RecordingSession:- " + ex);
                    }

                SetValue(recordingSessionProperty, value);
            }
        }

        public DateTime RecordingSessionStartDateTime { get; set; }

        private void Weather_weatherReceived(object sender, weatherEventArgs e)
        {
            if (appendWeather)
            {
                WeatherTextBox.Text = WeatherTextBox.Text + ": " + e.summary;
            }
            else
            {
                WeatherTextBox.Text = e.summary;
            }
        }

        #endregion recordingSession

        #region selectedFolder

        /// <summary>
        ///     selectedFolder Dependency Property
        /// </summary>
        public static readonly DependencyProperty selectedFolderProperty =
            DependencyProperty.Register(nameof(selectedFolder), typeof(string), typeof(RecordingSessionEditableControl),
                new FrameworkPropertyMetadata(""));

        /// <summary>
        ///     Gets or sets the selectedFolder property. This dependency property indicates ....
        /// </summary>
        public string selectedFolder
        {
            get => (string)GetValue(selectedFolderProperty);
            set => SetValue(selectedFolderProperty, value);
        }

        #endregion selectedFolder

        private void WeatherButton_Click(object sender, RoutedEventArgs e)
        {
            if (!recordingSession.hasGPSLocation) return;
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                appendWeather = true;
            }
            else
            {
                appendWeather = false;
            }

            Weather weather = new Weather();
            weather.weatherReceived += Weather_weatherReceived;
            TimeSpan weatherTime = getMidSessionTime(recordingSession);

            DateTime weatherDateTime = recordingSession.SessionDate.Date + weatherTime;

            var res = weather.GetWeatherHistory((double)recordingSession.LocationGPSLatitude,
                (double)recordingSession.LocationGPSLongitude, weatherDateTime);
        }
    }
}