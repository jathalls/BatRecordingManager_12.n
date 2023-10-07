using Avalonia.Controls;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Diagnostics;

namespace BatRecordingMaker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            doTest();
        }

        public void doTest()
        {
            var devices = new MMDeviceEnumerator();
            foreach(var device in devices.EnumerateAudioEndPoints(DataFlow.Capture,DeviceState.Active))
            {
                Debug.WriteLine($"{device.ID}: {device.DeviceFriendlyName} / {device.FriendlyName}");
            }
            
        }
    }
}
