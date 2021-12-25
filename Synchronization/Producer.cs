using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization
{
	public class Producer
	{
        private Data Data;

        // standard constructors
        public Producer(Data data)
		{
            Data = data;
		}
        public void Run()
        {
            string[] packets =
            {
                "First packet",
                "Second packet",
                "Third packet",
                "Fourth packet",
                "End"
            };

            foreach (string packet in packets)
            {
                Data.Send(packet);

                // Thread.sleep() to mimic heavy server-side processing
                try
                {
                    var rand = new Random();
                    Thread.Sleep(rand.Next(1000, 5000));
                }
                catch (InvalidOperationException e)
                {
                    Thread.CurrentThread.Interrupt();
                    Console.WriteLine("Thread interrupted", e);
                }
            }
        }
    }
}
