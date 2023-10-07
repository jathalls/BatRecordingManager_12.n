using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
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
using System.Security.Cryptography;
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

        private bool AxisLock;

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



        internal static void Display(LabelledSegment segment, PlayListItem pli = null)
        {
            if (segment == null && pli == null) return;
            


            string filename = segment?.Recording.GetFileName()??pli?.filename;
            if (!string.IsNullOrWhiteSpace(filename) && File.Exists(filename))
            {
                var start = segment?.StartOffset??pli.startOffset;
                TimeSpan end = (segment?.EndOffset??(pli?.startOffset+pli?.playLength)).Value;
                var spectrogram = new Spectrogram(filename, start, end, 1024, 512);
                Debug.WriteLine("@@@@@ Spectrogram created");


                var spectrogramWindow = new SpectrogramWindow();
                spectrogramWindow.segment = segment;
                spectrogramWindow.spectrogram = spectrogram;
                //spectrogramWindow.segmentSonagram = null;
                spectrogramWindow.Closing += SpectrogramWindowInstance_Closing;
                spectrogramWindow.Closed += SpectrogramWindowInstance_Closed;
                spectrogramWindow.heatMap = null;
                spectrogramWindow.RegenSonagram();

                Debug.WriteLine("Regen returned - About to show spectrogram window");
                spectrogramWindow.Show();
                Debug.WriteLine("Spectrogram window displayed");
                //spectrogramWindow.Update();

                spectrogramWindow.setPlayListItem(null);

                spectrogramWindowList.Add(spectrogramWindow);

            }


        }

        internal static void Display(List<LabelledSegment> segmentSelection)
        {
            if (segmentSelection == null || segmentSelection.Count == 0) return;
            if(spectrogramWindowList!=null && spectrogramWindowList.Count > 0)
            {
                foreach(var sw in spectrogramWindowList)
                {
                    sw.Close();
                }
                spectrogramWindowList.Clear();
            }

            foreach(var segment in segmentSelection)
            {
                Display(segment);
            }
        }

        private void Spectrogram_SpectrumCompleted(object sender, EventArgs e)
        {
            var lims=wpfPlot.Plot.GetAxisLimits();
            heatMap.Update(data);
            wpfPlot.Plot.SetAxisLimits(lims);
            wpfPlot.Refresh();
            var dims = wpfPlot.Plot.XAxis.Dims;
            UpdateLabelPlot(dims.Min, durationSecs);
            Debug.WriteLine("@@@@@ Spectrum Completed and LabelPlot updated");
        }

        private double durationSecs { get; set; }

        private async void Spectrogram_SectionCompletedEvent(object sender, spectrumEventArgs e)
        {
            Debug.WriteLine("@@@@@ Spectrogram section completed");
            bool isFirst = (data == null);
            double[,] newData;
            durationSecs = e.durationSecs;
            if (isFirst)
            {
                data = new double[e.spectrogram.GetLength(0), e.totalFFTs ];
                //data = e.spectrogram;
                int pos = 0;
                for(int i=e.dataStart;i<e.dataEnd && i<data.GetLength(1); i++)
                {
                    for(int f = 0; f < e.spectrogram.GetLength(0); f++)
                    {
                        data[f, i] = e.spectrogram[f, pos];
                    }
                    pos++;
                }

                Update(withLabels: false);
                Debug.WriteLine("@@@@@ Updated");
            }
            else
            {
                

                int pos = 0;
                for (int i = e.dataStart; i < e.dataEnd && i < data.GetLength(1); i++)
                {
                    for (int f = 0; f < e.spectrogram.GetLength(0); f++)
                    {
                        data[f, i] = e.spectrogram[f, pos];
                    }
                    pos++;
                }

                asyncUpdate();
                
            }
            
            
            
            

        }

        private async void asyncUpdate()
        {
            var lims = wpfPlot.Plot.GetAxisLimits();
            await heatmapUpdateAsync();
            //heatMap.Update(data);
            wpfPlot.Plot.SetAxisLimits(lims);
            wpfPlot.Refresh();
            Debug.WriteLine("@@@@@ heatmap Updated and wpfPlot Refreshed");
        }

        public Task heatmapUpdateAsync() => Task.Run(() =>
        {
            heatMap.Update(data);
        });

        private Spectrogram _spectrogram;

        private Spectrogram spectrogram
        {
            get { return _spectrogram; }
            set
            {
                _spectrogram = value;
                _spectrogram.SpectrumCompleted -= Spectrogram_SpectrumCompleted;
                _spectrogram.SpectrumCompleted += Spectrogram_SpectrumCompleted;

                _spectrogram.SectionCompletedEvent -= Spectrogram_SectionCompletedEvent;
                _spectrogram.SectionCompletedEvent += Spectrogram_SectionCompletedEvent;

                Debug.WriteLine("@@@@@ Spectrogram assigned");

            }
        }

        

        //private double[,] rawData;
        private double[,] data;
        private ScottPlot.Plottable.Heatmap heatMap;

        

        private LabelledSegment segment { get; set; }

        private static void SpectrogramWindowInstance_Closed(object sender, EventArgs e)
        {
            SpectrogramWindow spectrogramWindow = sender as SpectrogramWindow;
            Debug.WriteLine("Spectrogram Window closed");
            //spectrogramWindow?.segmentSonagram?.Close();
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
            var window = sender as SpectrogramWindow;
            if(window?.hasChanged??false)
            {
                //var result = MessageBox.Show("Save Changes?", "Labels Have been altered", MessageBoxButtons.YesNo);
                //var mb = new System.Windows.MessageBox.Show("Save changes?", "Labels have been altered", MessageBoxButton.YesNoCancel);


                var result = System.Windows.MessageBox.Show("Save Changes?","Label has been altered!",MessageBoxButton.YesNoCancel);
                if (result==MessageBoxResult.Yes)
                {
                    window.SaveChanges();
                    
                }
                else
                {
                    Debug.WriteLine("Don't save changes");
                }

            }
            
        }

        /// <summary>
        /// If the sonagram is of a segment rather than a file, modifies the labelled segment and writes
        /// the changes back to the database, leaving the associated text file unchanged.  This follows the practice adopted
        /// in editing a labelled segment in the BRM view window.
        /// </summary>
        /// 
        private void SaveChanges()
        {
            
            
            var newLabel = "";
            foreach(var brack in brackets)
            {
                newLabel += $"\n{brack.Label}";
            }
            while (newLabel.StartsWith("\n", System.StringComparison.CurrentCulture)) { newLabel = newLabel.Substring(1); }
            if (segment != null)
            {
                segment.Comment = newLabel;
                
                DBAccess.UpdateLabelledSegment(segment);// fails to update the bat list
                segment.CommentModified();
            }
            
        }

        public event EventHandler<EventArgs> SegmentChanged;
        protected virtual void OnSegmentChanged(EventArgs e)=>SegmentChanged?.Invoke(this, e);

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private double xMin;
        private double xMax;
        private double yMin;
        private double yMax;
        private double zoomedYmax;
        private Crosshair ch2;
        private static (double x, double y) staticPos = (0, 0);
        private Text txt;


        private void Update(bool withLabels=true)
        {
            if (data == null)
            {
                Debug.WriteLine("EEEEEEEE No data to Update the plot");
                return;
            }
                
            using (new WaitCursor())
            {

                wpfPlot.Plot.Clear();



                var maxdB = (OffsetSlider?.Value) ?? 60.0d;
                var mindB = maxdB - ((GainSlider?.Value) ?? 96.0d);
                //heatMap.Update(data, max: max, min: min);

                xMin = 0.0d;
                xMax = durationSecs;

                yMin = 0.0d;
                yMax =  (spectrogram?.sampleRate??384000) / 2000.0d;
                zoomedYmax = yMax;

                //sonagramGenerator.GetRange(out double mindB, out double maxdB, out double rangedB);

                heatMap = wpfPlot.Plot.AddHeatmap(data, ScottPlot.Drawing.Colormap.GrayscaleR, lockScales: false);
                Debug.WriteLine("Heatmap added");
                //heatMap.Update(data, max: maxdB, min: mindB);
                Debug.WriteLine($"heatmap Updataed {data.GetLength(0)} {data.GetLength(1)}");

                disableSliders = true;
                var cmMax = heatMap.ColormapMax;
                var cmMin = heatMap.ColormapMin;
                GainSlider.Maximum = (cmMax-cmMin)*2;
                GainSlider.Minimum = (cmMax-cmMin)/2;
                GainSlider.Value = (cmMax - cmMin);
                OffsetSlider.Maximum = cmMax * 2;
                OffsetSlider.Minimum = cmMin;
                OffsetSlider.Value = cmMax;
                disableSliders = false;
                

                var cb = wpfPlot.Plot.AddColorbar(heatMap);

                ch = wpfPlot.Plot.AddCrosshair(0.0d, 0.0d);
                ch.HorizontalLine.IsVisible = true;
                ch.VerticalLine.IsVisible = true;
                ch.HorizontalLine.PositionFormatter = customFormatterY;
                ch.VerticalLine.PositionFormatter = custonFormatterX;
                ch.HorizontalLine.PositionLabelOppositeAxis = true;
                ch.VerticalLine.PositionLabelOppositeAxis = true;

                txt = wpfPlot.Plot.AddText("_", 0, 0);

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

                string title = spectrogram.fileName;
                if (segment != null)
                {
                    title += $": {segment.StartDateTime} for {segment.Duration()?.TotalSeconds.ToString() ?? "?"} secs";
                }
                

                wpfPlot.Plot.Title(title+"\n'");

                double zoom = durationSecs / 5.0d;
                
                Debug.WriteLine($"dur={durationSecs},  zoom={zoom}, pre={wpfPlot.Plot.XAxis.Dims.Min},{wpfPlot.Plot.XAxis.Dims.Max}");
                
                wpfPlot.Plot.AxisZoom(xFrac: zoom);
                Debug.WriteLine($"postzoom={wpfPlot.Plot.XAxis.Dims.Min},{wpfPlot.Plot.XAxis.Dims.Max}");

                //double pan = -wpfPlot.Plot.XAxis.Dims.Min;
                double pan = 0.0d;
                //wpfPlot.Plot.AxisPan(dx: pan);
                //var dims = wpfPlot.Plot.XAxis.Dims;
                var Min = 0.0d;
                var Max = Math.Min(durationSecs, 5.0d);
                wpfPlot.Plot.SetAxisLimitsX(Min, Max);
                Debug.WriteLine($"postpan={pan} to {wpfPlot.Plot.XAxis.Dims.Min},{wpfPlot.Plot.XAxis.Dims.Max}");
                

                
                if (withLabels)
                {
                    var dims = wpfPlot.Plot.XAxis.Dims;
                    UpdateLabelPlot(dims.Min, durationSecs);
                }
                wpfPlot.Refresh();
                Debug.WriteLine("Plot refreshed - Update complete");
            }
            
        }

        bool disableSliders = false;

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
            

            if ( spectrogram!=null)
            {
                var startOffset = spectrogram.start.TotalSeconds;
                var file=spectrogram.fileName;
                var textFile = Path.ChangeExtension(file, ".TXT");
                if (!File.Exists(textFile))
                {
                    
                    return;
                }

                TimeSpan displayedStartInFile = TimeSpan.FromSeconds(startOffset);
                TimeSpan displayedEndInFile = displayedStartInFile + TimeSpan.FromSeconds(xMax);
                wpfPlot.Plot.YAxis.LockLimits(false);
                var lims = wpfPlot.Plot.GetAxisLimits();
                wpfPlot.Plot.SetAxisLimits(lims.XMin, lims.XMax, lims.YMin - 60, lims.YMax);
                // set to allow space for the label track below the spectrogram track


                List<(TimeSpan start,TimeSpan end,string label)> labelList = null;
                Debug.WriteLine($"Labels:- dispStart={displayedStartInFile}, dispEnd={displayedEndInFile} Lims={lims.YMin},{lims.YMax}");
                if (segment == null)
                {
                    labelList = GetLabels(textFile, displayedStartInFile, displayedEndInFile);
                }
                else
                {
                    labelList = new List<(TimeSpan start, TimeSpan end, string label)>();
                    labelList.Add(new(displayedStartInFile, displayedEndInFile, segment.Comment));
                }
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
                            var brack=wpfPlot.Plot.AddBracket((label.start - displayedStartInFile).TotalSeconds, -5.0d + (i*labelHeight),
                                (label.end - displayedStartInFile).TotalSeconds, -5.0d+(i*labelHeight), label.label);
                            brackets.Add(brack);
                            rowLimits[i] = label.end;
                            
                            break;
                        }
                    }

                    if (i == rowLimits.Count)
                    { // we did not add the label as it overlaps everything
                        rowLimits.Add(label.end);
                        spaceForLabels += 15;
                        var brack=wpfPlot.Plot.AddBracket((label.start - displayedStartInFile).TotalSeconds, -5.0d + (i * labelHeight),
                            (label.end - displayedStartInFile).TotalSeconds, -5.0d + (i * labelHeight), label.label);
                        brackets.Add(brack);
                        

                    }




                    Debug.WriteLine($"  label:- {(label.start - displayedStartInFile).TotalSeconds},{(label.end - displayedStartInFile).TotalSeconds} - ''{label.label}'' at -10");
                }


                wpfPlot.Plot.SetAxisLimits(lims.XMin, lims.XMax, lims.YMin -spaceForLabels, lims.YMax);

                wpfPlot.Refresh();
                wpfPlot.Plot.YAxis.LockLimits(true);
            }

        }

        private List<Bracket> brackets = new List<Bracket>();

        

        private double spaceForLabels = 30;
        private const  double labelHeight= -15.0d;

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

        private Crosshair ch;



        private void OffsetSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (OffsetSlider != null && heatMap != null && !disableSliders)
            {
                var max= (OffsetSlider?.Value) ;
                var min = max - ((GainSlider?.Value) );
                heatMap.Update(data, max: max, min: min);
                wpfPlot.Refresh();
            }
        }

        private void GainSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            if (GainSlider != null && OffsetSlider != null && heatMap != null && !disableSliders)
            {
                var max = (OffsetSlider?.Value) ;
                var min = max - ((GainSlider?.Value) );
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

        /// <summary>
        /// Recalculates the spectrogram using segmentSonagram or Spectrogram as appropriate
        /// </summary>
        private async void RegenSonagram()
        {
            Debug.WriteLine("Regen");
            //Debug.WriteLine($"FT Size changed to {FtSizeListbox.SelectedItem.ToString()}");
            int ftSize = 1024;
            int ftAdvance = 512;

            data = null;

            if (FtSizeListbox == null || FtSizeListbox.SelectedIndex < 0) return;

            string selection = (FtSizeListbox?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (selection == null) return;
            if (!int.TryParse(selection, out ftSize) || ftSize <= 0) return;
            ftAdvance = ftSize / 2;

            selection = (FtAdvanceListbox?.SelectedItem as ComboBoxItem)?.Content?.ToString();
            if (selection == null) return;
            if (selection.EndsWith("%", System.StringComparison.CurrentCulture)) selection = selection.Substring(0, selection.Length - "%".Length);
            if (!double.TryParse(selection, out double ftAdvancePC)) ftAdvancePC = ftSize / 2;
            switch (ftAdvancePC)
            {
                case 12.5d: ftAdvance = ftSize / 8; break;
                case 25.0d: ftAdvance = ftSize / 4; break;
                case 50.0d: ftAdvance = ftSize / 2; break;
                case 75.0d: ftAdvance = (ftSize / 4) * 3; break;
                case 100.0d: ftAdvance = ftSize; break;
                default: return;
            }



            if (spectrogram != null)
            {
                Debug.WriteLine("@@@@@ awaiting GetSpectrogramAsync");
                int errcode=await spectrogram?.GetSpectrogramAsync();
                Debug.WriteLine($"@@@@@ returned from GetSpectrogramAsync {errcode}");

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

        private PlayListItem currentPlayListItem { get; set; }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            var lims = wpfPlot.Plot.GetAxisLimits();
            if (segment == null) return;
            string filename = segment?.Recording?.GetFileName();
            if (string.IsNullOrWhiteSpace(filename))
            {
                filename = spectrogram?.fileName;
            }
            var segmentOffset = segment?.StartOffset;
            if (segmentOffset == null)
            {
                segmentOffset = TimeSpan.FromSeconds(0);
            }
            if (!string.IsNullOrWhiteSpace(filename) && File.Exists(filename))
            {
                TimeSpan offset = (segmentOffset ?? new TimeSpan()) + TimeSpan.FromSeconds(lims.XMin);
                TimeSpan duration = TimeSpan.FromSeconds(lims.XSpan);
                WaveFileReader wfr = new WaveFileReader(filename);
                if (currentPlayListItem == null)
                {
                    var pli = PlayListItem.Create(filename, offset, duration, segmentOffset ?? offset,
                        (segment?.Duration())??TimeSpan.FromSeconds(lims.XSpan),
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
            if ((ZoomFreeButton.Content as string) == "Lock")
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
            if ((ZoomFreeButton.Content as string) == "Lock")
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
            if ((ZoomFreeButton.Content as string) == "Free")
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

        private void wpfPlot_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        private bool hasChanged = false;

        private void wpfPlot_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
            //Debug.WriteLine($"Mouse down at {e.GetPosition(wpfPlot).X},{e.GetPosition(wpfPlot).Y}");

            var mouseAt = wpfPlot.GetMouseCoordinates();

            bool bracketed = false;

            foreach (var brack in brackets)
            {


                if (mouseAt.x > brack.X1 && mouseAt.x < brack.X2 && mouseAt.y < brack.Y1 && mouseAt.y > (brack.Y1 + labelHeight))
                {
                    Debug.WriteLine($"hit at {brack.Label}");
                    bracketed = true;

                    var brackPix=wpfPlot.Plot.GetPixel((brack.X1 + brack.X2) / 2, brack.Y1);
                    var mousePos=e.GetPosition(wpfPlot);

                    Debug.WriteLine($"clickd = {mouseAt.x},{mouseAt.y}  brack = {brack.X1}-{brack.X2},{brack.Y1},{brack.Y2}");
                    var editable = new TextEditWindow();
                    editable.textEditBlock.Text = brack.Label;
                    editable.Owner = this;
                    editable.Left = this.Left + mousePos.X-(editable.Width/2.0d);
                    editable.Top = this.Top + mousePos.Y;
                    if (editable.ShowDialog() ?? false)
                    {
                        brack.Label = editable.textEditBlock.Text;
                        Debug.WriteLine($"new label={brack.Label}");
                        hasChanged = true;
                        wpfPlot.Refresh();
                    }
                }
            }
            if (!bracketed)
            {
                var limits = wpfPlot.Plot.GetAxisLimits();
                if (ch2 != null)
                {
                    wpfPlot.Plot.Remove(ch2);
                    
                    var here = wpfPlot.GetMouseCoordinates();
                    
                    var YMin = limits.YMin;
                    if (limits.YMin < 0) YMin = 0;
                    if (here.x < limits.XMax && here.x > limits.XMin && here.y > YMin && here.y < limits.YMax)
                    {
                        // the current point is inside the plottable area
                        var xValue = here.x - staticPos.x;
                        var yValue = here.y - staticPos.y;
                        tableData.Add(Math.Abs(xValue), Math.Abs(yValue));
                        Debug.WriteLine($"{xValue:F3} => {tableData.intervalMean:F3} of {tableData.Count} +/- {tableData.intervalSD:F3}");
                    }
                    staticPos = (limits.XMin, limits.YMin > 0 ? limits.YMin : 0);
                    ch2 = null;
                    if (txt != null)
                    {
                        wpfPlot.Plot.Remove(txt);
                        txt = null;
                    }
                }
                else
                {
                    var label = $"{mouseAt.x:F3}s, {mouseAt.y:F1}kHz";
                    if (txt == null) txt = wpfPlot.Plot.AddText(label, mouseAt.x, mouseAt.y);

                    ch2 = wpfPlot.Plot.AddCrosshair(mouseAt.x, mouseAt.y);
                    txt.Label = label;
                    txt.X = mouseAt.x;
                    txt.Y = mouseAt.y;
                    txt.FontBold = true;
                    txt.Color = System.Drawing.Color.Red;
                    txt.BackgroundColor = System.Drawing.Color.White;
                    txt.BackgroundFill = true;
                    txt.Alignment = ScottPlot.Alignment.LowerLeft;

                    staticPos = mouseAt;
                    this.KeyDown -= SpectrogramWindow_KeyDown;
                    this.KeyDown += SpectrogramWindow_KeyDown;

                }
                wpfPlot.Refresh();
            }

            
        }

        private void SpectrogramWindow_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (ch2 != null && txt != null && e.Key == Key.T)
            {
                e.Handled = true;
                var here = wpfPlot.GetMouseCoordinates();
                var limits = wpfPlot.Plot.GetAxisLimits();
                var YMin = limits.YMin;
                if (limits.YMin < 0) YMin = 0;
                if (here.x < limits.XMax && here.x > limits.XMin && here.y > YMin && here.y < limits.YMax)
                {
                    // the current point is inside the plottable area
                    var xValue = here.x - staticPos.x;
                    var yValue = here.y - staticPos.y;
                    tableData.Add(xValue, yValue);
                    Debug.WriteLine($"{xValue:F3} of {tableData.Count} +/- {tableData.intervalSD:F3}");
                }
            }
        }

        private static string custonFormatterX(double position)
        {
            var x = position - staticPos.x;
            string text = "s";
            if (x < 1.0d)
            {
                x = x * 1000;
                text = "ms";
            }
            return ($"{x:F3} {text}");
        }

        private static string customFormatterY(double position)
        {
            return ($"{(int)(position-staticPos.y)}kHz");
        }

        /// <summary>
        /// Updates the position of the cross hairs and the displayed co-ordinates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wpfPlot_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            var mouseAt = wpfPlot.GetMouseCoordinates();
            
            if (ch2 != null && txt!=null)
            {
                ch.X = mouseAt.x;
                ch.Y = mouseAt.y;

                var dx=ch.X - ch2.X;
                var dy=Math.Abs(ch.Y - ch2.Y);
                txt.X = ch.X;
                txt.Y = ch.Y;
                
                txt.Label = dx < 1 ? $"   {dx*1000:F0}ms, {ch.Y:F1}kHz ({dy:F1}kHz)" :
                    $"{dx:F3}s, {ch.Y:F1}kHz ({dy:F1}kHz)";

                wpfPlot.Refresh();
                e.Handled = false;

            }

            


        }

        /// <summary>
        /// Takes the data in the current window, and filters hard at the displayed frequency limits (if zoomed in)
        /// Then performs an autocorrelation on the envelope of the resulting waveform.  Finally displays a table
        /// showing the values of the dominant peaks.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AnalysisButton_Click(object sender, RoutedEventArgs e)
        {
            Limits limits = GetDisplayedLimits();
            TimeAnalysisWindow.Create(limits);
        }

        /// <summary>
        /// Returns a Limits with the name of the current wav file, and the frequency limits of the currently (zoomed) displaye
        /// and the start location of the zoomed portion in the file, and the length of the 
        /// </summary>
        /// <returns></returns>
        private Limits GetDisplayedLimits()
        {
            Limits limits = new Limits();
            limits.filename = spectrogram?.fileName;
            var plotLimits=wpfPlot.Plot.GetAxisLimits();
            limits.fMin = plotLimits.YMin;
            limits.fMax = plotLimits.YMax;
            TimeSpan start = TimeSpan.FromSeconds(segment?.StartOffset.TotalSeconds??0.0d + plotLimits.XMin);
            limits.startInFile = start;
            limits.length = TimeSpan.FromSeconds(plotLimits.XMax - plotLimits.XMin);
            return (limits);
        }

        TableData tableData = new TableData();

        private void wpfPlot_PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            e.Handled = false;
            if(ch2!=null && txt!=null && e.Key == Key.T)
            {
                e.Handled = true;
                var here = wpfPlot.GetMouseCoordinates();
                var limits = wpfPlot.Plot.GetAxisLimits();
                var YMin = limits.YMin;
                if (limits.YMin < 0) YMin = 0;
                if(here.x<limits.XMax && here.x>limits.XMin && here.y>YMin && here.y < limits.YMax)
                {
                    // the current point is inside the plottable area
                    var xValue = here.x - staticPos.x;
                    var yValue= here.y - staticPos.y;
                    tableData.Add(xValue, yValue);
                    Debug.WriteLine($"{xValue} of {tableData.Count} +/- {tableData.intervalSD}");
                }
            }
        }
    }

    public class Limits
    {
        public string filename { get; set; } = "";
        public TimeSpan startInFile { get; set; }
        public TimeSpan length { get; set; }
        public double fMin { get; set; }
        public double fMax { get; set; } = 192.0d;

    }

    
}
