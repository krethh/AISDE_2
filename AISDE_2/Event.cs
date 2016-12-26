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

        public int CompareTo(Event other)
        {
            if (Time - other.Time < 0)
                return -1;
            else return 1;
        }
    }
}
