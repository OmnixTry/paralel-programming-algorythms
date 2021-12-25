using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synchronization
{
	public class NumberIncrement
	{
        public int count = 0;
        private DiyMutex mutex = new DiyMutex();
        public void Increment()
        {
            mutex.Lock();
            count = count + 1;
            mutex.Unlock();
        }

        public static void SeriesOfIncrements()
		{
            const int taskNum = 1000;

            NumberIncrement inccrementer = new NumberIncrement();
            Task[] tasks = new Task[taskNum];
            for (int i = 0; i < taskNum; i++)
            {
                tasks[i] = Task.Run(() => inccrementer.Increment());
            }
            Task.WaitAll(tasks);
            
            Console.WriteLine(inccrementer.count);
        }
    }
}
