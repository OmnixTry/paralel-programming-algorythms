using HarrisList;
using LinkedList;
using SkipList;
using Synchronization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab_1
{
	class Program
	{
		static async Task Main(string[] args)
		{
			/*
			var queue = new CasQueue<int>();
			queue.Add(1);
			queue.Add(2);
			queue.Add(3);
			queue.Add(4);
			queue.Add(5);
			queue.Add(6);
			int outT = 0;
			queue.Remove(out outT);
			Console.WriteLine(outT);
			queue.Remove(out outT);
			Console.WriteLine(outT);
			queue.Remove(out outT);
			Console.WriteLine(outT);
			queue.Remove(out outT);
			Console.WriteLine(outT);
			*/

			/*
						NonBlockingList<int> list = new NonBlockingList<int>();
						list.Insert(1);
						list.Insert(3);
						list.Insert(5);
						list.Insert(7);
						list.Insert(9);
						list.Insert(11);
						list.Insert(13);
						List<Task> tasks = new List<Task>();
						tasks.Add(Task.Run(() => { list.Delete(3); }));
						tasks.Add(Task.Run(() => { list.Delete(5); }));
						tasks.Add(Task.Run(() => { list.Insert(6); }));
						tasks.Add(Task.Run(() => { list.Delete(13); }));
						Task.WaitAll(tasks.ToArray());
						list.PrintTraverse();
						Console.WriteLine(list.Find(3));
			*/

			/*
			NumberIncrement.SeriesOfIncrements();
			*/

			/*
			Data data = new Data();
			Task sender = new Task(() => new Producer(data).Run());
			Task receiver = new Task(() => new Consumer(data).Run());

			sender.Start();
			receiver.Start();

			Task.WaitAll(sender, receiver);
			*/

			SkipList<int> list = new SkipList<int>();
			List<Node<int>> nodes = new List<Node<int>>
			{
				new Node<int>(5),
				new Node<int>(-10),
				new Node<int>(1),
				new Node<int>(10),
				new Node<int>(4),
				new Node<int>(14),
				new Node<int>(6),
				new Node<int>(3),
				new Node<int>(100),
			};
			foreach (var item in nodes)
			{
				list.Insert(item);
			}
			list.Delete(nodes[0]);
			list.Delete(nodes.Last());
			list.PrintListState();

			
		}
	}
}
