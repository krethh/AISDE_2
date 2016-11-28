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

        public Player()
        {
            Events = new PriorityQueue<Event>();
            CurrentTime = 0;
            Bandwidth = 400; //domyślnie ustawiam pierwszą przepustowość łącza na 200
            BufferSize = 0;
            GenerateEvents(300);
            Console.WriteLine();
        }

        public void Simulate(double time)
        {
            Server server = new Server();
            while(CurrentTime < time)
            {
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
                double nextTime = RandomNumberFromExpDistribution(20);
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

    }
}
