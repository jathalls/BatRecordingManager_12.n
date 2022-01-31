using System;
using UniversalToolkit;

namespace BatRecordingManager
{
    public class CallData
    {
        public CallData(double Start, double Fhi, double Fpk, double Flo, double Fk, double Fh, double dur, double TBC)
        {
            this.Start = Start;
            this.Fhi = Fhi;
            this.Fpk = Fpk;
            this.Flo = Flo;
            this.Fk = Fk;
            this.Fh = Fh;
            this.Dur = dur;
            this.TBC = TBC;
        }

        public override string? ToString()
        {
            string result = "";
            result += $"time={Start:####0.#}, ";
            result += $"start={Fhi:##0.0}, ";
            result += $"end={Flo:##0.0}, ";
            result += $"peak={Fpk:##0.0}, ";
            result += $"interval={TBC:###0.0}, ";
            result += $"duration={Dur:##0.00}, ";
            result += $"knee={Fk:##0.0}, ";
            result += $"heel={Fh:##0.0}";



            return (result);
        }

        /// <summary>
        /// the duration of the call in ms
        /// </summary>
        public double Dur { get; set; }

        /// <summary>
        /// The frequency of the Heel (shallow to steep inflection) if any
        /// </summary>
        public double Fh { get; set; }

        /// <summary>
        /// The start or hight frequency of the call
        /// </summary>
        public double Fhi { get; set; }

        /// <summary>
        /// The frequency of the Knee (steep to shallow inflection) if any
        /// </summary>
        public double Fk { get; set; }

        /// <summary>
        /// The end or low frequency of the call
        /// </summary>
        public double Flo { get; set; }

        /// <summary>
        /// The peak or max energy frequency of the call
        /// </summary>
        public double Fpk { get; set; }

        /// <summary>
        /// start location of the call in the segment in ms
        /// </summary>
        public double Start { get; set; }

        /// <summary>
        /// The interval between calls - actually the time from the start of this to the start of the next in ms
        /// </summary>
        public double TBC { get; set; }
    }

    public class callEventArgs : EventArgs
    {
        public callEventArgs(ReferenceCall call)
        {
            this.call = call;
        }

        public ReferenceCall call { get; set; } = new ReferenceCall();
    }
}
