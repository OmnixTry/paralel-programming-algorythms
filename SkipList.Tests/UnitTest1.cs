using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SkipList.Tests
{
	public class Tests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Insert_OrderIsCorrect()
		{
			// Arrange 
			const int quantity = 1000;
			Task[] tasks = new Task[quantity];
			Random random = new Random();
			SkipList<int> skipList = new SkipList<int>();

			// Act 
			for (int i = 0; i < quantity; i++)
			{
				tasks[i] = Task.Run(() => 
				{
					int value = random.Next();
					skipList.Insert(new Node<int>(value, value));
				});
			}
			Task.WaitAll(tasks);
			var result = skipList.PrintListState();

			// Assert
			for (int i = 0; i < result.Count - 1; i++)
			{
				Assert.IsTrue(result[i] <= result[i + 1]);
			}
		}

		[Test]
		public void Insert_MeanwhileSomeValuesGetDeleted()
		{
			// Arrange 
			const int quantity = 1000;
			Task[] tasks = new Task[quantity];
			Random random = new Random();
			SkipList<int> skipList = new SkipList<int>();

			// Act 
			for (int i = 0; i < quantity; i++)
			{
				int tmp = i;
				tasks[i] = Task.Run(() =>
				{
					int value = random.Next();
					var node = new Node<int>(tmp, tmp);
					skipList.Insert(node);
					if (tmp % 2 == 0)
					{
						Thread.Sleep(10);
						skipList.Delete(node);
					}
				});
			}
			Task.WaitAll(tasks);
			var result = skipList.PrintListState();

			// Assert
			Assert.AreEqual(500, result.Count);
			for (int i = 0; i < 500; i++)
			{
				Assert.AreEqual(result[i] % 2, 1);
			}
		}

		[Test]
		public void Delete_NonExistingValue_ReturnFalse()
		{
			// Arrange 
			SkipList<int> skipList = new SkipList<int>();
			skipList.Insert(new Node<int>(1));
			skipList.Insert(new Node<int>(2));
			skipList.Insert(new Node<int>(3));
			skipList.Insert(new Node<int>(4));
			skipList.Insert(new Node<int>(5));
			skipList.Insert(new Node<int>(6));
			skipList.Insert(new Node<int>(7));

			// Act 
			bool result = skipList.Delete(new Node<int>(8));
			
			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public void Insert_ExistingValue_ReturnsTrue()
		{
			// Arrange 
			SkipList<int> skipList = new SkipList<int>();
			skipList.Insert(new Node<int>(1));
			skipList.Insert(new Node<int>(2));
			skipList.Insert(new Node<int>(3));
			skipList.Insert(new Node<int>(4));
			skipList.Insert(new Node<int>(5));
			skipList.Insert(new Node<int>(6));
			skipList.Insert(new Node<int>(7));

			// Act 
			bool result = skipList.Insert(new Node<int>(5));
			
			// Assert
			Assert.IsFalse(result);

		}

		[Test]
		public void Find_NonExistingValue_ResurnsFalse()
		{
			// Arrange 
			SkipList<int> skipList = new SkipList<int>();
			skipList.Insert(new Node<int>(1));
			skipList.Insert(new Node<int>(2));
			skipList.Insert(new Node<int>(3));
			skipList.Insert(new Node<int>(5));
			skipList.Insert(new Node<int>(6));
			skipList.Insert(new Node<int>(7));
			var preds = new Node<int>[SkipList<int>.MaxLevel + 1];
			var succs = new Node<int>[SkipList<int>.MaxLevel + 1];

			// Act 
			bool result = skipList.Find(new Node<int>(4), ref preds, ref succs);

			// Assert
			Assert.IsFalse(result);
		}

		[Test]
		public void Test6()
		{
			// Arrange 

			// Act 

			// Assert

		}
	}
}