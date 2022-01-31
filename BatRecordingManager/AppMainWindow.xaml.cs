﻿using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace BatRecordingManager
{
    /// <summary>
    /// Interaction logic for AppMainWindow.xaml
    /// </summary>
    public partial class AppMainWindow : Window
    {
        public AppMainWindow()
        {
            try
            {
                InitializeComponent();
                DataContext = this;
            }catch(Exception ex)
            {
                Tools.ErrorLog($"AppMainWindow-{ex.Message}");
            }
        }

        public void DisplayControl(UserControl controlToDisplay)
        {
            AppMainWindoPanel.Children.Clear();

            (controlToDisplay as AppFilter).CloseClicked += AppMainWindow_CloseClicked;

            AppMainWindoPanel.Children.Add(controlToDisplay);

        }

        private void AppMainWindow_CloseClicked(object sender, AppFilter.CloseClickedEventArgs e)
        {
            Debug.WriteLine("Event args are " + e.value);
            Close();
        }
    }
}
