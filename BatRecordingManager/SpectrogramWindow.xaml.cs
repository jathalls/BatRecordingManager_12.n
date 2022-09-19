using LinqStatistics;
using MathNet.Numerics.LinearAlgebra.Storage;
using NAudio.Wave;
using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;
using Timer = System.Windows.Forms.Timer;

namespace BatRecordingManager
{
    /// <summary>
    /// Interaction logic for SpectrogramWindow.xaml
    /// </summary>
    public partial class SpectrogramWindow : Window
    {
        public SpectrogramWindow()
        {
            this.Loaded += SpectrogramWindow_Loaded;
            InitializeComponent();

            

        }
        /// <summary>
        /// called when the window is loaded to permit additional initialisations
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void SpectrogramWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var colorMaps=ScottPlot.Drawing.Colormap.GetColormapNames();
            ColourComboBox.ItemsSource= colorMaps;
            ColourComboBox.SelectedItem = "GrayscaleR";

            wpfPlot.AxesChanged += WpfPlot_AxesChanged;
        }

        private bool AxisLock = false;

        private void WpfPlot_AxesChanged(object sender, EventArgs e)
        {/*
            if (!AxisLock)
            {
                AxisLock = true;
                var original_RawPlotLimits = wpfPlot.Plot.GetAxisLimits();
                if (original_RawPlotLimits.XSpan == data.GetUpperBound(1) + 1 && original_RawPlotLimits.YSpan == data.GetUpperBound(0) + 1)
                {
                    TimeAxis?.Dims.SetAxis(min: 0, max: (sonagramGenerator?.duration.TotalSeconds) ?? 1.0d);
                    FreqAxis?.Dims.SetAxis(min: 0, max: ((sonagramGenerator?.SampleRate) ?? 384000) / 2000);
                }
                wpfPlot.Refresh();
                AxisLock = false;
            }*/
            if (!AxisLock)
            {
                
                AxisLock = true;
                var lims = wpfPlot.Plot.GetAxisLimits();
                if (currentPlayListItem != null)
                {
                    
                    currentPlayListItem.setStartAndLength(lims.XMin, lims.XSpan);
                }
                
                
                /*
                var hmDims = heatMap.GetAxisLimits();
                wpfPlot.Plot.SetAxisLimits(lims.XMin, lims.XMax, hmDims.YMin - spaceForLabels, hmDims.YMax);
                wpfPlot.Refresh();
                Debug.WriteLine($"Axes changed hm-YMin={hmDims.YMin}, space={spaceForLabels} - {hmDims.YMin - spaceForLabels},{hmDims.YMax} ");
                wpfPlot.Plot.YAxis.LockLimits(true);*/

                AxisLock = false;
            }
        }

        private static List<SpectrogramWindow> spectrogramWindowList = new List<SpectrogramWindow>();
        //private static SpectrogramWindow spectrogramWindow { get; set; } = null;

        internal static void Display(SegmentSonagrams sonagramGenerator, PlayListItem pli = null)
        {

            if (spectrogramWindowList == null)
            {
                spectrogramWindowList = new List<SpectrogramWindow>();
            }
            var spectrogramWindow = new SpectrogramWindow();
            spectrogramWindow.Closing += SpectrogramWindowInstance_Closing;
            spectrogramWindow.Closed += SpectrogramWindowInstance_Closed;
            spectrogramWindow.sonagramGenerator = sonagramGenerator;


            spectrogramWindow.heatMap = null;
            spectrogramWindow.RegenSonagram();



            

            spectrogramWindow.Show();
            spectrogramWindow.Update();

            spectrogramWindow.setPlayListItem(pli);

            spectrogramWindowList.Add(spectrogramWindow);




        }

        //private double[,] rawData;
        private double[,] data;
        private ScottPlot.Plottable.Heatmap heatMap;

        private SegmentSonagrams sonagramGenerator = null;

        private static void SpectrogramWindowInstance_Closed(object sender, EventArgs e)
        {
            SpectrogramWindow spectrogramWindow = sender as SpectrogramWindow;
            Debug.WriteLine("Spectrogram Window closed");
            spectrogramWindow?.sonagramGenerator.Close();
            if (spectrogramWindow.currentPlayListItem != null)
            {
                spectrogramWindow.currentPlayListItem.Close();
            }
            if(spectrogramWindowList!=null && spectrogramWindowList.Contains(spectrogramWindow))
            {
                spectrogramWindowList.Remove(spectrogramWindow);
            }
        }

        private static void SpectrogramWindowInstance_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Debug.WriteLine("Spectrogram window closing");
            
        }

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private double xMin = 0.0d;
        private double xMax = 0.0d;
        private double yMin = 0.0d;
        private double yMax = 0.0d;
        private double zoomedYmax = 0.0d;
        
        
        private void Update()
        {
            if (data == null) return;
            using (new WaitCursor())
            {

                wpfPlot.Plot.Clear();



                var maxdB = (OffsetSlider?.Value) ?? 60.0d;
                var mindB = maxdB - ((GainSlider?.Value) ?? 96.0d);
                //heatMap.Update(data, max: max, min: min);

                xMin = 0.0d;
                xMax = (sonagramGenerator?.duration.TotalSeconds) ?? 1.0d;

                yMin = 0.0d;
                yMax = ((double)((sonagramGenerator?.SampleRate) ?? 384000.0d)) / 2000.0d;
                zoomedYmax = yMax;

                //sonagramGenerator.GetRange(out double mindB, out double maxdB, out double rangedB);

                heatMap = wpfPlot.Plot.AddHeatmap(data, ScottPlot.Drawing.Colormap.GrayscaleR, lockScales: false);
                heatMap.Update(data, max: maxdB, min: mindB);

                var cb = wpfPlot.Plot.AddColorbar(heatMap);

                ch = wpfPlot.Plot.AddCrosshair(0.0d, 0.0d);
                ch.HorizontalLine.IsVisible = false;
                ch.VerticalLine.IsVisible = false;

                heatMap.CellHeight = (double)yMax / (double)data.GetLength(0);
                heatMap.CellWidth = (double)xMax / (double)data.GetLength(1);
                wpfPlot.Plot.AxisAuto();

                wpfPlot.Plot.Margins(x: 0, y: 0);

                List<double> positions = new List<double>();
                List<string> labels = new List<string>();
                for (int i = 0; i < yMax; i += 10)
                {
                    positions.Add(i);
                    labels.Add(i.ToString());
                }

                wpfPlot.Plot.YAxis.ManualTickPositions(positions.ToArray(), labels.ToArray());

               
                wpfPlot.Plot.YAxis.TickLabelFormat(SpectrogramWindow.customTickFormatter);

                wpfPlot.Plot.Grid(enable: true, lineStyle: ScottPlot.LineStyle.Solid, color: System.Drawing.Color.Black);

                wpfPlot.Plot.XAxis.Grid(false);
                wpfPlot.Plot.YAxis.Grid(true);
                wpfPlot.Plot.YAxis.LockLimits(true);

                wpfPlot.Plot.Grid(onTop: true);
                heatMap.Smooth = SmoothingCheckBox.IsChecked ?? false;

                string title = sonagramGenerator?.fileName ?? "";
                if (sonagramGenerator?.segment != null)
                {
                    title += $": {sonagramGenerator.segment.StartDateTime} for {sonagramGenerator.segment.Duration()?.TotalSeconds.ToString() ?? "?"} secs";
                }

                wpfPlot.Plot.Title(title);

                var dims = wpfPlot.Plot.XAxis.Dims;

                UpdateLabelPlot(dims.Min, dims.Max);
                wpfPlot.Refresh();
            }
            
        }

        public static string customTickFormatter(double position)
        {
            if (position < 0)
            {
                return ("");
            }
            else
            {
                return ($"{position:F0}");
            }
        }
        
        /// <summary>
        /// Looks for a text file to go with this .wav file, and if found populates the label track with
        /// appropriate brackets to display the labels
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void UpdateLabelPlot(double xMin,double xMax)
        {
            

            if (sonagramGenerator != null)
            {
                var startOffset=sonagramGenerator.startOffset;
                var file=sonagramGenerator.fileName;
                var textFile = Path.ChangeExtension(file, ".TXT");
                if (!File.Exists(textFile))
                {
                    
                    return;
                }

                TimeSpan displayedStartInFile = TimeSpan.FromSeconds(startOffset) + TimeSpan.FromSeconds(xMin);
                TimeSpan displayedEndInFile = displayedStartInFile + TimeSpan.FromSeconds(xMax - xMin);
                wpfPlot.Plot.YAxis.LockLimits(false);
                var lims = wpfPlot.Plot.GetAxisLimits();
                wpfPlot.Plot.SetAxisLimits(lims.XMin, lims.XMax, lims.YMin - 60, lims.YMax);

                Debug.WriteLine($"Labels:- dispStart={displayedStartInFile}, dispEnd={displayedEndInFile} Lims={lims.YMin},{lims.YMax}");

                var labelList = GetLabels(textFile, displayedStartInFile, displayedEndInFile);
                labelList = labelList.OrderBy(listItem => listItem.start).ToList();
                List<TimeSpan> rowLimits = new List<TimeSpan>();
                rowLimits.Add(labelList[0].start);
                spaceForLabels = 20;
                
                foreach (var label in labelList)
                {
                    int i = 0;
                    for(i=0;i<rowLimits.Count;i++)
                    {
                        if (label.start >= rowLimits[i])
                        {
                            wpfPlot.Plot.AddBracket((label.start - displayedStartInFile).TotalSeconds, -5.0d + (i*-15.0d),
                                (label.end - displayedStartInFile).TotalSeconds, -5.0d+(i*-15.0d), label.label);
                            
                            rowLimits[i] = label.end;
                            
                            break;
                        }
                    }

                    if (i == rowLimits.Count)
                    { // we did not add the label as it overlaps everything
                        rowLimits.Add(label.end);
                        spaceForLabels += 15;
                        var brack=wpfPlot.Plot.AddBracket((label.start - displayedStartInFile).TotalSeconds, -5.0d + (i * -15.0d),
                            (label.end - displayedStartInFile).TotalSeconds, -5.0d + (i * -15.0d), label.label);
                        
                        

                    }




                    Debug.WriteLine($"  label:- {(label.start - displayedStartInFile).TotalSeconds},{(label.end - displayedStartInFile).TotalSeconds} - ''{label.label}'' at -10");
                }


                wpfPlot.Plot.SetAxisLimits(lims.XMin, lims.XMax, lims.YMin -spaceForLabels, lims.YMax);

                wpfPlot.Refresh();
                wpfPlot.Plot.YAxis.LockLimits(true);
            }

        }

        private double spaceForLabels = 30;

        private List<(TimeSpan start, TimeSpan end, string label)> GetLabels(string textFile, TimeSpan displayedStartInFile, TimeSpan displayedEndInFile)
        {
            List<(TimeSpan start, TimeSpan end, string label)> labels = new List<(TimeSpan start, TimeSpan end, string label)>();
            
            var lines = File.ReadAllLines(textFile);
            if(lines!=null && lines.Length > 0)
            {
                string pattern = @"([0-9.]+)\s+([0-9.]+)\s+(.*)";
                foreach(var line in lines)
                {
                    TimeSpan start=displayedStartInFile;
                    TimeSpan end = displayedEndInFile;
                    string text = "";
                    var result=Regex.Match(line, pattern);
                    if (result.Success)
                    {
                        if(result.Groups.Count > 2)
                        {
                            if(double.TryParse(result.Groups[1].Value, out double startSecs))
                            {
                                start=TimeSpan.FromSeconds((double)startSecs);
                            }
                            if(double.TryParse(result.Groups[2].Value, out double endSecs))
                            {
                                end=TimeSpan.FromSeconds((double)endSecs);
                            }
                        }
                        if(result.Groups.Count > 3)
                        {
                            text=result.Groups[3].Value;
                        }
                        var Label = (start: start, end: end, label: text);
                        if(end>displayedStartInFile && start<displayedEndInFile)
                            labels.Add(Label);
                    }
                }
            }

            return (labels);
        }

        private Crosshair ch = null;

       

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

        /// <summary>
        /// Responds to a change in the selected colour scheme
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColourComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selection = ColourComboBox.SelectedItem as string;
            var colour = ScottPlot.Drawing.Colormap.GetColormapByName(selection);
            if(colour != null)
            {
                if (heatMap != null)
                {
                    heatMap.Update(data,colormap: colour);
                    wpfPlot.Plot.AddColorbar(heatMap);
                    wpfPlot.Refresh();
                }
            }
        }

        public void setPlayListItem(PlayListItem newItem)
        {
            currentPlayListItem = newItem;
            if (currentPlayListItem != null)
            {
                currentPlayListItem.SegmentChanged += currentPlayListItem_SegmentChanged;
                currentPlayListItem.ItemClosed += CurrentPlayListItem_ItemClosed;
               // currentPlayListItem.PlayEnded += CurrentPlayListItem_PlayEnded;
                //currentPlayListItem.PlayStarted += CurrentPlayListItem_PlayStarted;
            }
        }

        

        private void CurrentPlayListItem_ItemClosed(object sender, EventArgs e)
        {
            currentPlayListItem = null;
        }

        private PlayListItem currentPlayListItem { get; set; } = null;

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var lims = wpfPlot.Plot.GetAxisLimits();
            if (sonagramGenerator == null) return;
            string filename = sonagramGenerator.segment?.Recording?.GetFileName();
            if (string.IsNullOrWhiteSpace(filename))
            {
                filename = sonagramGenerator.fileName;
            }
            var segmentOffset = sonagramGenerator.segment?.StartOffset;
            if (segmentOffset == null)
            {
                segmentOffset = TimeSpan.FromSeconds(sonagramGenerator.startOffset);
            }
            if (!string.IsNullOrWhiteSpace(filename) && File.Exists(filename))
            {
                TimeSpan offset = (segmentOffset ?? new TimeSpan()) + TimeSpan.FromSeconds(lims.XMin);
                TimeSpan duration = TimeSpan.FromSeconds(lims.XSpan);
                WaveFileReader wfr = new WaveFileReader(filename);
                if (currentPlayListItem == null)
                {
                    var pli = PlayListItem.Create(filename, offset, duration, segmentOffset ?? offset,
                        TimeSpan.FromSeconds(sonagramGenerator?.durationSecs ?? lims.XSpan),
                         wfr.TotalTime, filename, null, hasSpectrogram: true);
                    setPlayListItem(pli);
                    
                    AudioHost.Instance.audioPlayer.AddToPlayList(currentPlayListItem);
                }
                
            }


        }


        private void currentPlayListItem_SegmentChanged(object sender, EventArgs e)
        {
            PlayListItem pli = sender as PlayListItem;
            if (pli != null)
            {
                double desiredStart = pli.startOffset.TotalSeconds- pli.segmentOffset.TotalSeconds ;
                double desiredZoom = pli.segmentDuration.TotalSeconds / pli.playLength.TotalSeconds;

                

                wpfPlot.Plot.SetAxisLimits(xMin: 0.0d, xMax: pli.segmentDuration.TotalSeconds);
                //TimeAxis.Dims.SetAxis(0.0d, pli.segmentDuration.TotalSeconds);
                wpfPlot.Refresh();

                //TimeAxis.Dims.Zoom(2.0d);
                //TimeAxis.Dims.Pan(-1.0d);

                var lims = wpfPlot.Plot.GetAxisLimits();

                var original_axisLimits=wpfPlot.Plot.GetAxisLimits();
                var original_RawPlotLimits = wpfPlot.Plot.GetAxisLimits();

                var halfNewAxis = pli.playLength.TotalSeconds / 2.0d;
                var newAxisCentre = desiredStart + halfNewAxis;
                var AxisPan = newAxisCentre-lims.XCenter ;

                //var PlotUnitsPerSec = original_RawPlotLimits.XMax / original_axisLimits.XMax;

                //wpfPlot.Plot.SetAxisLimits(xMin: desiredStart, xMax: desiredStart + pli.playLength.TotalSeconds, xAxisIndex: 2);
                wpfPlot.Plot.AxisPan(AxisPan);
                

                var zoom = pli.segmentDuration.TotalSeconds / pli.playLength.TotalSeconds;
                wpfPlot.Plot.AxisZoom(xFrac:zoom );
                

                wpfPlot.Refresh();

            }
            
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            var lims = wpfPlot.Plot.GetAxisLimits();
            if (ZoomFreeButton.Content == "Lock")
            {
                ZoomFreeButton_Click(sender, e);
                if (lims.YMax < yMax) zoomedYmax = lims.YMax;
            }
            if (yMax / 32 > zoomedYmax) return;
            wpfPlot.Plot.YAxis.LockLimits(false);
            
            zoomedYmax = (3 * zoomedYmax) / 4;
            wpfPlot.Plot.SetAxisLimitsY(0.0d-spaceForLabels, zoomedYmax);
            wpfPlot.Refresh();
            wpfPlot.Plot.YAxis.LockLimits(true);


        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            var lims = wpfPlot.Plot.GetAxisLimits();
            if (ZoomFreeButton.Content == "Lock")
            {
                ZoomFreeButton_Click(sender, e);
                if(lims.YMax<yMax) zoomedYmax = lims.YMax;
            }
            if(zoomedYmax>=yMax) return;
            var temp = zoomedYmax * 1.5d;
            if (temp > yMax) zoomedYmax = yMax;
            else zoomedYmax = temp;
            wpfPlot.Plot.YAxis.LockLimits(false);
            
            
            wpfPlot.Plot.SetAxisLimitsY(0.0d-spaceForLabels, zoomedYmax);
            wpfPlot.Refresh();
            wpfPlot.Plot.YAxis.LockLimits(true);

        }

        private void ZoomFreeButton_Click(object sender, RoutedEventArgs e)
        {
            if (ZoomFreeButton.Content == "Free")
            {
                wpfPlot.Plot.YAxis.LockLimits(false);
                ZoomFreeButton.Content = "Lock";
            }
            else
            {
                wpfPlot.Plot.YAxis.LockLimits(true);
                ZoomFreeButton.Content = "Free";
            }
        }

        private void wpfPlot_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Debug.WriteLine($"Text entered={e.Text}");
        }
    }
}
