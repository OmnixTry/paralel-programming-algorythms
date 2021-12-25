using NUnit.Framework;
using System.Linq;
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