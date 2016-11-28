using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISDE_2
{
    public class Server
    {
        public double VideoSize { get; set; }

        public Server()
        {
            VideoSize = 300; //domyślnie wideo 300 kb/s
        }

        public double RequestChunk(double length)
        {
            return length * VideoSize;
        }
    }
}
