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

namespace BatRecordingManager
{
	class NaudioWrapper2 : IDisposable
	{
		private readonly object _stoppedEventLock = new object();

		private WaveFormatConversionProvider _converter;
		private bool _doLoop;

		private bool _isDisposing;

		private WaveOut _player;
		private WaveFileReader[] _readerArray = new WaveFileReader[2];
		private MediaFoundationResampler[] _resamplerArray = new MediaFoundationResampler[2];
		private EventHandler _stoppedEvent;
		private AudioFileReader _wave;
		private int BufferToUse = 0;
		private string filename = "";
		private bool doSave = false;
		

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

		/// <summary>
		///     Tidy up before disposal
		/// </summary>
		public void Dispose()
		{
			if (!_isDisposing)
			{
				_doLoop = false;
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

		/// <summary>
		///     stops the player if it is playing
		/// </summary>
		/// <returns></returns>
		public bool Stop()
		{
			_doLoop = false;
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

			if (_readerArray != null)
			{

				if (_readerArray[0] != null && !_doLoop)
				{
					_readerArray[0].Dispose();
					_readerArray[0] = null;
				}

				if (_readerArray[1] != null && !_doLoop)
				{
					_readerArray[1].Dispose();
					_readerArray[1] = null;
				}


			}



			if (_converter != null)
			{
				_converter.Dispose();
				_converter = null;
			}

			if (_wave != null)
			{
				_wave.Dispose();
				_wave = null;
			}

			if (_resamplerArray != null && _readerArray.Length > 0)
			{
				_resamplerArray[0]?.Dispose();
				_resamplerArray[0] = null;

				_resamplerArray[1]?.Dispose();
				_resamplerArray[1] = null;
			}

			
		}

		/// <summary>
		/// Plays playListItem at a specified speed.
		/// If speed is 0.0m then use broadband mode.
		/// If speed is negative use Heterodyne mode
		/// </summary>
		/// 
		public void Play(string filename = "")
		{
			CleanUp();


			this.filename = filename;
			if (!string.IsNullOrWhiteSpace(filename) && Directory.Exists(Path.GetDirectoryName(filename)))
			{
				_doLoop = false;
				doSave = true;
			}
			else
			{
				doSave = false;
			}

			if (playableItem == null)
			{
				OnStopped(EventArgs.Empty);
				return;
			}

			if (_readerArray == null) _readerArray = new WaveFileReader[2];


			BufferToUse = 0;

			secondsPlayed = 0;



			if (doSave)
			{
				SaveProcessedDataToFile(filename,playableItem);
				return;

			}

			_readerArray[BufferToUse] = GetBuffer(playableItem);


			_readerArray[BufferToUse] = ProcessBuffer(playableItem, currentSpeed);

			nextBufferReady = false;

			_player = new WaveOut();
			if (_player == null)
			{
				CleanUp();
				OnStopped(EventArgs.Empty);
				return;
			}


			_player.PlaybackStopped += Player_PlaybackStopped;
			_player.Init(_readerArray[BufferToUse]);
			_player.Play();


			BufferToUse++;
			if (BufferToUse > 1) BufferToUse = 0;

			_readerArray[BufferToUse] = GetBuffer(playableItem);
			_readerArray[BufferToUse] = ProcessBuffer(playableItem, currentSpeed);

			nextBufferReady = true;

		}

		/// <summary>
		/// Loops filling and processing buffers and writing the result to the named file until there is no further data
		/// available, then closes the file and returns
		/// </summary>
		/// <param name="filename"></param>
		/// <exception cref="NotImplementedException"></exception>
		private void SaveProcessedDataToFile(string filename, PlayListItem itemToPlay)
		{
			decimal speed = currentSpeed; // since it must not change once started
			WaveFileReader reader = GetBuffer(playableItem);
			reader = ProcessBuffer(reader, itemToPlay, speed);
			using (WaveFileWriter _writer = new WaveFileWriter(filename, reader.WaveFormat))
			{

				byte[] buffer = new byte[1024];
				while (reader != null)
				{

					int read = -1;
					while ((read =reader.Read(buffer, 0, buffer.Length)) > 0)
					{
						_writer.Write(buffer, 0, read);
					}
					_writer.Flush();
					reader=GetBuffer(playableItem);
					reader = ProcessBuffer(reader, playableItem, speed);

				}
				_writer.Close();
			}
		}
		

		public PlayListItem playableItem = null;

		/// <summary>
		/// Given a WavFileReader, and a speed setting, processes the data from the reader and returns a 
		/// new reader that will access the modified data.
		/// The speed may be:-
		/// a positive fraction - in which case that will be the speed reduction for replay.  i.e. a value
		///		of 0.1 will replay the buffer at one tenth speed
		///	a value of zero - in which the buffer will be heterodyned with a sine wave at _frequency and then
		///		low pass filtered and returned at a lower sample rate.
		///	a negative value - in which case the file will be re-sampled at  10kHz, then low pass filtered and
		///		returned at a lower sample rate.  This is the same as a broad band bat detector, or multiple
		///		tuned bat detectors tuned at 10kHz intervals.  The 10kHz rate will be adjusted by upt to 5kHz 
		///		(+/-2.5kHz) to allow tuning to avaid nulls or provide a better listening experience.  The offset will
		///		be derived from _frequency which is remotely set from the frequency slider.
		/// </summary>
		/// <param name="itemToPlay"></param>
		/// <param name="currentSpeed"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		private WaveFileReader ProcessBuffer(PlayListItem itemToPlay, decimal currentSpeed)
		{
			if (itemToPlay == null) return (null);
			if (BufferToUse < 0 || BufferToUse > 1) return (null);
			if (_readerArray == null || _readerArray[BufferToUse] == null) return (null);
			var reader = _readerArray[BufferToUse];
			return (ProcessBuffer(reader, itemToPlay, currentSpeed));
		}

		private WaveFileReader ProcessBuffer(WaveFileReader reader,PlayListItem itemToPlay,decimal currentSpeed)
		{ 
			if(reader == null) return null;

			Debug.WriteLine($"Process buffer {BufferToUse} at {reader.WaveFormat.SampleRate} for speed {currentSpeed}");


			WaveFileReader result = null;
			if (currentSpeed == 0.0m) {
				result = heterodyne(reader);
				Debug.WriteLine($"Heterodyne {Frequency}kHz at {result.WaveFormat.SampleRate}");
			}
			else if (currentSpeed < 0.0m) {
				result = broadband(reader);
				Debug.WriteLine($"Broadband out at {result.WaveFormat.SampleRate}");
			}

			else
			{
				var sr = reader.WaveFormat.SampleRate;
				WaveFormat wf = WaveFormat.CreateIeeeFloatWaveFormat((int)(sr * currentSpeed),
					reader.WaveFormat.Channels);
				var ms = new MemoryStream();
				if(wf!=null && ms != null)
				{
					using (var wfw = new WaveFileWriter(new IgnoreDisposeStream(ms), wf))
					{
						if (wfw != null)
						{
							var floats=new float[reader.SampleCount];
							int read = -1;
							
							while ((read = reader.ToSampleProvider().Read(floats, 0, floats.Length))>0)
							{
								wfw.WriteSamples(floats, 0, read);
								wfw.Flush();
							}
						}
					}
					ms.Position = 0;
					result = new WaveFileReader(ms);
					Debug.WriteLine($"Processed out at {result.WaveFormat.SampleRate}");
				}
			}
			return (result);
		}

        /// <summary>
		/// Similar in concept to heterodyne, but actually resamples the audio contained in the reader
		/// at 10kHz+- a fiddle factor derived from Frequency.  The low pass filter and return in the guise of a new
        ///    WaveFileReader.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		/// 
        ///    
        private WaveFileReader broadband(WaveFileReader reader)
		{
			if (reader == null) return null;
			WaveFileReader result = null;

			int targetSampleRate = (int)(Frequency * 1000) / 5;
			int resampleGap = (int)(reader.WaveFormat.SampleRate / targetSampleRate);  // @384ksps => 38 SR=384000/38=10105
			int newSampleRate = (int)(reader.WaveFormat.SampleRate / resampleGap);
			Debug.WriteLine($"Broadband target rate={targetSampleRate}, resampleGap={resampleGap}, new SR={newSampleRate}");

			var ms = new MemoryStream();

            var outFormat = WaveFormat.CreateIeeeFloatWaveFormat(newSampleRate,
                            reader.WaveFormat.Channels);


			//var resampler = new MediaFoundationResampler(reader, outFormat);
			//var resampler = new WdlResamplingSampleProvider(reader.ToSampleProvider(), newSampleRate);
			

            using (var wfw = new WaveFileWriter(new IgnoreDisposeStream(ms), outFormat))
            {
                if (wfw == null) return (null);
				
               

                var floats = new float[reader.WaveFormat.SampleRate];
				var resampled = new float[newSampleRate];
                int read = -1;
                var filter = BiQuadFilter.LowPassFilter(newSampleRate, 5000, 2.0f);

                while ((read = reader.ToSampleProvider().Read(floats, 0, floats.Length)) > 0)
                {
					int resampledPos = 0;
					for(int i = 0; i < read; i += resampleGap)
					{
						if (resampledPos < resampled.Length)
						{
							resampled[resampledPos++] = filter.Transform(floats[i]);
						}
					}

                    //for (var i = 0; i < read; i++)
                    //{
                    //    floats[i] = filter.Transform(floats[i]);
                    //}
                    wfw.WriteSamples(resampled, 0,resampledPos );
                    wfw.Flush();
                }
                ms.Position = 0;


                result = new WaveFileReader(ms);
				

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
		private WaveFileReader heterodyne(WaveFileReader reader)
		{
			WaveFileReader result = null;
			if (reader == null) return (null);
			

			var ms = new MemoryStream();
			using (var wfw = new WaveFileWriter(new IgnoreDisposeStream(ms), reader.WaveFormat))
			{
				if (wfw == null) return (null);
				var sineBuffer = new float[reader.SampleCount];
				sineBuffer = Fill(sineBuffer, Frequency * 1000);

				var floats= new float[reader.SampleCount];
				int read = -1;
				var filter = BiQuadFilter.LowPassFilter(wfw.WaveFormat.SampleRate, 5000, 2.0f);

				while ((read = reader.ToSampleProvider().Read(floats, 0, floats.Length)) > 0)
				{
					for(var i = 0; i < read; i++)
					{
						floats[i] = filter.Transform(floats[i] * sineBuffer[i]);
					}
					wfw.WriteSamples(floats, 0, read);
					wfw.Flush();
				}
				ms.Position = 0;
				result=new WaveFileReader(ms);

			}


			return (result);
		}

        /// <summary>
        ///    
        /// </summary>
        /// <param name="sineBuffer"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        private float[] Fill(float[] sineBuffer, decimal frequency)
        {
            for (var i = 0; i < sineBuffer.Length; i++)
                sineBuffer[i] = (float)Math.Sin(2.0d * Math.PI * i * (double)frequency / sineBuffer.Length);
            return sineBuffer;
        }

        private int secondsPlayed = 0;

		/// <summary>
		/// Uses the itemToPlay and locally stored information to generate a WavFileReader which will
		/// return a data stream from the specified file, with a duration not exceeding a fixed buffer size
		/// that will be processed and played.  The data stream may be less than the 'normal' size if it is the last 
		/// buffer full in the file.  If the end of the file has already been reached, then the function returns a null.
		/// </summary>
		/// <param name="itemToPlay"></param>
		/// <returns></returns>
		/// <exception cref="NotImplementedException"></exception>
		private WaveFileReader GetBuffer(PlayListItem itemToPlay,[CallerMemberName] string caller = null, [CallerLineNumber] int linenumber = 0)
		{
			WaveFileReader result = null;
			MemoryStream ms = null;

            if (itemToPlay == null) return null;
            if (string.IsNullOrWhiteSpace(itemToPlay.filename)) return null;
            if (!File.Exists(itemToPlay.filename) || (new FileInfo(itemToPlay.filename).Length <= 0L)) return null;
			int startSecs = (int)itemToPlay.startOffset.TotalSeconds + secondsPlayed;
			using (var afr = new AudioFileReader(itemToPlay.filename))
			{
				if (afr != null)
				{
					afr.Skip((int)startSecs);

					if (secondsPlayed >= itemToPlay.playLength.TotalSeconds)
					{
						return null; // we have reached the end of the file
					}
					secondsPlayed++;

					var buffer = afr.Take(TimeSpan.FromSeconds(1));
					var read = -1;
					ms = new MemoryStream();
					using (var wfw = new WaveFileWriter(new IgnoreDisposeStream(ms), buffer.WaveFormat))
					{
						var floats=new float[buffer.WaveFormat.SampleRate];
						if ((read=buffer.Read(floats, 0, floats.Length)) > 0)
						{
							wfw.WriteSamples(floats, 0, read);
							wfw.Flush();
						}
						else
						{
							return null;
						}
					}


				}
			}

			if (ms != null)
			{
				ms.Position = 0;
				result = new WaveFileReader(ms);
				Debug.WriteLine($"    From {caller} at line {linenumber}");
				Debug.WriteLine($"Filled buffer from {TimeSpan.FromSeconds(startSecs)} at rate {result.WaveFormat.SampleRate} of {result.TotalTime}");
			}


            return	result;
		}

		private bool nextBufferReady = true;

		private void Player_PlaybackStopped(object sender,EventArgs e)
		{
			if (nextBufferReady && playableItem!=null)
			{
				nextBufferReady = false;
				if (_readerArray[BufferToUse] == null)
				{
					if (PlayLooped)
					{
						Play();
						return;
					}
					else
					{
						PlayEnded();
						return;
					}
				}
				_player.Init(_readerArray[BufferToUse]);
				_player.Play();

				if (BufferToUse == 0) BufferToUse = 1;
				else BufferToUse = 0;
				_readerArray[BufferToUse] = GetBuffer(playableItem);
				_readerArray[BufferToUse] = ProcessBuffer(playableItem, currentSpeed);
				nextBufferReady = true;
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
    }
}
