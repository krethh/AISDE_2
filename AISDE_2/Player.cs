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
        public List<double> XGraphValues { get; set; }
        public List<double> YGraphValues { get; set; }
        public EventHandler<LogEventArgs> LogCreated;

        public Player()
        {
            Events = new PriorityQueue<Event>();
            CurrentTime = 0;
            Bandwidth = 300; //domyślnie ustawiam pierwszą przepustowość łącza na 200
            BufferSize = 0;
            YGraphValues = new List<double>();
            Console.WriteLine();
        }

        public void Simulate(double time)
        {
            GenerateEvents(time);
            CurrentTime = 0;
            Server server = new Server();

            var usedBandwidth = Bandwidth;
            while (Events.Count != 0)
            {
                Event nextEvent = Events.Dequeue();
                while ((nextEvent.Time - CurrentTime) % CHUNK_LENGTH > 0) // tak długo jak możemy pobierać pełne chunki długości 2 sek.
                {
                    /// po to, żeby rozmiar bufora nie rosnął powyżej 30
                    if (BufferSize > 30 && Bandwidth > 300) 
                        usedBandwidth = server.VideoSize;
                    else usedBandwidth = Bandwidth;

                    BufferSize += (usedBandwidth * CHUNK_LENGTH - server.RequestChunk(CHUNK_LENGTH)) / server.VideoSize;
                    OnLogCreated(new LogEventArgs
                    {
                        Message = "Time: " + CurrentTime + ">> Chunk downloaded, length: " + CHUNK_LENGTH + ", bandwidth: " + Bandwidth + ", used bandwidth: " + usedBandwidth + ", size: " + server.VideoSize * CHUNK_LENGTH
                    + ", buffer size: " + Truncate(BufferSize.ToString()) + "sec."
                    });

                    YGraphValues.Add(BufferSize);
                    CurrentTime += CHUNK_LENGTH;
                }
                /// pobieranie chunka w momencie zmiany warunków na łączu modelujemy przez pobieranie go z przepustowością równą
                /// średniej z przepustowości po i przed zmianą
                var nextBandwidth = Bandwidth + nextEvent.BandwidthChange;
                var averageBandwidth = (Bandwidth + nextBandwidth) / 2;

                BufferSize += (averageBandwidth * CHUNK_LENGTH - server.RequestChunk(CHUNK_LENGTH)) / server.VideoSize;
                OnLogCreated(new LogEventArgs
                {
                    Message = "Time: " + CurrentTime + ">> Chunk downloaded, length: " + CHUNK_LENGTH + ", bandwidth: " + averageBandwidth + ", used bandwidth: " + averageBandwidth + ", size: " + server.VideoSize * CHUNK_LENGTH
                    + ", buffer size: " + Truncate(BufferSize.ToString()) + "sec."
                });

                Bandwidth += nextEvent.BandwidthChange;
            }
        }

        /// <summary>
        /// Generuje zdarzenia do kolejki zdarzeń.
        /// </summary>
        /// <param name="time">Czas trwania wszystkich zdarzeń.</param>
        public void GenerateEvents(double time)
        {
            double timeToElapse = 0;
            while(timeToElapse < time)
            {
                double nextTime = RandomNumberFromExpDistribution(5);
                Events.Enqueue(new Event { Time = timeToElapse + nextTime , BandwidthChange = Event.NextBandwidthChange() });
                timeToElapse += nextTime;
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
