using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AISDE_2
{
    /// <summary>
    /// Symulator playera strumieniowego.
    /// </summary>
    public class Player
    {
        public PriorityQueue<Event> Events { get; set; }
        public double CurrentTime { get; set; }
        public double Bandwidth { get; set; } // ile możemy pobrać na sekundę
        public double VideoStreamSize { get; set; } // ile kilobajtów to jedna sekunda
        public double BufferSize { get; set; } // ile sekund video mamy już zbuforowane
        public const double CHUNK_LENGTH = 2;
        public List<double> YGraphValues { get; set; }
        public List<double> XGraphValues { get; set; }
        public EventHandler<LogEventArgs> LogCreated;

        public Player()
        {
            Events = new PriorityQueue<Event>();
            CurrentTime = 0;
            Bandwidth = 300; //domyślna przepustowość łącza
            BufferSize = 0;
            YGraphValues = new List<double>();
            XGraphValues = new List<double>();
            Console.WriteLine();
        }

        public void Simulate(double time)
        {
            CurrentTime = 0;
            Server server = new Server();

            Events.Enqueue(new BandwidthEvent { Time = RandomNumberFromExpDistribution(5), BandwidthChange = BandwidthEvent.NextBandwidthChange() });
            Events.Enqueue(new DownloadingFinishedEvent { Time = CHUNK_LENGTH * server.VideoSize / Bandwidth });

            while (CurrentTime < time)
            {
                Event nextEvent = Events.Dequeue();
                var bufferGone = nextEvent.Time - CurrentTime;

                if (nextEvent as BandwidthEvent != null)
                {
                    BandwidthEvent bandwidthEvent = (BandwidthEvent)nextEvent;
                    BufferSize -= bufferGone;
                    BufferSize = BufferSize < 0 ? 0 : BufferSize;

                    CurrentTime = bandwidthEvent.Time;
                    Bandwidth += bandwidthEvent.BandwidthChange;
                    YGraphValues.Add(BufferSize);

                    OnLogCreated(new LogEventArgs
                    {
                        Message = "Bandwidth change, time = " + Truncate(CurrentTime.ToString()) + ", buffer = " + Truncate(BufferSize.ToString()) + ", bandwidth = " + Bandwidth
                    });

                    Events.Enqueue(new BandwidthEvent
                    {
                        Time = CurrentTime + RandomNumberFromExpDistribution(10),
                        BandwidthChange = BandwidthEvent.NextBandwidthChange()
                    });
                }

                if (nextEvent as DownloadingFinishedEvent != null)
                {
                    DownloadingFinishedEvent downloadingEvent = (DownloadingFinishedEvent)nextEvent;
                    BufferSize -= bufferGone;
                    BufferSize = BufferSize < 0 ? 0 : BufferSize;

                    CurrentTime = downloadingEvent.Time;
                    BufferSize += CHUNK_LENGTH;
                    YGraphValues.Add(BufferSize);

                    OnLogCreated(new LogEventArgs
                    {
                        Message = "Downloaded, time = " + Truncate(CurrentTime.ToString()) + ", buffer = " + Truncate(BufferSize.ToString()) + ", bandwidth = " + Bandwidth
                    });

                    var surplus = (BufferSize - 30 > 0) ? BufferSize - 30 : 0;
                    CurrentTime += surplus;
                    BufferSize -= surplus;

                    OnLogCreated(new LogEventArgs
                    {
                        Message = "Request, time = " + Truncate(CurrentTime.ToString()) + ", buffer = " + Truncate(BufferSize.ToString()) + ", bandwidth = " + Bandwidth
                    });

                    Events.Enqueue(new DownloadingFinishedEvent
                    {
                        Time = CHUNK_LENGTH * server.VideoSize / Bandwidth + CurrentTime
                    });
                }
            }
        }

        private double RandomNumberFromExpDistribution(double mean)
        {
            Random random = new Random();
            Thread.Sleep(100);
            return (Math.Log(1 - random.NextDouble()) * (-mean) + 3); // +3 żeby nigdy nie było bardzo małych wartości
        }

        protected virtual void OnLogCreated(LogEventArgs e) => LogCreated?.Invoke(this, e);
    
        private string Truncate(string s)
        {
            if (s.Length > 5)
                return s.Substring(0, 5);
            else return s;
        }
    }
}
