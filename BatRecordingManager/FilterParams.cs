using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatRecordingManager
{
    /// <summary>
    /// A simple class to hold the parameters for high and low pass filters to be applied to
    /// waveforms before generating spectrograms or analysing pulses
    /// </summary>
    internal class FilterParams
    {
        public int HighPassFilterFrequency { get; set; } = 15000;
        public int LowPassFilterFrequency { get; set; } = 192000;

        public double FilterQ { get; set; } = 1.0d;

        public int HighPassFilterIterations{get;set;}=1;
        public int LowPassFilterIterations{get;set;}=1;
    }
}
