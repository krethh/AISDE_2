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
        public double MeasurementSamplingRate { get; set; } = 0.05; // co ile zbieramy próbki z bufora
        public const double CHUNK_LENGTH = 2;
        public List<double> YGraphValues { get; set; }
        public List<double> BandwidthValues { get; set; } = new List<double>();
        public List<double> SegmentSizeValues { get; set; } = new List<double>();
        public EventHandler<LogEventArgs> LogCreated;

        public Player()
        {
            Events = new PriorityQueue<Event>();
            CurrentTime = 0;
            Bandwidth = 450; //domyślna przepustowość łącza
            BufferSize = 0;
            YGraphValues = new List<double>();
            Console.WriteLine();
        }

        public void Simulate(double time)
        {
            CurrentTime = 0;
            Server server = new Server();

            Events.Enqueue(new BandwidthEvent { Time = RandomNumberFromExpDistribution(5), BandwidthChange = BandwidthEvent.NextBandwidthChange() });
            Events.Enqueue(new DownloadingFinishedEvent { Time = CHUNK_LENGTH * server.VideoSize / Bandwidth });
            Events.Enqueue(new MeasurementEvent { Time = 0 });

            int CyclesWithTooMuch = 0, CyclesWithTooLittle = 0;

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

                    OnLogCreated(new LogEventArgs
                    {
                        Message = "Bandwidth change, time = " + Truncate(CurrentTime.ToString()) + ", buffer = " + Truncate(BufferSize.ToString()) +
                        ", bandwidth = " + Bandwidth + ", size = " + server.VideoSize
                    });

                    Events.Enqueue(new BandwidthEvent
                    {
                        Time = CurrentTime + RandomNumberFromExpDistribution(10),
                        BandwidthChange = BandwidthEvent.NextBandwidthChange()
                    });
                }

                else if (nextEvent as DownloadingFinishedEvent != null)
                {
                    DownloadingFinishedEvent downloadingEvent = (DownloadingFinishedEvent)nextEvent;
                    BufferSize -= bufferGone;

                    if (BufferSize < 10)
                    {
                        CyclesWithTooLittle++;
                    }
                    else
                    {
                        CyclesWithTooLittle = 0;
                    }

                    if(CyclesWithTooLittle > 2)
                    {
                        server.SetSmallerVideoSize();
                        CyclesWithTooLittle = 0;
                    }
                    BufferSize = BufferSize < 0 ? 0 : BufferSize;

                    CurrentTime = downloadingEvent.Time;
                    BufferSize += CHUNK_LENGTH;

                    OnLogCreated(new LogEventArgs
                    {
                        Message = "Downloaded, time = " + Truncate(CurrentTime.ToString()) + ", buffer = " + Truncate(BufferSize.ToString()) +
                        ", bandwidth = " + Bandwidth + ", size = " + server.VideoSize
                    });

                    /// jeżeli nadwyżka bufora jest duża, to zliczaj, jak długo jest taki stan. Jeżeli więcej niż dwa chunki, to zwiększ jakość obrazu
                    var surplus = (BufferSize - 30 > 0) ? BufferSize - 30 : 0;
                    if (surplus > 0)
                        CyclesWithTooMuch++;
                    else
                        CyclesWithTooMuch = 0;

                    if (CyclesWithTooMuch > 2)
                    {
                        server.SetBiggerVideoSize();
                        CyclesWithTooMuch = 0;
                    }

                    OnLogCreated(new LogEventArgs
                    {
                        Message = "Request, time = " + Truncate(CurrentTime.ToString()) + ", buffer = " + Truncate(BufferSize.ToString()) +
                        ", bandwidth = " + Bandwidth + ", size = " + server.VideoSize
                    });

                    Events.Enqueue(new DownloadingFinishedEvent
                    {
                        Time = CHUNK_LENGTH * server.VideoSize / Bandwidth + CurrentTime + surplus
                    });
                }

                else if (nextEvent as MeasurementEvent != null)
                {
                    MeasurementEvent measurementEvent = (MeasurementEvent)nextEvent;
                    BufferSize -= bufferGone;
                    BufferSize = BufferSize < 0 ? 0 : BufferSize;

                    CurrentTime = measurementEvent.Time;
                    YGraphValues.Add(BufferSize);
                    BandwidthValues.Add(Bandwidth);
                    SegmentSizeValues.Add(server.VideoSize);
                    
                    Events.Enqueue(new MeasurementEvent
                    {
                        Time = CurrentTime + MeasurementSamplingRate
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
