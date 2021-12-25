using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization
{
	public class Data
	{
        public DiyMutex DiyMutex { get; set; } = new DiyMutex();
        private String packet;

        // True if receiver should wait
        // False if sender should wait
        private bool transfer = true;

        public void Send(String packet)
        {
            DiyMutex.Lock();
            while (!transfer)
            {
                try
                {
                    DiyMutex.Wait();
                }
                catch (InvalidOperationException e)
                {
                    Thread.CurrentThread.Interrupt();
                    Console.WriteLine("Thread interrupted", e);
                }
            }
            transfer = false;

            this.packet = packet;
            DiyMutex.NotifyAll();
            DiyMutex.Unlock();
        }

        public string Receive()
        {
            DiyMutex.Lock();
            while (transfer)
            {
                try
                {
                    DiyMutex.Wait();
                }
                catch (InvalidOperationException e)
                {
                    Thread.CurrentThread.Interrupt();
                    Console.WriteLine("Thread interrupted", e);
                }
            }
            transfer = true;

            DiyMutex.NotifyAll();
            DiyMutex.Unlock();
            return packet;
        }
    }
}
