// *  Copyright 2016 Justin A T Halls
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

 
using Mm.ExportableDataGrid;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BatRecordingManager
{
    /// <summary>
    ///     Interaction logic for ReportMaster.xaml
    ///     This class is the base class for specific report types.
    /// </summary>
    public abstract partial class ReportMaster : UserControl
    {
        /// <summary>
        ///     abstract generic list for the specific type of report data
        /// </summary>
        public abstract string tabHeader { get; }



        /// <summary>
        ///     Generic SetData for each tabbed instance of ReportMaster
        /// </summary>
        /// <param name="reportBatStatsList"></param>
        /// <param name="reportSessionList"></param>
        /// <param name="reportRecordingList"></param>
        public abstract void SetData(ObservableCollection<BatStatistics> reportBatStatsList,
            ObservableCollection<RecordingSession> reportSessionList,
            ObservableCollection<Recording> reportRecordingList);

        internal void Export(CsvExporter exporter, string fileName)
        {
            ReportDataGrid.ExportUsingRefection(exporter, fileName);
        }

        /// <summary>
        ///     default constructor
        /// </summary>
        protected ReportMaster()
        {
            InitializeComponent();
            ReportDataGrid.EnableColumnVirtualization = true;
            ReportDataGrid.EnableRowVirtualization = true;
        }

        /// <summary>
        ///     Generic column creation for versatile DataGrids
        /// </summary>
        /// <param name="header"></param>
        /// <param name="bindingSource"></param>
        /// <param name="visibility"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        protected DataGridTextColumn CreateColumn(string header, string bindingSource, Visibility visibility = Visibility.Visible,
            string converter = "")
        {
            var column = new DataGridTextColumn { Header = header };
            var bind = new Binding(bindingSource);
            if (!string.IsNullOrWhiteSpace(converter))
            {
                switch (converter)
                {
                    case "ShortTime_Converter":
                        bind.Converter = new ShortTimeConverter();
                        break;

                    case "ShortDate_Converter":
                        bind.Converter = new ShortDateConverter();
                        break;

                    case "GPSConverter":
                        bind.Converter = new GPSConverter();
                        break;

                    case "MapRefConverter":
                        bind.Converter = new MapRefConverter();
                        break;

                    case "SessionStartDateTimeConverter":
                        bind.Converter = new SessionStartDateTimeConverter();
                        break;

                    case "SessionEndDateTimeConverter":
                        bind.Converter = new SessionEndDateTimeConverter();
                        break;

                    case "BSPassesConverter":
                        bind.Converter = new BSPassesConverter();
                        break;

                    default:
                        break;
                }
            }

            column.Binding = bind;
            column.Visibility = visibility;
            return column;
        }

        /// <summary>
        ///     Sets the header text into the headerTextBox and returns it to be put into the
        ///     DataGrid.  Adds bat passes summary for the session.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        protected string SetHeaderText(RecordingSession session)
        {
            var footnote = @"
* NB Grid references marked with * are session locations, others are for the start of the recording
";
            var result = session.SessionTag + "\n" + session.SessionNotes.Replace(',', ';') + "\n";

            var summary = Tools.GetSessionSummary(session);
            foreach (var item in summary) result += item.Replace(',', ';') + "\n";
            var metaData = session.Recordings.First()?.getFormattedMetadata();
            if (string.IsNullOrWhiteSpace(metaData)) metaData = "";
            metaData = metaData.Replace(',', ';');
            HeaderTextBox.Text += result + footnote + "***************************************\n" + metaData + "\n";

            return result;
        }

        //protected ObservableCollection<BatStatistics> ReportBatStatsList { get; set; } = new ObservableCollection<BatStatistics>();
        //protected ObservableCollection<RecordingSession> ReportSessionList { get; set; } = new ObservableCollection<RecordingSession>();
        //protected ObservableCollection<Recording> ReportRecordingList { get; set; } = new ObservableCollection<Recording>();
        private void ReportDataGrid_ColumnReordered(object sender, DataGridColumnEventArgs e)
        {
        }
    }
}