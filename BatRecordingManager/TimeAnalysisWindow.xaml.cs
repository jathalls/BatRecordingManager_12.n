using MathNet.Numerics.Distributions;
using NAudio.Dsp;
using NAudio.Wave;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Text = ScottPlot.Plottable.Text;

namespace BatRecordingManager
{
    /// <summary>
    /// Interaction logic for TimeAnalysisWindow.xaml
    /// </summary>
    public partial class TimeAnalysisWindow : Window
    {
        public TimeAnalysisWindow()
        {
            Loaded += TimeAnalysisWindow_Loaded;
            InitializeComponent();
        }
        private bool loaded;
        private void TimeAnalysisWindow_Loaded(object sender, RoutedEventArgs e)
        {
            loaded = true;
        }

        private static TimeAnalysisWindow instance;

        private Limits limits;

        public static void Create(Limits limits)
        {
            TimeAnalysisWindow timeAnalysisWindow= new TimeAnalysisWindow();
            instance = timeAnalysisWindow;
            instance.limits = limits;
            instance.Analyse();
            //timeAnalysisWindow.Show();

        }

        /// <summary>
        /// Performs the time based analysis
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void Analyse()
        {
            if (limits == null || !File.Exists(limits.filename)) return ;
            FilterParams filter = new FilterParams();
            filter.FilterQ = 1;
            filter.HighPassFilterFrequency = (int)(limits.fMin * 1000.0d);
            filter.LowPassFilterFrequency = (int)(limits.fMax * 1000.0d);
            filter.LowPassFilterIterations = 2;
            filter.HighPassFilterIterations = 2;
            //var data = SegmentSonagrams.GetFilteredData(limits.filename, limits.startInFile, limits.length, filter);

            (double[] audio, int sampleRate) data = GetData(limits);

            data = GetEnvelope(data);
            double[] correlation = GetAutoCorrelation(data);
            double[] peaks = GetPeaks(correlation,data.sampleRate);
        }

        private (double[] audio, int sampleRate) GetData(Limits limits)
        {
            



                
                FilterParams filter = new FilterParams();
                filter.FilterQ = 1;
                filter.HighPassFilterFrequency = (int)(limits.fMin * 1000.0d);
                filter.LowPassFilterFrequency = (int)(limits.fMax * 1000.0d);
                filter.LowPassFilterIterations = 2;
                filter.HighPassFilterIterations = 2;
                var data = SegmentSonagrams.GetFilteredData(limits.filename, limits.startInFile, limits.length, filter);
                return (data);
               
        }

        private double[] GetPeaks(double[] correlation,int sampleRate)
        {
            this.Show();
            while (!loaded) Thread.Sleep(10);
            signalPlot=wpfPlot.Plot.AddSignal(correlation, sampleRate);
            
            crossHair=wpfPlot.Plot.AddCrosshair(0.0d, 0.0d);
            txt = wpfPlot.Plot.AddText("_", 0, 0);
            txt.Alignment = ScottPlot.Alignment.LowerLeft;
            wpfPlot.Refresh();
            return (correlation);
        }

        private SignalPlot signalPlot = new SignalPlot();
        private Crosshair crossHair = new Crosshair();
        private Crosshair staticCrossHair;

        /// <summary>
        /// Returns a one-sided autocorrelation of the data for an extent of 1 second according to the specified
        /// sample rate, or length of the sample if less than 1s
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private double[] GetAutoCorrelation((double[] audio, int sampleRate) data)
        {
            var result = new double[(int)Math.Min(data.audio.Length,data.sampleRate)];
            for(int i = 0; i < result.Length; i++)
            {
                double total = 0.0d;
                for(int j = 0; j < data.audio.Length/2; j++)
                {
                    int index = j + i;
                    double value = 0.0d;
                    if(index<data.audio.Length) value=data.audio[index];
                    total += data.audio[j] * value;
                }
                result[i] = total;
            }
            return (result);
        }

        /// <summary>
        /// given an array of audio data and a sample rate, extarcts the enveope of the data bye rectification and
        /// low pass filtering at 10kHz.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private (double[] audio, int sampleRate) GetEnvelope((double[] audio, int sampleRate) data)
        {
            (double[] audio, int sampleRate) result = (new double[data.audio.Length], data.sampleRate/10);
            List<double> correlation=new List<double>();

            BiQuadFilter filter = BiQuadFilter.LowPassFilter(result.sampleRate, 5000.0f,2.0f);
            
            for(int i = 0; i < result.audio.Length ; i++)
            {
                var val= (double)filter.Transform((float)(data.audio[i] * data.audio[i]));
                if(i%10==0) correlation.Add(val*val);
                 
                
            }
            result.audio=correlation.ToArray();
            return (result);
            
        }

        private static (double x, double y) baseCoordinates = (0, 0);

        private Text txt { get; set; }

        private void wpfPlot_MouseMove(object sender, MouseEventArgs e)
        {
            var pos = wpfPlot.GetMouseCoordinates();
            var limits = wpfPlot.Plot.GetAxisLimits();
            crossHair.X = pos.x;
            
            crossHair.Y =pos.y;
            crossHair.VerticalLine.PositionFormatter = customFormatter;

            
            if ( txt != null)
            {
                
                txt.X = crossHair.X;
                txt.Y = crossHair.Y;

                txt.Label = $"   {txt.X:F3}";
                    

            }

            wpfPlot.Refresh();


        }

        private void wpfPlot_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            baseCoordinates=wpfPlot.GetMouseCoordinates();
        }

        static string customFormatter(double position)
        {


            return ($"{position - baseCoordinates.x:F3}");
        }

        private void wpfPlot_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            baseCoordinates = wpfPlot.GetMouseCoordinates();
            if (staticCrossHair != null) wpfPlot.Plot.Remove(staticCrossHair);
            staticCrossHair = wpfPlot.Plot.AddCrosshair(baseCoordinates.x, baseCoordinates.y);
            staticCrossHair.VerticalLine.PositionLabelOppositeAxis=true;
            staticCrossHair.HorizontalLine.PositionLabelOppositeAxis = true;
            e.Handled = true;
        }
    }
}
