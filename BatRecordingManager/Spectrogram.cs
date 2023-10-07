using FftSharp;
using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using Spectrogram;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.TextFormatting;
using FFTW;
using FFTW.NET;
using Complex = NAudio.Dsp.Complex;

namespace BatRecordingManager
{
    /// <summary>
    /// given a file and start and end points, generates 5s spectrograms asynchronously
    /// returning the spectrograms in chunks via a callback.
    /// </summary>
    internal class Spectrogram
    {
        public string fileName { get; set; }
        public TimeSpan start { get; set; }
        public TimeSpan end { get; set; }
        private int FFTSize { get; set; }

        public int FFTAdvance { get; set; }

        public int sampleRate { get; set; } = 384000;

        /// <summary>
        /// frequency of a high pass filter to be used in Hz
        /// </summary>
        public int HighPassCutoff { get; set; }

        /// <summary>
        /// frequency of a low pass filter to be used in Hz
        /// </summary>
        public int LowPassCutOff { get; set; }

        /// <summary>
        /// Q of the high and low pass filters to be used
        /// </summary>
        public int FilterQ { get; set; } = 1;
        internal Spectrogram(string filename, TimeSpan start, TimeSpan end, int FFTSize, int FFTAdvance)
        {
            if (File.Exists(filename))
            {
                this.fileName = filename;
            }
            else
            {
                this.fileName = "";
                Debug.WriteLine($"File {filename} does not exist");
            }
            this.start = start;
            this.end = end;
            if (start == end) fileName = "";
            if (start > end)
            {
                var temp = start;
                start = end;
                end = temp;
            }
            this.FFTSize = FFTSize;
            this.FFTAdvance = FFTAdvance;
        }

        private ISampleProvider sampleProvider;


        public Task<int> GetSpectrogramAsync() => Task.Run(() =>
        {
            Debug.WriteLine("@@@@ ++++ GetSpectrogramAsync...");
            int errcode = 0;
            if (string.IsNullOrWhiteSpace(this.fileName))
            {
                Debug.WriteLine($"@@@@ ++++ GetSpectrogramAsync has no filename [{fileName}] to work with");
                return -1;
            }
            using (var wfr = new AudioFileReader(fileName))
            {
                if (wfr == null) return (-3);
                sampleRate = wfr.WaveFormat.SampleRate;
                var duration = end - start;
                double durSecs = duration.TotalSeconds;
                var samples = durSecs * sampleRate;
                int totalFFTs = (int)Math.Floor((samples - FFTSize) / FFTAdvance);
                int height = FFTSize / 2;

                if (totalFFTs > 32768 || totalFFTs*height>10_000_000)
                {
                    int maxFFTs = (10_000_000 / height)-1;
                    if (maxFFTs > 32768) maxFFTs = 32768;
                    durSecs = (FFTAdvance * maxFFTs) / (double)sampleRate;
                    duration = TimeSpan.FromSeconds(durSecs);
                    samples = durSecs * sampleRate;
                    totalFFTs = (int)Math.Floor((samples - FFTSize) / FFTAdvance);
                }


                sampleProvider = wfr.ToSampleProvider().Skip(start).Take(duration);




                    int position = 0;
                if (sampleProvider != null)
                {
                    bool complete = false;
                    double[,] spectrogram = new double[1, 1];
                    while (spectrogram != null)
                    {
                        spectrogram = getFFT(ref sampleProvider);
                        Debug.WriteLine($"got a spectrogram of {spectrogram?.GetLength(0)}x{spectrogram?.GetLength(1)}");
                        if (spectrogram == null || spectrogram.GetLength(1) <= 0) break;
                        var args = new spectrumEventArgs();
                        args.sampleRate = sampleRate;
                        args.spectrogram = spectrogram;
                        args.durationSecs = durSecs;


                        args.totalFFTs = totalFFTs;
                        args.dataStart = position;
                        args.dataEnd = position + spectrogram.GetLength(1);

                        position = args.dataEnd - 1;
                        OnSectionCompleted(args);
                        Debug.WriteLine("@@@@@ ++++ Dispatch Completed Section");

                    }
                    OnSpectrumCompleted(EventArgs.Empty);
                    complete = true;
                    Debug.WriteLine("@@@@@ ++++ Dispatch Spectrum Complete");
                }
                else
                {
                    errcode = 2;
                }

            }
    
            Debug.WriteLine("@@@@@ ++++ GetSpectrogramAsync finished");
            return (errcode);
        });

        private double[,] getFFT(ref ISampleProvider sampleProvider)
        {
           
            
            var data = getDataBlock(ref sampleProvider);
            Debug.WriteLine($"spectrogram of {data.Length} samples");
            if (data.Length > 0)
            {
                if (HighPassCutoff > 0)
                {
                    data = HighPassFilter(data, sampleRate);
                }
                if (LowPassCutOff > 0)
                {
                    data = LowPassFilter(data, sampleRate);
                }
                double[,] spectrogram = GetBlockSpectrogram(data);
                return (spectrogram);
            }
            return null;
        }

        private double[,] GetBlockSpectrogram(float[] data)
        {
            var sg = new SpectrogramGenerator(sampleRate, fftSize: FFTSize,stepSize: FFTAdvance, maxFreq: sampleRate / 2);

            sg.Add(data.Select(x=>(double)x));
            var ft = sg.GetFFTs();
            var result = new double[ft[0].Length, ft.Count];
            for(int l = 0; l < ft.Count; l++)
            {
                for(int n = 0; n < ft[0].Length; n++)
                {
                    ft[l][n] = 20.0 * Math.Log10(ft[l][n]);
                    result[ft[0].Length - n - 1, l] = ft[l][n];
                }
            }
            return (result);

        }

       

        private float[] getDataBlock(ref ISampleProvider sampleProvider)
        {
            
            float[] fdata = new float[sampleRate * 5];
            var dataRead = sampleProvider.Read(fdata, 0, sampleRate * 5);
            Debug.WriteLine($"Read {dataRead} samples");
            float[] data = fdata[..dataRead];
            //float[] data = new float[dataRead];
            //for (int i = 0; i < data.Length; i++) data[i] = fdata[i];
            return data;
        }

        private float[] HighPassFilter(float[] dataArray, int sampleRate)
        {
            var filter=BiQuadFilter.HighPassFilter(sampleRate, HighPassCutoff, FilterQ);
            for (int j = 0; j < dataArray.Length; j++)
            {
                var tmp = filter.Transform(dataArray[j]);
                if (float.IsNaN(tmp))
                {
                    tmp = dataArray[j];
                }
                dataArray[j] = tmp;
            }
            return (dataArray);
        }

        private float[] LowPassFilter(float[] dataArray, int sampleRate)
        {
            var filter = BiQuadFilter.LowPassFilter(sampleRate, HighPassCutoff, FilterQ);
            for (int j = 0; j < dataArray.Length; j++)
            {
                var tmp = filter.Transform(dataArray[j]);
                if (float.IsNaN(tmp))
                {
                    tmp = dataArray[j];
                }
                dataArray[j] = tmp;
            }
            return (dataArray);
        }




        public event EventHandler<spectrumEventArgs> SectionCompletedEvent;

        //protected virtual void OnSectionCompleted(spectrumEventArgs e) => SectionCompletedEvent?.Invoke(this, e);
        protected virtual void OnSectionCompleted(spectrumEventArgs e) => Application.Current.Dispatcher.Invoke((Action)delegate { SectionCompletedEvent(this, e); });

        public event EventHandler<EventArgs> SpectrumCompleted;

        //protected virtual void OnSpectrumCompleted(EventArgs e) => SpectrumCompleted?.Invoke(this, e);
        protected virtual void OnSpectrumCompleted(EventArgs e) => Application.Current.Dispatcher.Invoke((Action)delegate { SpectrumCompleted(this, e); });
    }

    internal class spectrumEventArgs : EventArgs
    {
        public double[,] spectrogram { get; set; }
        public int sampleRate { get; set; }

        public int totalFFTs { get; set; }

        public int dataStart { get; set; }

        public int dataEnd { get; set; }

        public double durationSecs { get; set; }

        public (double[,] spectrogram,int sampleRate) SGData
        {
            get
            {
                return ((spectrogram, sampleRate));
            }
        }
    }
}
