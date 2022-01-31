
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BatRecordingManager
{
    /// <summary>
    /// Interaction logic for SelectOneDialog.xaml
    /// </summary>
    public partial class SelectOneDialog : Window
    {

        public ObservableCollection<string> itemList { get; set; } = new ObservableCollection<string>();

        protected string selectedItem { get; set; } = "";
        public SelectOneDialog()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public void SetItems(List<string> items)
        {
            if (itemList == null) itemList = new ObservableCollection<string>();
            //itemList.Clear();
            itemList = new ObservableCollection<string>(items);
        }

        private void SelectionListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectButton != null)
            {
                SelectButton.IsEnabled = true;
                SelectButton.IsDefault = true;
            }
            if (SelectionListBox.SelectedItem == null)
            {
                selectedItem = "";
                if (SelectButton != null)
                {
                    SelectButton.IsEnabled = false;
                }
            }
            else
            {
                selectedItem = SelectionListBox.SelectedItem as string;
                if (SelectButton != null)
                {
                    SelectButton.IsEnabled = true;
                }
            }
        }

        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        internal string GetSelectedItem()
        {
            return (selectedItem);
        }
    }
}
