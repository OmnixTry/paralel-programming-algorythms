using LinkedList;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Queue.Tests
{
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Insert_AllElementsInserted()
		{
			// Arrange 
			const int quantity = 1000;
			CasQueue<int> queue = new CasQueue<int>();
			Task[] tasks = new Task[quantity];
			int[] results = new int[quantity];
			// Act 
			for (int i = 0; i < quantity; i++)
			{
				int tmp = i;
				tasks[i] = Task.Run(() =>
				{
					queue.Add(tmp + 1);
				});
			}
			Task.WaitAll(tasks);

			for (int i = 0; i < quantity; i++)
			{
				var res = 0;
				queue.Remove(out res);
				results[i] = res;
			}

			// Assert
			int result = 0;
			Assert.IsFalse(queue.Remove(out result));
			for (int i = 0; i < quantity; i++)
			{
				Assert.AreNotEqual(results[i], 0);
			}
			int realQuantity = results.Where(x => x <= 1000 && x > 0).Distinct().Count();
			Assert.AreEqual(quantity, realQuantity);
		}

		[Test]
		public void Insert_DeleteIsInvokedInParalell_nothingLeftInQueue()
		{
			// Arrange 
			const int quantity = 1000;
			CasQueue<int> queue = new CasQueue<int>();
			Task[] tasks = new Task[quantity];
			
			// Act 
			for (int i = 0; i < quantity; i++)
			{
				int tmp = i + 1;
				tasks[i] = Task.Run(() =>
				{
					queue.Add(tmp);
					int removeRes = 0;
					bool hasRemoved = queue.Remove(out removeRes);
					Assert.IsTrue(hasRemoved);
				});
			}
			Task.WaitAll(tasks);

			// Assert
			int result = 0;
			Assert.IsFalse(queue.Remove(out result));
		}

		[Test]
		public void Test3()
		{
			// Arrange 
			const int quantity = 1000;
			CasQueue<int> queue = new CasQueue<int>();

			// Act 
			for (int i = 0; i < quantity; i++)
			{
				queue.Add(i);
			}

			// Assert
			for (int i = 0; i < quantity; i++)
			{
				int outVal = 0;
				Assert.IsTrue(queue.Remove(out outVal));
				Assert.AreEqual(i, outVal);
			}
		}
	}
}