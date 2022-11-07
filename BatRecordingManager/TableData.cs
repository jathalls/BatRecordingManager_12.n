using MathNet.Numerics.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatRecordingManager
{
    /// <summary>
    /// Stores and organizes the measurement data from SpectrogramWindow so that it can be displayed ina DataTable control
    /// dataGrid
    /// </summary>
    internal class TableData
    {
        public List<double> intervals { get; set; } =new List<double>();
        public List<double> frequencyRanges { get; set; } = new List<double>();

        public double intervalMean
        {
            get
            {
                if (intervals.Count > 0)
                    return (intervals.Mean());
                else return 0;
            }
        }

        public double intervalMax
        {

            get
            {
                if (intervals.Count > 0)
                    return (intervals.Max());
                else return 0;
            }
        }

        public double intervalMin 
        { 
            get 
            {
                if (intervals.Count > 0)
                    return (intervals.Min());
                else return 0;
            } 
        }

        public double intervalSD
        {
            get
            {
                if(intervals.Count>3)
                    return intervals.StandardDeviation();
                else return (intervalMax-intervalMin)/ 2;
            }
        }

        public double freqMin
        {
            get
            {
                if (frequencyRanges.Count > 0) return (frequencyRanges.Min());
                else return 0.0d;
            }
        }

        public double freqMax
        {
            get
            {
                if (frequencyRanges.Count > 0) return frequencyRanges.Max();
                else return 0;
            }
        }

        public double freqMean
        {
            get
            {
                if (frequencyRanges.Count > 0) return frequencyRanges.Mean();
                else return 0;
            }
        }

        public double freqSD
        {
            get
            {
                if (frequencyRanges.Count > 3) return (frequencyRanges.StandardDeviation());
                else return (freqMax-freqMin)/ 2;
            }
        }

        public int Count
        {
            get
            {
                return (intervals.Count);
            }
        }

        public int Add(double interval,double frequency)
        {
            intervals.Add(interval);
            frequencyRanges.Add(frequency);
            return intervals.Count;
        }

        public void Clear()
        {
            intervals = new List<double>();
            frequencyRanges = new List<double>();
        }

    }
}
