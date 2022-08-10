using LinqStatistics;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BatRecordingManager
{
    /// <summary>
    /// Interaction logic for SpectrogramWindow.xaml
    /// </summary>
    public partial class SpectrogramWindow : Window
    {
        public SpectrogramWindow()
        {
            InitializeComponent();
        }

        private static SpectrogramWindow Sgi { get; set; } = null;

        internal static void Display(SegmentSonagrams sonagramGenerator)
        {
            
            if (Sgi == null)
            {
                Sgi = new SpectrogramWindow();
                Sgi.Closing += SpectrogramWindowInstance_Closing;
                Sgi.Closed += SpectrogramWindowInstance_Closed;
                Sgi.sonagramGenerator = sonagramGenerator;
            }

            Sgi.heatMap = null;
            Sgi.RegenSonagram();
            

            


            Sgi.Show();
            Sgi.Update();

           
            
            

           
        }

        //private double[,] rawData;
        private double[,] data;
        private ScottPlot.Plottable.Heatmap heatMap;

        private SegmentSonagrams sonagramGenerator = null;

        private static void SpectrogramWindowInstance_Closed(object sender, EventArgs e)
        {
            Debug.WriteLine("Spectrogram Window closed");
            Sgi = null;
        }

        private static void SpectrogramWindowInstance_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine("Spectrogram window closing");
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        

        private void Update()
        {
            /*
            for(int i = 0; i < data.GetLength(0); i++)
            {
                for(int j=0;j<data.GetLength(1); j++)
                {
                    data[i,j]=(rawData[i,j]+offset)*Math.Pow(10.0,GainSlider.Value);
                }
            }*/
            
            if (data == null) return;
            


            wpfPlot.Plot.Clear();
            //heatMap = wpfPlot.Plot.AddHeatmap(data,colormap:ScottPlot.Drawing.Colormap.GrayscaleR);


            var plt = wpfPlot.Plot;

            var max = (OffsetSlider?.Value) ?? 60.0d;
            var min = max - ((GainSlider?.Value) ?? 96.0d);
            //heatMap.Update(data, max: max, min: min);

            heatMap = plt.AddHeatmap(data, ScottPlot.Drawing.Colormap.GrayscaleR, lockScales: false);
            heatMap.Update(data, max: max, min: min);
            //for(int l = 50; l < max; l += 50)
            //{
            //    plt.AddHorizontalLine(l);
            //}


            plt.YAxis.IsVisible = false;
            plt.XAxis.IsVisible = false;
            plt.Margins(0, 0);
            plt.Render();

            sonagramGenerator.GetRange(out double mindB, out double maxdB, out double rangedB);

            
            

            if (FreqAxis != null) plt.RemoveAxis(FreqAxis);
            if (TimeAxis != null) plt.RemoveAxis(TimeAxis);
            
            FreqAxis = plt.AddAxis(ScottPlot.Renderable.Edge.Left, axisIndex: 2);
            FreqAxis.Label("Freq kHz");
            FreqAxis.MajorGrid(enable:true,lineWidth:1,lineStyle:ScottPlot.LineStyle.Solid,color:System.Drawing.Color.Black);
            var fMax = ((sonagramGenerator?.SampleRate) ?? 384000) / 2000;
            wpfPlot.Plot.SetAxisLimits(yMin: 0, yMax: fMax, yAxisIndex: 2);
            

            FreqAxis.IsVisible = true;

            //var tScaleMax = sonagramGenerator.secsPerFft * ft.Count;
            TimeAxis = plt.AddAxis(ScottPlot.Renderable.Edge.Bottom, axisIndex: 2);
            TimeAxis.Label("time s");
            wpfPlot.Plot.SetAxisLimits(xMin: 0, xMax: (sonagramGenerator?.duration.TotalSeconds) ?? 1.0d, xAxisIndex: 2);

            TimeAxis.IsVisible = true;

            var cb = plt.AddColorbar(heatMap);

            plt.Grid(onTop: true);
            heatMap.Smooth = SmoothingCheckBox.IsChecked ?? false;
            
            
            wpfPlot.Refresh();
            
        }

        private ScottPlot.Renderable.Axis FreqAxis = null;
        private ScottPlot.Renderable.Axis TimeAxis=null;

        

        

        private void OffsetSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OffsetSlider != null && heatMap != null)
            {
                var max= (OffsetSlider?.Value) ?? 60.0d;
                var min = max - ((GainSlider?.Value) ?? 96.0d);
                heatMap.Update(data, max: max, min: min);
                wpfPlot.Refresh();
            }
        }

        private void GainSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            if (GainSlider != null && OffsetSlider != null && heatMap != null)
            {
                var max = (OffsetSlider?.Value) ?? 60.0d;
                var min = max - ((GainSlider?.Value) ?? 96.0d);
                heatMap.Update(data, max: max, min: min);
                wpfPlot.Refresh();
            }
        }

        /// <summary>
        /// Actually smoothing check box changed - triggerd by both checked and unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SmoothingCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            if (heatMap != null)
            {
                heatMap.Smooth = SmoothingCheckBox.IsChecked ?? false;
                wpfPlot.Refresh();
            }
        }



        private void FtSizeListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            RegenSonagram();
        }

        private void FtAdvanceListbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            RegenSonagram();
        }

        private void RegenSonagram() 
        { 
            //Debug.WriteLine($"FT Size changed to {FtSizeListbox.SelectedItem.ToString()}");
            int ftSize = 1024;
            int ftAdvance = 512;

            if (FtSizeListbox == null || FtSizeListbox.SelectedIndex < 0) return;

            string selection = (FtSizeListbox?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (selection == null) return;
            int.TryParse(selection, out ftSize);
            if (ftSize <= 0) return;
            ftAdvance = ftSize / 2;

            selection = (FtAdvanceListbox?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (selection == null) return;
            if(selection.EndsWith("%"))selection=selection.Substring(0, selection.Length - "%".Length);
            double.TryParse(selection, out double ftAdvancePC);
            switch (ftAdvancePC)
            {
                case 12.5d: ftAdvance = ftSize / 8; break;
                case 25.0d: ftAdvance = ftSize / 4; break;
                case 50.0d: ftAdvance = ftSize / 2; break;
                case 75.0d: ftAdvance = (ftSize / 4) * 3; break;
                case 100.0d: ftAdvance = ftSize; break;
                default: return;
            }

            using (new WaitCursor())
            {
                data = sonagramGenerator?.Regen(ftSize, ftAdvance);
                


                
                if (data != null && heatMap!=null)
                {
                    Update();
                }
            }
        }
    }
}
