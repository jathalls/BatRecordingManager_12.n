
using LinqStatistics;
using Microsoft.Maps.MapControl.WPF;
using NAudio.Dsp;
using NAudio.Wave;
using Spectrogram;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UniversalToolkit;

namespace BatRecordingManager
{
    internal class SegmentSonagrams
    {
        public List<double[]> Ffts { get; private set; }

        public bool GenerateForSegments(List<LabelledSegment> segmentList, bool experimental = false,bool display=false)
        {
            if (segmentList != null && segmentList.Any())
            {
                foreach (var seg in segmentList)
                {
                    if (seg.BatSegmentLinks?.Any() ?? false)
                    {
                        ObservableCollection<StoredImage> imageList = new ObservableCollection<StoredImage>();
                        StoredImage spectrogram = GenerateSpectrogram(seg, experimental: experimental,display: display);
                        if (spectrogram != null)
                        {
                            imageList.Add(spectrogram);
                            DBAccess.UpdateSegmentImages(imageList, seg);
                        }
                    }
                }
                return (true);
            }
            return (false);
        }

        public Task<bool> GenerateForSegmentsAsync(List<LabelledSegment> segments)
        {
            return (Task.Run(() => GenerateForSegments(segments)));
        }

        internal StoredImage GenerateForSegment(LabelledSegment sel, Parametrization param = null,FilterParams filterParams=null,bool display=false)
        {
            return (GenerateSpectrogram(sel, param,filterParams: filterParams,display: display));
        }

        async internal void GenerateForSession(RecordingSession selectedSession)
        {
            List<LabelledSegment> segments = DBAccess.GetSessionSegments(selectedSession);
            var success = await GenerateForSegmentsAsync(segments);
            OnGenerationComplete(EventArgs.Empty);
        }

        public event EventHandler<EventArgs> GenerationComplete;

        private FilterParams FilterParams = null;
        protected virtual void OnGenerationComplete(EventArgs args) => GenerationComplete?.Invoke(this, args);

        private LabelledSegment segment = null;
        /// <summary>
        /// Generates a spectrogram of the given LabelledSegment.  If the segment already has an image
        /// of type SPCT then that is retrieved and the image of it is modified to the new spectrogram
        /// </summary>
        /// <param name="seg">the labelled segment to be analysed</param>
        /// <param name="param">instance of Parametrization for extracting numerical data</param>
        /// <param name="experimental">boolean - true gives a spectrogram with hyerbolic (1/f) frequency axis</param>
        /// <returns></returns>
        private StoredImage GenerateSpectrogram(LabelledSegment seg, Parametrization param = null,
            bool experimental = false, decimal percentOverlap = -1.0m,
            FilterParams filterParams = null, bool display = false)
        {
            segment = seg;
            FilterParams = filterParams;
            if (seg == null || (seg.StartOffset.TotalMilliseconds == 0 && seg.EndOffset == seg.StartOffset)) return (null);
            if (string.IsNullOrWhiteSpace(seg.Comment) || seg.Comment.Contains("No Bats")) return (null);
            if (seg.BatSegmentLinks == null || seg.BatSegmentLinks.Count <= 0) return (null);
            StoredImage si = DBAccess.GetSpectrogramForSegment(seg); // retrieve if already exists in the database
            if (si == null || display)
            {
                //int FFTOrder = 10;
                // List<float> data = GetData(seg, FFTOrder, out int sampleRate);

                var settings = UTSettings.getSettings();

                int stepSize = settings.Spectrogram.FFTAdvance;
                if (percentOverlap > 0.0m)
                {
                    var pcAdvance = 100.0m - percentOverlap;
                    stepSize = (int)((pcAdvance / 100.0m) * settings.Spectrogram.FFTSize);
                    if (stepSize <= 0) stepSize = settings.Spectrogram.FFTAdvance;
                }

                var data = GetDataSG(seg, settings.Spectrogram.FFTSize, out int sampleRate, 
                    (double)settings.Spectrogram.scale,filterParams: filterParams);
                if (data.audio == null) return (null);
                //if (experimental) data = halfWaveRectify(data);
                Debug.WriteLine($"gen spectrogram-> data {data.audio.Count()} lasting {data.audio.Count() / (double)sampleRate}s");
                int maxFrequency = sampleRate / 2;
                if (settings.Spectrogram.maxFrequency > 0)
                {
                    maxFrequency = settings.Spectrogram.maxFrequency;
                }
                System.Drawing.Bitmap bmp = null;

                var sg = new SpectrogramGenerator(sampleRate,
                    fftSize: settings.Spectrogram.FFTSize,
                    stepSize: settings.Spectrogram.FFTAdvance,
                    maxFreq: maxFrequency);

                sg.Add(data.audio);
                sg.Colormap = Colormap.GrayscaleReversed;
                var ft=sg.GetFFTs();
                Ffts = ft;
                maxFrequencykHzToShow = maxFrequency / 1000.0d;
                maxFrequencykHz = (sampleRate / 2.0d) / 1000.0d;
                secsPerFft=sg.SecPerPx;
                //sg.SetColormap(Colormap.GrayscaleReversed);
                //sg.SaveImage(@"C:\BRMTestData\Test.png", dB: true, dBScale: 20, intensity: 5);
                Debug.WriteLine($"Scale={settings.Spectrogram.scale}");

                //sg.getFFTs returns a List of double[]
                bmp = sg.GetBitmap(dB: true, dBScale: settings.Spectrogram.dBScale, intensity: settings.Spectrogram.intensity);
                if (experimental)
                {

                    bmp = sg.GetBitmapMel(melBinCount: 512, dB: true, dBScale: settings.Spectrogram.dBScale, intensity: settings.Spectrogram.intensity);
                }

                bmp = DeepAnalysis.decorateBitmap(bmp, settings.Spectrogram.FFTSize, settings.Spectrogram.FFTAdvance, sampleRate, param);
                //List<Spectrum> spectra = DeepAnalysis.GetSpectrum(data, sampleRate, FFTOrder, 0.5f, out int FFTAdvance, out double advanceMS, out double[] FFTQuiet);
                //var bmp = DeepAnalysis.GetBmpFromSpectra(spectra, FFTOrder, 0.5f, sampleRate);
                si = new StoredImage(Tools.ToBitmapSource(bmp),
                    $"{seg.Recording.RecordingName} {seg.StartOffset.TotalSeconds} - {seg.EndOffset.TotalSeconds}" +
                    (filterParams!=null?$", filtered {filterParams.HighPassFilterFrequency}-{filterParams.LowPassFilterFrequency}":""),
                    $"FFTSize/Advance={settings.Spectrogram.FFTSize}/{settings.Spectrogram.FFTAdvance}\n{seg.Comment}",
                    -1, false, Tools.BlobType.SPCT);
            }
            return (si);
        }

        public double maxFrequencykHzToShow { get; set; } = 192.0;
        public double maxFrequencykHz { get; set; } = 192.0;

        public double secsPerFft { get; set; } = 1.0d;

        private (double[] audio, int sampleRate) halfWaveRectify((double[] audio, int sampleRate) data)
        {
            for (int i = 0; i < data.audio.Length; i++)
            {
                if (data.audio[i] < 0) data.audio[i] = 0;
            }
            return data;
        }

        private List<float> GetData(LabelledSegment segment, int FFTOrder, out int sampleRate)
        {
            List<float> result = new List<float>();
            sampleRate = 384000;
            int FFTSize = (int)Math.Pow(2, FFTOrder);

            string file = segment.Recording.GetFileName();
            //string file = Path.Combine(sel.Recording.RecordingSession.OriginalFilePath,sel.Recording.RecordingName);
            if (!File.Exists(file)) return result;

            using (var wfr = new WaveFileReader(file))
            {
                var sp = wfr.ToSampleProvider();
                sampleRate = wfr.WaveFormat.SampleRate;
                TimeSpan leadin = new TimeSpan();
                var requestedDuration = segment.EndOffset - segment.StartOffset;
                if (requestedDuration.TotalSeconds > 15)
                {
                    leadin = TimeSpan.FromSeconds(((requestedDuration.TotalSeconds - 15.0d) / 2.0d));
                    requestedDuration = TimeSpan.FromSeconds(15.0d);
                }
                var data = sp.Skip(segment.StartOffset + leadin).Take(requestedDuration);
                float[] faData = new float[FFTSize];
                result = new List<float>();
                int samplesRead;
                while ((samplesRead = data.Read(faData, 0, FFTSize)) > 0)
                {
                    result.AddRange(faData.Take(samplesRead));
                }

                var filter = BiQuadFilter.HighPassFilter(sampleRate, 15000, 1);
                for (int i = 0; i < result.Count; i++)
                {
                    result[i] = filter.Transform(result[i]);
                }
            }
            return (result);
        }

        public TimeSpan duration = new TimeSpan();
        public int SampleRate = 384000;

        private (double[] audio, int sampleRate) GetDataSG(LabelledSegment segment, int FFTSize, out int sampleRate,
            double scale = 16000.0d,FilterParams filterParams=null)
        {
            List<double> result;
            double[] dataArray;

            sampleRate = 384000;
            //int FFTSize = (int)Math.Pow(2, FFTOrder);

            string file = segment.Recording.GetFileName();
            //string file = Path.Combine(sel.Recording.RecordingSession.OriginalFilePath,sel.Recording.RecordingName);
            if (!File.Exists(file)) return (null, sampleRate);
            var audio = new double[1];
            using (var wfr = new AudioFileReader(file))
            {
                var sp = wfr.ToSampleProvider();
                sampleRate = wfr.WaveFormat.SampleRate;
                SampleRate = sampleRate;
                TimeSpan leadin = new TimeSpan();
                var requestedDuration = segment.EndOffset - segment.StartOffset;
                if (requestedDuration.TotalSeconds > 15)
                {
                    leadin = TimeSpan.FromSeconds(((requestedDuration.TotalSeconds - 15.0d) / 2.0d));
                    requestedDuration = TimeSpan.FromSeconds(15.0d);
                }

                duration = requestedDuration;

                var data = sp.Skip(segment.StartOffset + leadin).Take(requestedDuration);
                var sampleCount = (int)(requestedDuration.TotalSeconds * sampleRate);
                float[] faData = new float[FFTSize];
                result = new List<double>();
                int samplesRead;
                //double scale = 16_000.0d;

                while ((samplesRead = data.Read(faData, 0, FFTSize)) > 0)
                {
                    result.AddRange(faData.Take(samplesRead).Select(x => x * scale));
                }

                dataArray = result.ToArray();
                if (filterParams != null)
                {

                    BiQuadFilter filter;
                    for(int i = 0; i < filterParams.HighPassFilterIterations; i++)
                    {
                        filter = BiQuadFilter.HighPassFilter(sampleRate, filterParams.HighPassFilterFrequency,
                        (float)filterParams.FilterQ);
                        Debug.WriteLine($"HPFilter {sampleRate}, {filterParams.HighPassFilterFrequency}, {filterParams.FilterQ}");

                        for (int j = 0; j < dataArray.Length; j++)
                        {
                            var tmp= filter.Transform((float)dataArray[j]);
                            if (float.IsNaN(tmp))
                            {
                                tmp = (float)dataArray[j];
                            }
                            dataArray[j] = (double)tmp;
                        }
                    }

                    
                    
                    for (int i = 0; i < filterParams.LowPassFilterIterations; i++)
                    {
                        filter = BiQuadFilter.LowPassFilter(sampleRate, filterParams.LowPassFilterFrequency,
                        (float)filterParams.FilterQ);
                        Debug.WriteLine($"LPFilter {sampleRate}, {filterParams.LowPassFilterFrequency}, {filterParams.FilterQ}");
                        for (int j = 0; j < dataArray.Length; j++)
                        {
                            
                            var tmp = filter.Transform((float)dataArray[j]);
                            if (float.IsNaN(tmp))
                            {
                                tmp = (float)dataArray[j];
                            }
                            dataArray[j] = (double)tmp;
                        }
                    }
                    
                }
                /*
                var filter = BiQuadFilter.HighPassFilter(sampleRate, 15000, 1);
                audio = new double[result.Count];
                var maxVal = result.Max();
                var scale = 1.0f / maxVal;
                for (int i = 0; i < result.Count; i++)
                {
                    result[i] = result[i] * scale * 1000.0f;
                    audio[i] = (double)filter.Transform(result[i]);
                }*/
            }
            return (dataArray, sampleRate);
        }

        internal double[,] Regen(int FFTSize,int FFTAdvance)
        {
            if (segment != null)
            {
                

                var data = GetDataSG(segment, FFTSize, out int sampleRate,
                         filterParams: FilterParams);
                if (data.audio == null) return null;
                //if (experimental) data = halfWaveRectify(data);
                Debug.WriteLine($"gen spectrogram-> data {data.audio.Count()} lasting {data.audio.Count() / (double)sampleRate}s");
                int maxFrequency = sampleRate / 2;
                
                

                var sg = new SpectrogramGenerator(sampleRate,
                    fftSize: FFTSize,
                    stepSize: FFTAdvance,
                    maxFreq: maxFrequency);

                sg.Add(data.audio);

                

                var fts = sg.GetFFTs();

                maxFrequencykHzToShow = maxFrequency / 1000.0d;
                maxFrequencykHz = (sampleRate / 2.0d) / 1000.0d;
                secsPerFft = sg.SecPerPx;

                var result = new double[ fts[0].Length,fts.Count];

                minval = double.MaxValue;
                maxval = double.MinValue;

                for (int l = 0; l < fts.Count; l++)
                {
                    for (int n = 0; n < fts[0].Length; n++)
                    {
                        fts[l][ n] = 20.0 * Math.Log10(fts[l][ n]);
                        result[fts[0].Length - n - 1, l] = fts[l][n];
                        if (fts[l][n] < minval) minval = fts[l][n];
                        if(fts[l][n] > maxval) maxval = fts[l][n];
                        
                    }
                }
                rangeval = maxval - minval;

                return (result);
            }
            return (null);
        }

        private double minval = 0.0d;
        private double maxval = 2.0d;
        private double rangeval = 2.0d;

        internal void GetRange(out double min, out double max, out double range)
        {
            min = minval;
            max = maxval;
            range = maxval - minval;
        }
    }
}