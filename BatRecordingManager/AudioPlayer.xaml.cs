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


using NAudio.Wave;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
            var pli = PlayListItem.Create(filename);
            pli.startOffset = start;
            pli.playLength = duration;
            pli.label = comment;
            pli.segment = segmentToAdd;
            pli.segmentDuration=segmentToAdd.Duration()??new TimeSpan();
            pli.segmentOffset = segmentToAdd.StartOffset;
            var sonagramGenerator = new SegmentSonagrams();
            var si=sonagramGenerator.GenerateForSegment(ref segmentToAdd, display: SegmentSonagrams.DisplayMode.ALWAYS);
            SpectrogramWindow.Display(sonagramGenerator,pli);

            if(si!=null)
                pli.hasSpectrogram = true;
            else
                pli.hasSpectrogram = false;
            AddToPlayList(pli);

            pli.ItemClosed += Pli_ItemClosed;

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
        public bool AddToPlayList(string filename, TimeSpan start, TimeSpan duration, 
            TimeSpan segmentOffset, TimeSpan segmentDuration, TimeSpan fileDuration, string label,LabelledSegment segment)
        {
            var pli = PlayListItem.Create(filename, start, duration,segmentOffset,segmentDuration, fileDuration, label,segment);
            if (pli != null)
            {
                pli.ItemClosed += Pli_ItemClosed;
                return AddToPlayList(pli);
            }
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
            pli.ItemClosed += Pli_ItemClosed;
            if (!pli.hasSpectrogram)
            {
                
                var sgi = new SegmentSonagrams();
                StoredImage si = null;
                if (pli.segment != null && pli.segment.Recording != null)
                {
                    var seg = pli.segment;
                    si = sgi.GenerateForSegment(ref seg, display: SegmentSonagrams.DisplayMode.ALWAYS);
                }
                else
                {
                    si = sgi.GenerateForFile(pli.filename, 0.0d, pli.fileDuration.TotalSeconds,display:SegmentSonagrams.DisplayMode.ALWAYS);
                }
                if (si != null)
                {
                    pli.hasSpectrogram = true;
                    SpectrogramWindow.Display(segmentSonagram:sgi, pli: pli);
                }
                
            }
            SetButtonVisibility();
            this.BringIntoView();
            this.Focus();
            return true;
        }

        private void Pli_ItemClosed(object sender, EventArgs e)
        {
            var pli = sender as PlayListItem;
            if (pli != null)
            {
                PlayList.Remove(pli);
                this.BringIntoView();
                this.Focus();
            }
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
                _wrapper.PlayStarted += _wrapper_PlayStarted;
                _wrapper.currentSpeed = 0.0m;
                _wrapper.Frequency = (decimal)Frequency;
                if (BroadbandButton.IsChecked ?? false) _wrapper.currentSpeed = -1.0m;
                else if (TenthButton.IsChecked ?? false) _wrapper.currentSpeed = 0.1m;
                else if (FifthButton.IsChecked ?? false) _wrapper.currentSpeed = 0.2m;
                else if (TwentiethButton.IsChecked ?? false) _wrapper.currentSpeed = 0.05m;
                else if (FullSpeedButton.IsChecked ?? false) _wrapper.currentSpeed = 1.0m;
                _wrapper.playableItem = itemToPlay;
                _wrapper.PlayLooped = playLooped;
                _wrapper.SaveFileName = filename;
                _wrapper.Volume = getLogVol((float)VolumeSlider.Value);
                _wrapper.PlayContinuous();
               // _wrapper.Play(filename);
            }


        }

        private void _wrapper_PlayStarted(object sender, EventArgs e)
        {
            var playingItem = GetItemToPlay();
            playingItem.OnPlayStarted(EventArgs.Empty);

            
        }

        private void Wrapper_Stopped(object sender, EventArgs e)
        {
            
            var PlayingItem = GetItemToPlay();
            PlayingItem.OnPlayEnded(EventArgs.Empty);
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

            for(int i=PlayList.Count-1; i>=0; i--)
            {
                PlayList[i]?.Close();
                //PlayList.RemoveAt(i);
            }

            //PlayList?.Clear();
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
                    using (new WaitCursor())
                    {
                        var pli = PlayListItem.Create(file);
                        if (pli != null) AddToPlayList(pli);
                    }
                }
            }
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            float logVol = getLogVol((float)VolumeSlider.Value);

            if (_wrapper != null)
            {
                if (_wrapper._volumeProvider != null)
                {
                    _wrapper.Volume = logVol;
                    _wrapper._volumeProvider.Volume = logVol;
                    //Debug.WriteLine($"Vol = {logVol}");
                }
            }
        }

        private float getLogVol(float value)
        {
            return (0.04f * (float)Math.Exp(0.7d * value));
        }

        public event EventHandler<EventArgs> PlayStarted;
        protected virtual void OnPlayStarted(EventArgs e) => PlayStarted?.Invoke(this, e);

        public event EventHandler<EventArgs> PlayEnded;
        protected virtual void OnPlayEnded(EventArgs e) => PlayEnded?.Invoke(this, e);
    }



    /// <summary>
    ///     A class to hold items to be displayed in the AudioPlayer playlist
    /// </summary>
    public class PlayListItem:INotifyPropertyChanged
    {
        /// <summary>
        ///     fully qualified name of the source .wav file
        /// </summary>
        public string filename { get; set; }

        public bool hasSpectrogram { get; set; } = false;

        public TimeSpan segmentOffset { get; set; } = new TimeSpan();

        public TimeSpan segmentDuration { get; set; } = new TimeSpan();

        private TimeSpan _startOffset = new TimeSpan();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName="")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void setStartAndLength(double startOffset,double duration)
        {
            Locked = true;
            this.startOffset = TimeSpan.FromSeconds(startOffset) + segmentOffset;
            this.playLength = TimeSpan.FromSeconds(duration);
            Locked = false;
        }
        /// <summary>
        ///     offset in the file for the start of the region to be played
        /// </summary>
        public TimeSpan startOffset 
        {
            get
            {
                if (_startOffset > fileDuration - _playLength)
                {
                    _startOffset = fileDuration - _playLength;
                    if (_startOffset.Ticks < 0) _startOffset = new TimeSpan();
                }
                return _startOffset;
            }
            set
            {
                _startOffset = value;
                OnPropertyChanged(nameof(startOffset));
                if(!Locked)
                    OnSegmentChanged(EventArgs.Empty);
            }
        }

        private TimeSpan _playLength = new TimeSpan();
        /// <summary>
        ///     duration of the segment to be played
        /// </summary>
        public TimeSpan playLength 
        { 
            get 
            {
                if (_playLength + _startOffset > fileDuration)
                {
                    _playLength = fileDuration - _startOffset;
                    if(_playLength.Ticks < 0) _playLength = new TimeSpan();
                }
                return _playLength;
            } 
            set 
            { 
                _playLength = value;
                OnPropertyChanged(nameof(playLength));
                if(!Locked)
                    OnSegmentChanged(EventArgs.Empty);
            } 
        }

        private TimeSpan _fileDuration = new TimeSpan();
        /// <summary>
        /// The full length of the file refferred to
        /// </summary>
        public TimeSpan fileDuration 
        {
            get
            {
                return _fileDuration;
            }
            set
            {
                _fileDuration = value;
            }
        }

        /// <summary>
        ///     label of the original labelled segment or other comment for the playlist display
        /// </summary>
        public string label { get; set; }
        public LabelledSegment segment { get; internal set; }

        /// <summary>
        ///     Constructor for playlist elements
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="start"></param>
        /// <param name="duration"></param>
        /// <param name="label"></param>
        public static PlayListItem Create(string filename, TimeSpan start, TimeSpan duration, 
            TimeSpan segmentOffset, TimeSpan segmentDuration, TimeSpan fileDuration, string label,LabelledSegment segment,bool hasSpectrogram=false)
        {
            if (string.IsNullOrWhiteSpace(filename)) return null;
            if (!File.Exists(filename) || (new FileInfo(filename).Length <= 0L)) return null;
            var result = new PlayListItem
            {
                filename = filename,
                startOffset = start,
                playLength = duration,
                segmentOffset= segmentOffset,
                segmentDuration= segmentDuration,
                fileDuration = fileDuration,
                label = label,
                segment=segment,
                hasSpectrogram=hasSpectrogram
            };


            return result;
        }

        private bool Locked = false;

        public static PlayListItem Create(string filename)
        {
            WaveFileReader wfr=new WaveFileReader(filename);

            TimeSpan defLength = wfr.TotalTime;

            if(wfr.TotalTime<defLength)defLength= wfr.TotalTime;

            return(PlayListItem.Create(filename,start: new TimeSpan(),duration:defLength,
                segmentOffset: new TimeSpan(),segmentDuration: defLength,
                fileDuration: wfr.TotalTime,label: filename,segment: null));
        }

        public event EventHandler<EventArgs> SegmentChanged;

        protected virtual void OnSegmentChanged(EventArgs e) => SegmentChanged?.Invoke(this, e);

        internal void Close()
        {
            OnItemClosed(EventArgs.Empty);
        }

        public event EventHandler<EventArgs> ItemClosed;

        protected virtual void OnItemClosed(EventArgs e) => ItemClosed?.Invoke(this, e);

        public event EventHandler<EventArgs> PlayStarted;
        public virtual void OnPlayStarted(EventArgs e) => PlayStarted?.Invoke(this, e);

        public event EventHandler<EventArgs> PlayEnded;
        public virtual void OnPlayEnded(EventArgs e) => PlayEnded?.Invoke(this, e);


    }

    


}