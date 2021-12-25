using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization
{
	public class TakingTurns
	{
        private DiyMutex  mutex = new DiyMutex ();
        public List<string> Results = new List<string>();
        public void Tick(bool running)
        {
            mutex.Lock();

            if (!running)
            {
                // Остановить часы
                mutex.Notify();
                mutex.Unlock();
                return;
            }

            Console.Write("тик ");
            Results.Add("Tick");
            mutex.Notify();
            mutex.Wait();
            mutex.Unlock();
        }

        public void Tock(bool running)
        {
            mutex.Lock();
            if (!running)
            {
                mutex.Notify();
                mutex.Unlock();
                return;
            }

            Console.WriteLine("так");
            Results.Add("Tock");
            mutex.Notify();
            mutex.Wait();

            mutex.Unlock();
        }

        public void NotifyAll()
        {
            mutex.Notify();
        }
    }

    public class Operator
    {
        public Task thrd;
        TakingTurns ttobj;
        string name;

        // Новый поток
        public Operator(string name, TakingTurns tt)
        {
            thrd = new Task(() => this.Run());
            ttobj = tt;
            this.name = name;
            thrd.Start();
        }

        void Run()
        {
            if (name == "Tick")
            {
                for (int i = 0; i < 5; i++)
                    ttobj.Tick(true);
                ttobj.Tick(false);
            }
            else
            {
                for (int i = 0; i < 5; i++)
                    ttobj.Tock(true);
                ttobj.Tock(false);
            }

        }
    }
}
