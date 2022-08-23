﻿// *  Copyright 2016 Justin A T Halls
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


using NAudio.Wave;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;




namespace BatRecordingManager
{
    /// <summary>
    ///     Interaction logic for AudioPlayerIK.xaml
    /// </summary>
    public partial class AudioPlayer : Window
    {
        private NaudioWrapper2 _wrapper;

        /// <summary>
        ///     Constructor for the AudioPlayer
        /// </summary>
        public AudioPlayer()
        {
            Loaded += AudioPlayer_Loaded;
            InitializeComponent();
            DataContext = this;

            Closing += AudioPlayer_Closing;
            SetButtonVisibility();
            //PlayListItem pli = PlayListItem.Create(@"X:\BatRecordings\2018\Knebworth-KNB18-2_20180816\KNB18-2p_20180816\KNB18-2p_20180816_212555.wav", TimeSpan.FromSeconds(218), TimeSpan.FromSeconds(8), "Comment line");
            //PlayList.Add(pli);
            
        }

        private void AudioPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            this.BringIntoView();
            this.Focus();
        }


        /// <summary>
        ///     List of items that can be played if selected
        /// </summary>
        public ObservableCollection<PlayListItem> PlayList { get; set; } =
            new ObservableCollection<PlayListItem>();

        /// <summary>
        ///     Adds a specific labelled segment to the playlist
        /// </summary>
        /// <param name="segmentToAdd"></param>
        public int AddToList(LabelledSegment segmentToAdd)
        {
            if (PlayList == null) PlayList = new ObservableCollection<PlayListItem>();
            var filename = segmentToAdd.Recording.GetFileName();
            if (string.IsNullOrWhiteSpace(filename))
            {
                MessageBox.Show("No file found on this computer for this segment");
                return PlayList.Count;
            }

            var start = segmentToAdd.StartOffset;
            var duration = segmentToAdd.Duration() ?? new TimeSpan();
            var comment = segmentToAdd.Comment;
            var pli = PlayListItem.Create(filename, start, duration, comment);
            AddToPlayList(pli);


            return PlayList.Count;
        }

        private void SetButtonVisibility()
        {
            if (PlayList == null || PlayList.Count <= 0)
            {
                PlayButton.IsEnabled = false;
                PlayLoopedButton.IsEnabled = false;
            }
            else
            {
                PlayButton.IsEnabled = true;
                PlayLoopedButton.IsEnabled = true;
            }
        }

        /// <summary>
        ///     event handler triggered when the window is closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AudioPlayer_Closing(object sender, CancelEventArgs e)
        {
            if (_wrapper != null)
            {
                _wrapper.Dispose();
                var i = 0;
                while (_wrapper != null && _wrapper.playBackState != PlaybackState.Stopped)
                {
                    Thread.Sleep(100);
                    if (i++ > 10) _wrapper = null;
                }
            }
        }

        /// <summary>
        ///     constructs a playlistitem and adds it to the playlist
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="start"></param>
        /// <param name="duration"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public bool AddToPlayList(string filename, TimeSpan start, TimeSpan duration, string label)
        {
            var pli = PlayListItem.Create(filename, start, duration, label);
            if (pli != null) return AddToPlayList(pli);
            this.BringIntoView();
            this.Focus();
            return false;
        }

        /// <summary>
        ///     Adds a pre-constructed playlistitem to the playlist
        /// </summary>
        /// <param name="pli"></param>
        /// <returns></returns>
        public bool AddToPlayList(PlayListItem pli)
        {
            if (pli == null) return false;
            PlayList.Add(pli);
            SetButtonVisibility();
            this.BringIntoView();
            this.Focus();
            return true;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            string filename = "";
            var looped = (sender as Button).Content as string == "LOOP";
            if ((sender as Button).Content as string == "SAVE")
            {
                filename = Tools.GetFileToWriteTo("", ".wav");
            }
            if (PlayButton.Content as string == "PLAY" || (sender as Button).Content as String == "SAVE")
            {
                PlayListItem itemToPlay = GetItemToPlay();

                if (itemToPlay != null)
                {
                    if (string.IsNullOrWhiteSpace(filename))
                    {
                        PlayButton.Content = "STOP";
                    }
                    else
                    {
                        StopPlaying(); // we are going to save, so force a stop incase sometning is already playing
                    }
                    PlayItem(itemToPlay, looped, filename);
                    
                }
            }
            else
            {
                StopPlaying();
            }
        }

        private void StopPlaying()
        {
            if (_wrapper != null)
            {
                _wrapper.Stop();
                if (_wrapper.playBackState == PlaybackState.Stopped)
                {
                    _wrapper.Dispose();
                    _wrapper = null;
                    PlayButton.Content = "PLAY";
                }
            }
        }

        private PlayListItem GetItemToPlay()
        {
            PlayListItem item = null;
            if (!PlayList.IsNullOrEmpty())
            {
                if (PlayListDatagrid.SelectedItem != null)
                    item = PlayListDatagrid.SelectedItem as PlayListItem;
                else
                    item = PlayList.First();
            }

            return (item);
        }

        private void PlayItem(PlayListItem itemToPlay, bool playLooped, string filename)
        {
           
            if (_wrapper == null)
            {
                _wrapper = new NaudioWrapper2();
                _wrapper.e_Stopped += Wrapper_Stopped;
                _wrapper.currentSpeed = 0.0m;
                _wrapper.Frequency = (decimal)Frequency;
                if (BroadbandButton.IsChecked ?? false) _wrapper.currentSpeed = -1.0m;
                else if (TenthButton.IsChecked ?? false) _wrapper.currentSpeed = 0.1m;
                else if (FifthButton.IsChecked ?? false) _wrapper.currentSpeed = 0.2m;
                else if (TwentiethButton.IsChecked ?? false) _wrapper.currentSpeed = 0.05m;
                else if (FullSpeedButton.IsChecked ?? false) _wrapper.currentSpeed = 1.0m;
                _wrapper.playableItem = itemToPlay;
                _wrapper.PlayLooped = playLooped;
                _wrapper.PlayContinuous();
               // _wrapper.Play(filename);
            }


        }

       

        private void Wrapper_Stopped(object sender, EventArgs e)
        {
            if (_wrapper != null)
            {
                _wrapper.Dispose();
                _wrapper = null;
            }

            PlayButton.Content = "PLAY";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_wrapper != null)
            {
                _wrapper.Dispose();
                _wrapper = null;
            }

            PlayButton.Content = "PLAY";
            Close();
        }

        private void FrequencySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_wrapper != null) _wrapper.Frequency = (decimal)(sender as Slider).Value;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (_wrapper != null)
            {
                _wrapper.Dispose();
                _wrapper = null;
            }

            PlayList?.Clear();
            ShowInTaskbar = true;
            WindowState = WindowState.Minimized;
            PlayButton.Content = "PLAY";
            e.Cancel = true;
        }

        internal void Stop()
        {
            _wrapper?.Stop();
        }

        #region

        /// <summary>
        ///     Frequency Dependency Property
        /// </summary>
        public static readonly DependencyProperty FrequencyProperty =
            DependencyProperty.Register(nameof(Frequency), typeof(double), typeof(AudioPlayer),
                new FrameworkPropertyMetadata(50.0d));

        /// <summary>
        ///     Gets or sets the Frequency property.  This dependency property
        ///     indicates ....
        /// </summary>
        public double Frequency
        {
            get => (double)GetValue(FrequencyProperty);
            set
            {
                if (_wrapper != null) _wrapper.Frequency = (decimal)value;
                SetValue(FrequencyProperty, value);
            }
        }


        #endregion

        /// <summary>
        /// Produces an Open file dialog to select any wav file to be played.  The details are
        /// prseented as usual.  Eventually there wil be an option to only play a portion of the file.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileButton_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string file=Tools.SelectWavFileFolder(path,selectFile:true);
            if (!string.IsNullOrWhiteSpace(file))
            {
                if (File.Exists(file))
                {
                    var pli = PlayListItem.Create(file);
                    if (pli != null) AddToPlayList(pli);
                }
            }
        }
    }

    /// <summary>
    ///     A class to hold items to be displayed in the AudioPlayer playlist
    /// </summary>
    public class PlayListItem
    {
        /// <summary>
        ///     fully qualified name of the source .wav file
        /// </summary>
        public string filename { get; set; }

        /// <summary>
        ///     offset in the file for the start of the region to be played
        /// </summary>
        public TimeSpan startOffset { get; set; }

        /// <summary>
        ///     duration of the segment to be played
        /// </summary>
        public TimeSpan playLength { get; set; }

        /// <summary>
        ///     label of the original labelled segment or other comment for the playlist display
        /// </summary>
        public string label { get; set; }

        /// <summary>
        ///     Constructor for playlist elements
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="start"></param>
        /// <param name="duration"></param>
        /// <param name="label"></param>
        public static PlayListItem Create(string filename, TimeSpan start, TimeSpan duration, string label)
        {
            if (string.IsNullOrWhiteSpace(filename)) return null;
            if (!File.Exists(filename) || (new FileInfo(filename).Length <= 0L)) return null;
            var result = new PlayListItem
            {
                filename = filename,
                startOffset = start,
                playLength = duration,
                label = label
            };


            return result;
        }

        public static PlayListItem Create(string filename)
        {
            WaveFileReader wfr=new WaveFileReader(filename);

            TimeSpan defLength = TimeSpan.FromSeconds(10);

            if(wfr.TotalTime<defLength)defLength= wfr.TotalTime;

            return(PlayListItem.Create(filename,new TimeSpan(),defLength,filename));
        }
    }
}