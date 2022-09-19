using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace UniversalToolkit
{
    /// <summary>
    /// Interaction logic for WPFTimePicker.xaml
    /// </summary>
    public partial class WPFShortTimePicker : UserControl
    {
        public WPFShortTimePicker()
        {
            InitializeComponent();
        }
        public TimeSpan Value
        {
            get { return (TimeSpan)GetValue(ValueProperty); }
            set 
            { 
                if(value!=Value)
                    SetValue(ValueProperty, value); 
            }
        }
        public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(TimeSpan), typeof(WPFShortTimePicker),
        new UIPropertyMetadata(DateTime.Now.TimeOfDay, new PropertyChangedCallback(OnValueChanged)));

        private static void OnValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            WPFShortTimePicker control = obj as WPFShortTimePicker;
            //var Hours = ((TimeSpan)e.NewValue).Hours;
            control.Minutes = (int)Math.Floor(((TimeSpan)e.NewValue).TotalMinutes);
            control.Seconds = (double)((TimeSpan)e.NewValue).Seconds + (double)((TimeSpan)e.NewValue).Milliseconds / 1000.0d;
            //control.Seconds += (double)((TimeSpan)e.NewValue).Milliseconds / 1000.0d;
        }

       

        public int Minutes
        {
            get { return (int)GetValue(MinutesProperty); }
            set 
            { 
                if(value!=Minutes)
                    SetValue(MinutesProperty, value); 
            }
        }
        public static readonly DependencyProperty MinutesProperty =
        DependencyProperty.Register("Minutes", typeof(int), typeof(WPFShortTimePicker),
        new UIPropertyMetadata(0, new PropertyChangedCallback(OnTimeChanged)));

        public double Seconds
        {
            get { return (double)GetValue(SecondsProperty); }
            set 
            { 
                if(value!=Seconds)
                     SetValue(SecondsProperty, value); 
            }
        }

        public static readonly DependencyProperty SecondsProperty =
        DependencyProperty.Register("Seconds", typeof(double), typeof(WPFShortTimePicker),
        new UIPropertyMetadata(0.0d, new PropertyChangedCallback(OnTimeChanged)));

        public bool IsReadOnly
        {
            get { return ((bool)GetValue(IsReadOnlyProperty)); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        public static DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(WPFShortTimePicker),
                new UIPropertyMetadata(false));

        private static bool inTimeChanged = false;

        private static void OnTimeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if (!inTimeChanged)
            {
                inTimeChanged = true;
                WPFShortTimePicker control = obj as WPFShortTimePicker;
                var totalSecs = control.Seconds + (control.Minutes * 60.0d);
                control.Value = TimeSpan.FromSeconds(totalSecs);
                inTimeChanged = false;
            }
        }

        private void Down(object sender, KeyEventArgs args)
        {
            if (!IsReadOnly)
            {
                switch (((Grid)sender).Name)
                {
                    case "sec":
                        if (args.Key == Key.Up)
                        {
                            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                            {
                                this.Seconds += 0.1d;
                            }
                            else if (Keyboard.IsKeyDown(Key.RightCtrl))
                            {
                                this.Seconds += 0.01d;
                            }
                            else
                            {
                                this.Seconds += 1.0d;
                            }
                        }
                        if (args.Key == Key.Down)
                            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                            {
                                this.Seconds -= 0.1d;
                            }
                            else if (Keyboard.IsKeyDown(Key.RightCtrl))
                            {
                                this.Seconds -= 0.01d;
                            }
                            else
                            {
                                this.Seconds-=1.0d;
                            }
                        break;

                    case "min":
                        if (args.Key == Key.Up)
                            this.Minutes++;
                        if (args.Key == Key.Down)
                            this.Minutes--;
                        break;

                    
                }
            }
        }

    }
}

