using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISDE_2
{
    public class Event : IComparable<Event>
    {
        public double Time { get; set; }
        public double BandwidthChange { get; set; }
        private static double[] BandwidthChanges = { 100, -200, 300, -100, -150, 100, 200, -250 };
        private static int BandwidthChangeIndex = 0;

        public int CompareTo(Event other)
        {
            if (Time - other.Time < 0)
                return -1;
            else return 1;
        }

        public static double NextBandwidthChange()
        {
            return BandwidthChanges[(BandwidthChangeIndex++) % BandwidthChanges.Length];
        }
    }
}
