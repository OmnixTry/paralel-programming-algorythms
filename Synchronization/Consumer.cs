using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization
{
	public class Consumer
	{
        private Data Load;

		public Consumer(Data load)
		{
			Load = load;
		}

		// standard constructors

		public bool Run()
        {
            string receivedMessage = "";
            for (receivedMessage = Load.Receive(); "End" != receivedMessage;receivedMessage = Load.Receive())
            {
                Console.WriteLine(receivedMessage);

                // ...
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
			if (receivedMessage == "End")
			{
                return true;
			}
            return false;
        }
    }
}

