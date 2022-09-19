
using FftSharp;
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

        public bool GenerateForSegments(List<LabelledSegment> segmentList, bool experimental = false,DisplayMode display = DisplayMode.NONE)
        {
            if (segmentList != null && segmentList.Any())
            {
                foreach (var seg in segmentList)
                {
                    if (seg.BatSegmentLinks?.Any() ?? false)
                    {
                        ObservableCollection<StoredImage> imageList = new ObservableCollection<StoredImage>();
                        StoredImage spectrogram = GenerateSpectrogram(seg, experimental: experimental, display: display);
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

        internal StoredImage GenerateForSegment(LabelledSegment sel, Parametrization param = null, 
            FilterParams filterParams = null, DisplayMode display = DisplayMode.NONE)
        {
            return (GenerateSpectrogram(sel, param, filterParams: filterParams, display: display));
        }

        async internal void GenerateForSession(RecordingSession selectedSession)
        {
            List<LabelledSegment> segments = DBAccess.GetSessionSegments(selectedSession);
            var success = await GenerateForSegmentsAsync(segments);
            OnGenerationComplete(EventArgs.Empty);
        }

        /// <summary>
        /// Generates a spectrogram for a given .wav file starting withe specified offset, and with a segment
        /// length specified.  Both these parameters as doubles in seconds.
        /// returns true if the spectrogram is made OK, false otherwise
        /// </summary>
        /// <param name="fileName">Fully qualified name of .wav file to be analysed</param>
        /// <param name="startOffset">starting location of the section to be analysed in seconds</param>
        /// <param name="duration">length of the section to be analysed in seconds</param>
        /// <param name="FFTSize">Size of the FFT window to be used, default is 1024</param>
        /// <param name="FFTAdvancePercent">the percentage FFT advance to be used, default is 50%</param>
        public StoredImage GenerateForFile(string fileName, double startOffset, double duration, int? FFTSize = null, double? FFTAdvancePercent = null,
            Parametrization param = null, FilterParams filterParams = null, DisplayMode display = DisplayMode.NONE)
        {
            StoredImage result = null;
            this.fileName = fileName;
            this.startOffset = startOffset;
            this.durationSecs = duration;
            this.segment = null;


            result = Generate(FFTSize, FFTAdvancePercent, param, filterParams: filterParams, display: display);
            return (result);
        }



        public event EventHandler<EventArgs> GenerationComplete;

        private FilterParams FilterParams = null;
        protected virtual void OnGenerationComplete(EventArgs args) => GenerationComplete?.Invoke(this, args);

        public LabelledSegment segment = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fFTSize"></param>
        /// <param name="fFTAdvancePercent"></param>
        /// <param name="param"></param>
        /// <param name="filterParams"></param>
        /// <param name="display"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private StoredImage Generate(int? FFTSize = null, double? FFTAdvancePercent = null, Parametrization param = null,
            FilterParams filterParams = null, DisplayMode display = DisplayMode.NONE)
        {
            var settings = UTSettings.getSettings();

            if (FFTSize == null) FFTSize = settings.Spectrogram.FFTSize;

            int stepSize = FFTSize.Value / 2;
            if (FFTAdvancePercent == null)
            {
                stepSize = settings.Spectrogram.FFTAdvance;
            }



            if (FFTAdvancePercent > 0.0d)
            {

                stepSize = (int)((FFTAdvancePercent / 100.0d) * FFTSize.Value);
                if (stepSize <= 0) stepSize = FFTSize.Value / 2;
            }

            //var data = GetDataSG(seg, settings.Spectrogram.FFTSize, out int sampleRate,
            //    (double)settings.Spectrogram.scale, filterParams: filterParams);

            var data = GetDataSG(fileName, startOffset, durationSecs, FFTSize.Value, out int sampleRate, filterParams: filterParams);
            SampleRate = sampleRate;
            if (data.audio == null) return (null);
            //if (experimental) data = halfWaveRectify(data);
            //Debug.WriteLine($"gen spectrogram-> data {data.audio.Count()} lasting {data.audio.Count() / (double)sampleRate}s");
            int maxFrequency = sampleRate / 2;
            if (settings.Spectrogram.maxFrequency > 0)
            {
                maxFrequency = settings.Spectrogram.maxFrequency;
            }
            System.Drawing.Bitmap bmp = null;

            var sg = new SpectrogramGenerator(sampleRate,
                fftSize: FFTSize.Value,
                stepSize: stepSize,
                maxFreq: maxFrequency);

            sg.Add(data.audio);
            sg.Colormap = Colormap.GrayscaleReversed;
            var ft = sg.GetFFTs();
            Ffts = ft;
            maxFrequencykHzToShow = maxFrequency / 1000.0d;
            maxFrequencykHz = (sampleRate / 2.0d) / 1000.0d;
            secsPerFft = sg.SecPerPx;
            //sg.SetColormap(Colormap.GrayscaleReversed);
            //sg.SaveImage(@"C:\BRMTestData\Test.png", dB: true, dBScale: 20, intensity: 5);
            Debug.WriteLine($"Scale={settings.Spectrogram.scale}");

            //sg.getFFTs returns a List of double[]
            bmp = sg.GetBitmap(dB: true, dBScale: settings.Spectrogram.dBScale, intensity: settings.Spectrogram.intensity);


            bmp = DeepAnalysis.decorateBitmap(bmp, settings.Spectrogram.FFTSize, settings.Spectrogram.FFTAdvance, sampleRate, param);
            //List<Spectrum> spectra = DeepAnalysis.GetSpectrum(data, sampleRate, FFTOrder, 0.5f, out int FFTAdvance, out double advanceMS, out double[] FFTQuiet);
            //var bmp = DeepAnalysis.GetBmpFromSpectra(spectra, FFTOrder, 0.5f, sampleRate);
            StoredImage si = null;
            if (segment != null)
            {
                si = new StoredImage(Tools.ToBitmapSource(bmp),
                    $"{segment.Recording.RecordingName} {segment.StartOffset.TotalSeconds} - {segment.EndOffset.TotalSeconds}" +
                    (filterParams != null ? $", filtered {filterParams.HighPassFilterFrequency}-{filterParams.LowPassFilterFrequency}" : ""),
                    $"FFTSize/Advance={settings.Spectrogram.FFTSize}/{settings.Spectrogram.FFTAdvance}\n{segment.Comment}",
                    -1, false, Tools.BlobType.SPCT);
            }
            else
            {
                si = new StoredImage(
                    Tools.ToBitmapSource(bmp),
                    $"{fileName} {startOffset} - {startOffset + durationSecs}" +
                    (filterParams != null ? $", filtered {filterParams.HighPassFilterFrequency}-{filterParams.LowPassFilterFrequency}" : ""),
                    $"FFTSize/Advance={FFTSize}/{FFTAdvancePercent}\n",
                    -1, false, Tools.BlobType.SPCT);
            }

            return (si);
        }
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
            FilterParams filterParams = null, DisplayMode display = DisplayMode.NONE)
        {
            segment = seg;
            FilterParams = filterParams;
            if (seg == null || (seg.StartOffset.TotalMilliseconds == 0 && seg.EndOffset == seg.StartOffset)) return (null);

            this.fileName = seg.Recording?.GetFileName();
            this.startOffset = seg.StartOffset.TotalSeconds;
            this.durationSecs = seg.Duration()?.TotalSeconds ?? 5.0d;
            if (display == DisplayMode.NONE || display == DisplayMode.IF_HAS_BATS)
            {
                if (string.IsNullOrWhiteSpace(seg.Comment) || seg.Comment.Contains("No Bats")) return (null);
                if (seg.BatSegmentLinks == null || seg.BatSegmentLinks.Count <= 0) return (null);
            }
            StoredImage si = DBAccess.GetSpectrogramForSegment(seg); // retrieve if already exists in the database
            if (si == null || display!=DisplayMode.NONE)
            {
                double FFTAdvance = 0.0d;
                if (percentOverlap < 0.0m) FFTAdvance = -1.0d;
                else
                {
                    FFTAdvance = (double)(100.0m - percentOverlap);
                    if (FFTAdvance < 0.0d) FFTAdvance = 0.0d;
                    if (FFTAdvance > 100.0d) FFTAdvance = 100.0d;
                }

                si = Generate(FFTAdvancePercent: FFTAdvance, param: param, filterParams: filterParams, display: display);
                //int FFTOrder = 10;
                // List<float> data = GetData(seg, FFTOrder, out int sampleRate);


            }
            return (si);
        }

        public enum DisplayMode {NONE,IF_HAS_BATS,ALWAYS };
       

        public double maxFrequencykHzToShow { get; set; } = 192.0;
        public double maxFrequencykHz { get; set; } = 192.0;

        public double secsPerFft { get; set; } = 1.0d;

        

        

        public TimeSpan duration = new TimeSpan();
        public int SampleRate = 384000;

        private (double[] audio, int sampleRate) GetDataSG(LabelledSegment segment, int FFTSize, out int sampleRate,
            double scale = 16000.0d, FilterParams filterParams = null)
        {
            int sr = 384000;   
            var result=GetDataSG(segment?.Recording?.GetFileName()??"",segment?.StartOffset.TotalSeconds??0.0d,segment?.Duration()?.TotalSeconds??5.0d,FFTSize,out sr,
                scale,filterParams);
            sampleRate = sr;
            return (result);

        }

        private (double[] audio,int sampleRate) GetDataSG(string file,double startOffset,double durationSecs,int FFTSize,out int sampleRate,
            double scale=16000.0d,FilterParams filterParams = null)
        { 
            List<double> result;
            double[] dataArray;

            sampleRate = 384000;
            //int FFTSize = (int)Math.Pow(2, FFTOrder);

            
            //string file = Path.Combine(sel.Recording.RecordingSession.OriginalFilePath,sel.Recording.RecordingName);
            if (!File.Exists(file)) return (null, sampleRate);
            var audio = new double[1];
            using (var wfr = new AudioFileReader(file))
            {
                var sp = wfr.ToSampleProvider();
                sampleRate = wfr.WaveFormat.SampleRate;
                SampleRate = sampleRate;
                TimeSpan leadin = new TimeSpan();
                
                

                duration = TimeSpan.FromSeconds(durationSecs);

                var data = sp.Skip(TimeSpan.FromSeconds(startOffset) + leadin).Take(duration);
                var sampleCount = (int)(durationSecs * sampleRate);
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

        public string fileName = "";
        public double startOffset = 0.0d;
        public double durationSecs = 5.0d;

        internal double[,] Regen(int FFTSize,int FFTAdvance)
        {
            if (!((segment == null)&&string.IsNullOrWhiteSpace(fileName)))
            {
                int sampleRate = SampleRate;
                (double[] audio, int sampleRate)? data = null;
                if (segment != null)
                {
                    data = GetDataSG(segment, FFTSize, out sampleRate,
                             filterParams: FilterParams);
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(fileName)) { return (null); }
                    if(!File.Exists(fileName)) { return (null); }
                    data = GetDataSG(fileName, startOffset, durationSecs, FFTSize, out sampleRate, filterParams: FilterParams);
                }
                if (data?.audio == null) return null;
                //if (experimental) data = halfWaveRectify(data);
                Debug.WriteLine($"gen spectrogram-> data {data.Value.audio.Count()} lasting {data.Value.audio.Count() / (double)sampleRate}s");
                int maxFrequency = sampleRate / 2;
                
                

                var sg = new SpectrogramGenerator(sampleRate,
                    fftSize: FFTSize,
                    stepSize: FFTAdvance,
                    maxFreq: maxFrequency);

                sg.Add(data?.audio??new double[0]);

                

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

        /// <summary>
        /// is called when this sonagram generator is being closed and finished with typically by SpectrogramWindow
        /// </summary>
        internal void Close()
        {
            
        }
    }
}