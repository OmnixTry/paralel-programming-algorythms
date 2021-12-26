using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace HarrisList.Tests
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
			NonBlockingList<int> list = new NonBlockingList<int>();

			// Act 
			for (int i = 0; i < quantity; i++)
			{
				tasks[i] = Task.Run(() =>
				{
					int value = random.Next();
					list.Insert(value);
				});
			}
			Task.WaitAll(tasks);
			var result = list.PrintTraverse();

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
			NonBlockingList<int> skipList = new NonBlockingList<int>();

			// Act 
			for (int i = 0; i < quantity; i++)
			{
				int tmp = i;
				tasks[i] = Task.Run(() =>
				{
					int value = random.Next();
					skipList.Insert(tmp);
					if (tmp % 2 == 0)
					{
						Thread.Sleep(10);
						skipList.Delete(tmp);
					}
				});
			}
			Task.WaitAll(tasks);
			var result = skipList.PrintTraverse();

			// Assert
			for (int i = 0; i < result.Count; i++)
			{
				Assert.AreEqual(result[i] % 2, 1);
			}
		}

		
		[Test]
		public void Delete_NonExistingValue_ReturnFalse()
		{
			// Arrange 
			const int quantity = 1000;
			Task[] tasks = new Task[quantity];
			Random random = new Random();
			NonBlockingList<int> skipList = new NonBlockingList<int>();

			// Act 
			for (int i = 0; i < quantity; i++)
			{
				int tmp = i;
				tasks[i] = Task.Run(() =>
				{
					int value = random.Next();
					skipList.Insert(tmp);
				});
			}
			Task.WaitAll(tasks);
			var result = skipList.Find(quantity + 1);

			// Assert
			Assert.IsFalse(result);
			
		}

		[Test]
		public void Delete_ExistingValue_ReturnTrue()
		{
			// Arrange 
			const int quantity = 1000;
			Task[] tasks = new Task[quantity];
			Random random = new Random();
			NonBlockingList<int> skipList = new NonBlockingList<int>();

			// Act 
			for (int i = 0; i < quantity; i++)
			{
				int tmp = i;
				tasks[i] = Task.Run(() =>
				{
					int value = random.Next();
					skipList.Insert(tmp);
				});
			}
			Task.WaitAll(tasks);
			var result = skipList.Find(quantity - 1);

			// Assert
			Assert.IsTrue(result);

		}

		
		[Test]
		public void Insert_ExistingValue_ReturnsFalse()
		{
			// Arrange 
			const int quantity = 1000;
			Task[] tasks = new Task[quantity];
			Random random = new Random();
			NonBlockingList<int> skipList = new NonBlockingList<int>();

			// Act 
			for (int i = 0; i < quantity; i++)
			{
				int tmp = i;
				tasks[i] = Task.Run(() =>
				{
					int value = random.Next();
					skipList.Insert(tmp);
				});
			}
			Task.WaitAll(tasks);
			bool result = skipList.Insert(5);

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