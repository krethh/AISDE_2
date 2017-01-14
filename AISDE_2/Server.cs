using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AISDE_2
{
    public class Server
    {
        public double VideoSize { get; set; } = 300;
        private double[] VideoSizes = { 300, 500, 600, 700, 1000 };

        public double RequestChunk(double length)
        {
            return length * VideoSize;
        }

        public void SetBiggerVideoSize()
        {
            if (VideoSize == VideoSizes[VideoSizes.Length - 1])
                return;

            int i = 0;
            for (; i < VideoSizes.Length; i++)
            {
                if (VideoSize == VideoSizes[i])
                    break;
            }
            VideoSize = VideoSizes[i + 1];
        }

        public void SetSmallerVideoSize()
        {
            if (VideoSize == VideoSizes[0])
                return;

            int i = VideoSizes.Length - 1;
            for (; i >= 0; i--)
            {
                if (VideoSize == VideoSizes[i])
                    break;
            }

            VideoSize = VideoSizes[i - 1];
        }
    }
}
