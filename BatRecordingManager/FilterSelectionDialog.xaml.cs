using System.Windows;
using System.Windows.Input;

namespace BatRecordingManager
{
    /// <summary>
    /// Interaction logic for FilterSelectionDialog.xaml
    /// </summary>
    public partial class FilterSelectionDialog : Window
    {
        /// <summary>
        /// frequency in Hz for the biquad high pass filter at Q=1
        /// </summary>
        public int lowCutoff { 
            get { return (((int)hpSpinnerValue)*1000); }
            set { hpSpinnerValue = (decimal)value/1000.0m; }
        }

        /// <summary>
        /// frequency in Hz for the biquad low pass filter at Q=1
        /// </summary>
        public int hiCutoff { 
            get { return (((int)lpSpinnerValue) * 1000); }
            set { lpSpinnerValue = (decimal)value / 1000.0m; }
        }

        private int _sampleRate = 384000;
        public decimal maxFrequency { get; set; } = 192.0m;
        public int sampleRate { 
            get { return _sampleRate; }
            set { _sampleRate = value; hiCutoff = _sampleRate / 2; maxFrequency = lpSpinnerValue; } 
        }

        public bool? lpRunTwice { get; set; }

        public bool? hpRunTwice { get;set; }

        public decimal lpSpinnerValue { get; set; }

        public decimal hpSpinnerValue { get; set; }
        public FilterSelectionDialog()
        {
            lowCutoff = 15000;
            hiCutoff = 192000;
            InitializeComponent();
            this.DataContext = this;
            oldCursor = this.Cursor;
            this.Cursor = Cursors.Arrow;
        }

        private Cursor oldCursor;

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Cursor = oldCursor;
            this.Close();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Cursor = oldCursor;
            this.DialogResult = true;
            this.Close();
        }
    }
}
