using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISDE_2
{
    public class BandwidthEvent : Event
    {
        public double BandwidthChange { get; set; }
        private static double[] BandwidthChanges = { 100, -200, 300, -100, -150, 100, 200, -250 };
        private static int BandwidthChangeIndex = 0;


        public static double NextBandwidthChange()
        {
            return BandwidthChanges[(BandwidthChangeIndex++) % BandwidthChanges.Length];
        }
    }
}
