using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synchronization
{
	public class NumberIncrement
	{
        public int Count = 0;
        private DiyMutex Mutex;

		public NumberIncrement(int count, DiyMutex mutex)
		{
			this.Count = count;
			this.Mutex = mutex;
		}

		public void Increment()
        {
            Mutex.Lock();
            Count = Count + 1;
            Mutex.Unlock();
        }

        public void SeriesOfIncrements(int taskNum)
		{
            Task[] tasks = new Task[taskNum];
            for (int i = 0; i < taskNum; i++)
            {
                tasks[i] = Task.Run(() => Increment());
            }
            Task.WaitAll(tasks);
            
            Console.WriteLine(Count);
        }
    }
}
