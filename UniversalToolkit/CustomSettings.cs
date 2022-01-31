using Invisionware.Settings;
using Invisionware.Settings.Sinks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UniversalToolkit
{
    public static class UTSettings
    {
        public static CustomSettings getSettings()
        {
            string settingsPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Echolocation\WinBLP\");
            settingsPath = Path.Combine(settingsPath, "customSettings.json");
            var settingsConfig = new SettingsConfiguration().WriteTo.JsonNet(settingsPath).ReadFrom.JsonNet(settingsPath);
            var SettingsMgr = settingsConfig.CreateSettingsMgr<ISettingsObjectMgr>();
            CustomSettings settings = new CustomSettings();
            try
            {
                settings = SettingsMgr.ReadSettings<CustomSettings>();
            }
            catch (Exception)
            {
                settings = new CustomSettings();
                SettingsMgr.WriteSettings<CustomSettings>(settings);
            }
            Debug.WriteLine($"Read settings with scale={settings.Spectrogram.scale}");

            return (settings);
        }

        public static void setSettings(CustomSettings settings)
        {
            string settingsPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    @"Echolocation\WinBLP\");
            settingsPath = Path.Combine(settingsPath, "customSettings.json");
            var settingsConfig = new SettingsConfiguration().WriteTo.JsonNet(settingsPath).ReadFrom.JsonNet(settingsPath);
            var SettingsMgr = settingsConfig.CreateSettingsMgr<ISettingsObjectMgr>();
            Debug.WriteLine($"Write Settings scale={settings.Spectrogram.scale}");
            SettingsMgr.WriteSettings<CustomSettings>(settings);
        }
    }

    public class CustomSettings
    {
        public bool changed = false;
        public ReferenceCall call { get; set; }
        public (double min, double mean, double max) fBandwidth { get; set; } = (0, 0, 0);
        public (double min, double mean, double max) fEnd { get; set; } = (0, 0, 0);
        public (double min, double mean, double max) fHeel { get; set; } = (0, 0, 0);
        public (double min, double mean, double max) fKnee { get; set; } = (0, 0, 0);
        public (double min, double mean, double max) fPeak { get; set; } = (0, 0, 0);
        public (double min, double mean, double max) fStart { get; set; } = (0, 0, 0);
        public CustomSettingsSpectrogram Spectrogram { get; set; } = new CustomSettingsSpectrogram();
        public (double min, double mean, double max) tDuration { get; set; } = (0, 0, 0);
        public (double min, double mean, double max) tInterval { get; set; } = (0, 0, 0);

        public CustomSettingsAnalysisFW AnalysisFW { get; set; } = new CustomSettingsAnalysisFW();

        public CustomSettingsPassAnalysis PassAnalysis { get; set; }=new CustomSettingsPassAnalysis();
    }

    public class CustomSettingsPassAnalysis
    {
        public float envelopeLeadInMS { get; set; } = 0.2f;

        public float envelopeLeadOutMS { get; set; } = 1.0f;


    }

    public class CustomSettingsSpectrogram
    {
        public int dBScale { get; set; } = 10;
        public int FFTAdvance { get; set; } = 512;
        public int FFTSize { get; set; } = 1024;
        public int intensity { get; set; } = 5;

        public int maxFrequency { get; set; } = 120000;
        public double scale { get; set; } = 8000.0d;
    }

    public class CustomSettingsAnalysisFW
    {
        public string currentVersion { get; set; }

        public float envelopeThresholdFactor { get; set; } = 1.5f;

        public float spectrumThresholdFactor { get; set; } = 1.5f;

        public float envelopeLeadInMS { get; set; } = 0.2f;

        public float envelopeLeadOutMS { get; set; } = 1.0f;

        public int spectrumLeadInsamples { get; set; } = 4;

        public int spectrumLeadOutSample { get; set; } = 5;

        public int fftSize { get; set; } = 1024;

        public string initialDirectory { get; set; }

        public bool enableFilter { get; set; } 

        public bool enableFilterDefault { get; set; } = true;

        public CustomSettingsAnalysisFW()
        {
            currentVersion= Assembly.GetExecutingAssembly().GetName().Version.ToString();
            initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            enableFilter = enableFilterDefault;
        }

        public void SetDefaultFilter(bool newDefault)
        {
            enableFilterDefault = newDefault;
        }
    }
}