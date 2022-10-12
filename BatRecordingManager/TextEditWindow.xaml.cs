using System;
using System.Collections.Generic;
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
    /// Interaction logic for TextEditWindow.xaml
    /// </summary>
    public partial class TextEditWindow : Window
    {
        public TextEditWindow()
        {
            InitializeComponent();
        }

        private void textEditBlock_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled=false;
            if(e.Key == Key.Enter)
            {
                DialogResult=true;
                e.Handled=true;
                this.Close();
            }else if(e.Key == Key.Escape)
            {
                DialogResult = false;
                e.Handled = true;
                this.Close();
            }

        }

        
    }
}
