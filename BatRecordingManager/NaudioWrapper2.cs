/*// *  Copyright 2016 Justin A T Halls
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
*/


using NAudio.Dsp;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.Devices;
using System.Runtime.CompilerServices;
using NAudio.Wave.SampleProviders;
using System.Windows.Forms;

namespace BatRecordingManager
{
	class NaudioWrapper2 : IDisposable
	{
		private readonly object _stoppedEventLock = new object();


		

		private bool _isDisposing;

		private WaveOut _player;

		private EventHandler _stoppedEvent;

		public VolumeSampleProvider _volumeProvider { get; set; } = null;

		public float Volume { get; set; } = 5.0f;

		
		
		private PlayListItem currentItem { get; set; }

		/// <summary>
		/// The speed factor to be used for playing from now.  The value may be a fraction
		/// <= 1.0m, 0.0m for heterodyne mode, or a negative value for broadband mode.
		/// </summary>
		public decimal currentSpeed { get; set; }

		/// <summary>
		/// The frequency to use of heterodyne mode in kHz
		/// </summary>
		public decimal Frequency { get; set; } = 50.0m;

		public bool PlayLooped { get; set; } = false;

		public PlaybackState playBackState
		{
			get
			{
				if (_player != null)
					return _player.PlaybackState;
				return PlaybackState.Stopped;
			}
		}

        public string SaveFileName { get; internal set; }

        /// <summary>
        ///     Tidy up before disposal
        /// </summary>
        public void Dispose()
		{
			if (!_isDisposing)
			{
				
				_isDisposing = true;
				if (_player != null && _player.PlaybackState == PlaybackState.Playing)
				{
					if (!Stop())
					{
						_player.Dispose();
						_player = null;
					}
				}



				CleanUp();
			}
		}

		public NaudioWrapper2()
		{
			
		}

		/// <summary>
		///     stops the player if it is playing
		/// </summary>
		/// <returns></returns>
		public bool Stop()
		{
			
			if (_player != null && _player.PlaybackState != PlaybackState.Stopped)
			{
				_player.Stop();
				return true;
			}

			return false;
		}

		private void CleanUp()
		{
			if (_player != null)
			{
				_player.Dispose();
				_player = null;
			}



			if (afr != null)
			{
				afr.Dispose();
				afr = null;
			}
			if (sampleProvider != null)
			{

				sampleProvider = null;
			}

			
			if (bufferedWaveProvider != null)
			{
				bufferedWaveProvider.ClearBuffer();
				bufferedWaveProvider = null;
			}

			if (_volumeProvider != null)
			{
				_volumeProvider = null;
			}

			if (_timer != null)
			{
				_timer.Stop();
				_timer.Dispose();
				_timer = null;
				
			}




		}



		public void PlayContinuous()
		{
			if (playableItem == null) return;

			CleanUp();

			if (!String.IsNullOrWhiteSpace(SaveFileName))
			{
				using (new WaitCursor())
				{
					SaveProcessedDataToFile(SaveFileName, playableItem);
					CleanUp();
					OnStopped(EventArgs.Empty);
					return;
				}
			}

			if (playableItem.playLength.TotalSeconds <= 2.0d && currentSpeed<=0.0m)
			{
				// process and play the entire item in one go with no breaks

				GetBuffer2();
				var reader = ProcessBuffer(playableItem.playLength.TotalSeconds);
				var player = new WaveOut();

                
                _volumeProvider = new VolumeSampleProvider(reader.ToSampleProvider());

                
                _volumeProvider.Volume = Volume;

                _player = new WaveOut();
				if (_player == null)
				{
					CleanUp();
					OnStopped(EventArgs.Empty);
					return;
				}


				_player.PlaybackStopped += Player_PlaybackStopped;
				_player.Init(_volumeProvider);
				_player.Play();

			}
			else
			{
				// Play using a BufferedWaveProvider to give continuous playback with the ability to
				// adjust some parameters, but not the output format

				GetBuffer2(); // initialises the AudioFileReader afr and positions it at the start of the playable section
				var reader = ProcessBuffer(2.0d);
				if (reader == null) return;

				bufferedWaveProvider = new BufferedWaveProvider(reader.WaveFormat);
				
				
				bufferedWaveProvider.BufferDuration = TimeSpan.FromSeconds(2.5);
				
				

				if (_volumeProvider == null)
				{
					_volumeProvider = new VolumeSampleProvider(bufferedWaveProvider.ToSampleProvider());
					
				}
				_volumeProvider.Volume = Volume;

                Byte[] buffer = new byte[reader.Length]; // which is 2 seconds from call to ProcessBuffer
                int Read = 0;
                Read = reader.Read(buffer, 0, buffer.Length);
                bufferedWaveProvider.AddSamples(buffer, 0, Read);

                _player = new WaveOut();
				if (_player == null)
				{
					CleanUp();
					OnStopped(EventArgs.Empty);
					return;
				}

				_timer = new Timer();
				_timer.Interval = (int)(1000);
				_timer.Tick += _timer_Tick;

				Debug.WriteLine($"BWP has capacity {bufferedWaveProvider.BufferDuration}, filled to {bufferedWaveProvider.BufferedDuration}");

				_player.PlaybackStopped += Player_PlaybackStopped;
				_player.Init(_volumeProvider);
				_player.Play();
				
				_timer.Start();

				OnPlayStarted(EventArgs.Empty);
			}
		}

		private TimeSpan readerChunkDuration = TimeSpan.FromSeconds(2.0d);

		

		private void _timer_Tick(object sender, EventArgs e)
		{
			
			
			if (bufferedWaveProvider != null)
			{
				//Debug.WriteLine($"Buffer contains {bufferedWaveProvider.BufferedDuration.TotalSeconds}seconds");
				

				if (bufferedWaveProvider.BufferedDuration.TotalMilliseconds < 1.0d)
				{
					Stop();
					return;
				}

				TimeSpan availableSpace = bufferedWaveProvider.BufferDuration - bufferedWaveProvider.BufferedDuration;

				var reader = ProcessBuffer(availableSpace.TotalSeconds);
				if (reader == null || !reader.HasData(1))
				{
					return;

				}

				if (_volumeProvider != null) _volumeProvider.Volume = Volume;

				Debug.WriteLine($"Volume requested/got = {Volume}/{_volumeProvider.Volume}");

				Byte[] buffer = new byte[reader.Length];
				int Read = 0;
				Read = reader.Read(buffer, 0, buffer.Length);
				if (Read > 0)
				{
					bufferedWaveProvider.AddSamples(buffer, 0, Read);
				}
				else
				{
					return; // and wait for the bufferedwaveprovider to be empty
				}


			}
		}

		Timer _timer = null;

		private BufferedWaveProvider bufferedWaveProvider = null;

		/// <summary>
		/// Loops filling and processing buffers and writing the result to the named file until there is no further data
		/// available, then closes the file and returns
		/// </summary>
		/// <param name="filename"></param>
		/// <exception cref="NotImplementedException"></exception>
		private void SaveProcessedDataToFile(string filename, PlayListItem itemToPlay)
		{
			try
			{
				var playLength = playableItem.playLength.TotalSeconds;
				GetBuffer2();
				if (currentSpeed > 0.0m)
				{
					playLength = playLength / (double)currentSpeed;
				}
				var reader = ProcessBuffer(playLength);
				using (WaveFileWriter _writer = new WaveFileWriter(filename, reader.WaveFormat))
				{

					byte[] buffer = new byte[4096];
					if (reader != null)
					{

						int read = -1;
						while ((read = reader.Read(buffer, 0, buffer.Length)) > 0)
						{
							_writer.Write(buffer, 0, read);
                            _writer.Flush();
                        }
						


					}
					_writer.Close();
				}
			}catch(Exception ex)
			{
				SaveFileName = "";
				PlayContinuous();
			}
		}


		public PlayListItem playableItem = null;

		/// <summary>
		/// Applies the process buffer according to the currentspeed, Frequency etc properties
		/// using the AudioFileReader afr which is pre-positioned at the start.  It will process
		/// and return as a reader a block of data no bigger than the duration parameter in seconds.
		/// </summary>
		/// <returns></returns>
		private WaveFileReader ProcessBuffer(double duration)
		{
			//if(reader == null) return null;
			if (afr == null || sampleProvider == null) return null;

			if (currentSpeed > 0.0m)
			{
				duration = duration * (double)currentSpeed;
			}

			WaveFileReader result = null;
			readerChunkDuration = TimeSpan.FromSeconds(duration);
			if (currentSpeed == 0.0m)
			{
				result = heterodyne(duration);
				//Debug.WriteLine($"Heterodyne {Frequency}kHz at {result.WaveFormat.SampleRate}");
			}
			else if (currentSpeed < 0.0m)
			{
				result = heterodyne(duration);
				//result = broadband(reader);
				//Debug.WriteLine($"Broadband out at {result.WaveFormat.SampleRate}");
			}

			else
			{
				var sr = sampleProvider.WaveFormat.SampleRate;
				WaveFormat wf = WaveFormat.CreateIeeeFloatWaveFormat((int)(sr * currentSpeed),
					sampleProvider.WaveFormat.Channels);
				var ms = new MemoryStream();
				if (wf != null && ms != null)
				{
					using (var wfw = new WaveFileWriter(new IgnoreDisposeStream(ms), wf))
					{
						if (wfw != null)
						{
							var floats = new float[(int)(sr * duration)];
							int read = -1;

							read = sampleProvider.Read(floats, 0, floats.Length);
							if (read > 0)
							{
								wfw.WriteSamples(floats, 0, read);
								wfw.Flush();

							}
							else
							{
								return (null);
							}
							ms.Position = 0;
							result = new WaveFileReader(ms);
							//Debug.WriteLine($"Processed out at {result.WaveFormat.SampleRate}");
							readerChunkDuration = result.TotalTime;
						}
					}

				}
			}
			return (result);
		}





		/// <summary>
		/// given an itemToPly and with a valid WaveFileReader in _readerArray[BufferToUse] the
		/// audio in the reader (assumed to be a small enough sample to process in one go) is 
		/// multiplied by a sine wave of _frequency (which is set according to the Frequency slider in the
		/// Player Window), and then low pass filtered at 5kHz.  Then it is written to a new
		/// WaveFileReader which is returned.
		/// </summary>
		/// <param name="reader">WaveFileReader from which to take audio data</param>
		/// <returns></returns>
		/// 
		private WaveFileReader heterodyne(double duration)
		{
			WaveFileReader result = null;
			if (sampleProvider == null) return (null);

			//Debug.WriteLine($"afr position={afr.Position} out of {afr.Length} = {(afr.Position*100)/afr.Length}%");

			var ms = new MemoryStream();
			using (var wfw = new WaveFileWriter(new IgnoreDisposeStream(ms), sampleProvider.WaveFormat))
			{
				if (wfw == null) return (null);
				var sineBuffer = new float[(int)(sampleProvider.WaveFormat.SampleRate * duration)];
				sineBuffer = Fill(sineBuffer, Frequency * 1000, sampleProvider.WaveFormat.SampleRate);

				var floats = new float[(int)(sampleProvider.WaveFormat.SampleRate * duration)];
				int read = -1;
				var filter = BiQuadFilter.LowPassFilter(wfw.WaveFormat.SampleRate, 5000, 2.0f);

				read = sampleProvider.Read(floats, 0, floats.Length);
				if (read > 0) {
					for (var i = 0; i < read; i++)
					{
						floats[i] = filter.Transform(floats[i] * sineBuffer[i]);
					}
					wfw.WriteSamples(floats, 0, read);
					wfw.Flush();

					//secondsRead += read / sampleProvider.WaveFormat.SampleRate;
					ms.Position = 0;
					result = new WaveFileReader(ms);
				}

			}


			return (result);
		}

		/// <summary>
		///    
		/// </summary>
		/// <param name="sineBuffer"></param>
		/// <param name="frequency"></param>
		/// <returns></returns>
		private float[] Fill(float[] sineBuffer, decimal frequency, int sampleRate)
		{
			if (currentSpeed < 0.0m) // broadband
			{
				int targetSampleRate = (int)((Frequency * 1000.0m) / 5.0m);
				int resampleGap = (int)(sampleRate / targetSampleRate);  // @384ksps => 38 SR=384000/38=10105

				for (int i = 0; i < sineBuffer.Length; i++)
				{
					sineBuffer[i] = 0.0f;
					if (i % resampleGap == 0) sineBuffer[i] = 50.0f;
				}

			}
			else
			{   // tuned
				for (var i = 0; i < sineBuffer.Length; i++)
				{
					sineBuffer[i] = 5.0f*(float)Math.Sin(2.0d * Math.PI * i * (double)frequency / sampleRate);
					//sineBuffer[i] = 10.0f * (float)Math.Sin(2.0d * Math.PI * i * (double)frequency );
				}
			}
			return sineBuffer;
		}



		private AudioFileReader afr = null;

		private ISampleProvider sampleProvider = null;

		/// <summary>
		/// Called once to prep and position the audioflereader and its associated sampleprovider
		/// </summary>
		private void GetBuffer2()
		{


			if (playableItem == null) return;
			if (string.IsNullOrWhiteSpace(playableItem.filename)) return;
			if (!File.Exists(playableItem.filename) || (new FileInfo(playableItem.filename).Length <= 0L)) return;

			if (afr == null)
			{
				afr = new AudioFileReader(playableItem.filename);
				Debug.WriteLine($"new afr position={afr.Position}bytes out of {afr.Length}bytes sample={afr.WaveFormat.AverageBytesPerSecond}");
				sampleProvider = afr.Skip(playableItem.startOffset).Take(playableItem.playLength);
			}

		}





		private void Player_PlaybackStopped(object sender, EventArgs e)
		{

			if (PlayLooped)
			{
				CleanUp();
				PlayContinuous();
				return;
			}
			else
			{
				PlayEnded();
				return;
			}



		}

		private void PlayEnded()
		{
			playableItem = null;
			CleanUp();
			OnStopped(EventArgs.Empty);
		}




		/// <summary>
		///     Event raised after the  property value has changed.
		/// </summary>
		public event EventHandler e_Stopped
        {
            add
            {
                lock (_stoppedEventLock)
                {
                    _stoppedEvent += value;
                }
            }
            remove
            {
                lock (_stoppedEventLock)
                {
                    _stoppedEvent -= value;
                }
            }
        }

        /// <summary>
        ///     Raises the <see cref="e_Stopped" /> event.
        /// </summary>
        /// <param name="e"><see cref="EventArgs" /> object that provides the arguments for the event.</param>
        protected virtual void OnStopped(EventArgs e)
        {
            EventHandler handler = null;

            lock (_stoppedEventLock)
            {
                handler = _stoppedEvent;

                if (handler == null)
                    return;
            }

            handler(this, e);
        }

		public event EventHandler<EventArgs> PlayStarted;

		protected virtual void OnPlayStarted(EventArgs e)=>PlayStarted?.Invoke(this, e);
    }

	
}
