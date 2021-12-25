using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization.Tests
{
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Notify_WhenNotAcquiredLock_ThrowsException()
		{
			var mutex = new DiyMutex();
			
			Assert.Throws<InvalidOperationException>(() => mutex.Notify(), "Acquire the lock first.");
		}

		[Test]
		public void NotifyAll_WhenNotAcquiredLock_ThrowsException()
		{
			var mutex = new DiyMutex();

			Assert.Throws<InvalidOperationException>(() => mutex.NotifyAll(), "Acquire the lock first.");
		}

		[Test]
		public void Wait_WhenNotAcquiredLock_ThrowsException()
		{
			var mutex = new DiyMutex();

			Assert.Throws<InvalidOperationException>(() => mutex.Wait(), "Acquire the lock first.");
		}

		[Test]
		public void Unlock_WhenNotAcquiredLock_ThrowsException()
		{
			var mutex = new DiyMutex();

			Assert.Throws<InvalidOperationException>(() => mutex.Unlock(), "Acquire the lock first.");
		}

		[Test]
		public void NotifyAll_MultipleThreadsWait_AllWakeUp()
		{
			// Arrange
			const int expectedWakeup = 10;
			int wakeUp = 0;
			Task[] tasks = new Task[expectedWakeup + 1];
			var mutex = new DiyMutex();
			// Act
			for (int i = 0; i < expectedWakeup; i++)
			{
				tasks[i] = Task.Run(() => run());
			}
			Thread.Sleep(60000);
			tasks[expectedWakeup] = Task.Run(() => 
			{
				mutex.Lock();
				mutex.NotifyAll();
				mutex.Unlock();
			});
			Task.WaitAll(tasks);
			// Assert
			Assert.AreEqual(expectedWakeup, wakeUp);


			void run() {
				mutex.Lock();
				mutex.Wait();
				Interlocked.Increment(ref wakeUp);
				mutex.Unlock();
			}
		}

		[Test]
		public void Lock_MultipleThreadsCallLock_OtherThreadsAcquireAfterRelease()
		{
			// Arrange
			bool firstDone = false;
			bool firstValueInSecond = false;
			var mutex = new DiyMutex();
			// Act
			Task.Run(() =>
			{
				mutex.Lock();
				Thread.Sleep(2000);
				firstDone = true;
				mutex.Unlock();
			});
			Thread.Sleep(1000);
			Task.Run(() =>
			{
				mutex.Lock();
				firstValueInSecond = firstDone;
				mutex.Unlock();
			}).Wait();
			
			// Assert
			Assert.IsTrue(firstValueInSecond);
		}

		[Test]
		public void ParalellIncrements()
		{
			// Arrange
			const int numberOfIncrements = 1000;
			const int originalCount = 0;
			DiyMutex mutex = new DiyMutex();
			NumberIncrement incrementor = new NumberIncrement(originalCount, mutex);

			// Act 
			incrementor.SeriesOfIncrements(numberOfIncrements);

			// Assert
			Assert.AreEqual(numberOfIncrements, incrementor.Count);
		}

		[Test]
		public void ProducerConsumer()
		{
			// Arrange
			Data data = new Data();
			Task sender = new Task(() => new Producer(data).Run());
			Task<bool> receiver = new Task<bool>(() => new Consumer(data).Run());

			// Act 
			sender.Start();
			receiver.Start();
			Task.WaitAll(sender, receiver);

			// Assert
			Assert.IsTrue(receiver.Result);
		}

		[Test]
		public void RuningInTuns()
		{
			// Arrange
			TakingTurns tt = new TakingTurns();
			Operator mt1 = new Operator("Tick", tt);
			Operator mt2 = new Operator("Tock", tt);

			// Act 
			Task.WaitAll(mt1.thrd, mt2.thrd);

			// Assert
			Assert.AreEqual(tt.Results.Count, 10);
			Assert.AreNotEqual(tt.Results[0], tt.Results[1]);
			string[] results = { tt.Results[0], tt.Results[1] };
			for (int i = 0; i < tt.Results.Count(); i++)
			{
				Assert.AreEqual(tt.Results[i], results[i % 2]);
			}
		}
	}
}