using System;
using System.Threading;
using System.Threading.Tasks;

namespace Synchronization
{
	public class DiyMutex
	{
		#region doNotTucc

		private int _locked;
		private Thread _lockedThread;
		private int _notify;
		private int _notifyAll;
		private int _waitingThreads;
		#endregion

		private bool Locked
		{
			get
			{
				return _locked == 1;
			}
			set
			{
				Interlocked.Exchange(ref _locked, value ? 1 : 0);
			}
		}
		private Thread LockedThread
		{
			get
			{
				return _lockedThread;
			}
			set
			{
				Interlocked.Exchange(ref _lockedThread, value);
			}
		}
		private bool ShouldNotify
		{
			get
			{
				return _notify == 1;
			}
			set
			{
				Interlocked.Exchange(ref _notify, value ? 1 : 0);
			}
		}
		private bool ShouldNotifyAll
		{
			get
			{
				return _notifyAll == 1;
			}
			set
			{
				Interlocked.Exchange(ref _notifyAll, value ? 1 : 0);
			}
		}
		private int WaitingThreads
		{
			get
			{
				return _waitingThreads;
			}
			set
			{
				Interlocked.Exchange(ref _waitingThreads, value);
			}
		}

		public void FakeLock()
		{
			int currentLockedValue = _locked;
			int value = 0;
			while(!CAS(ref _locked, currentLockedValue, value))
			{
				Thread.Yield();
				currentLockedValue = _locked;
			}
			LockedThread = Thread.CurrentThread;
		}

		public void Lock()
		{
			if (IsCurrentThread())
				return;
			int expected = 0;
			int value = 1;

			while (!CAS(ref _locked, expected, value))
			{
				Thread.Yield();
			}
			LockedThread = Thread.CurrentThread;
		}

		public void Unlock()
		{
			if (!IsCurrentThread())
			{
				throw new InvalidOperationException("Can't release lock before acquiring it.");
			}
			LockedThread = null;
			Locked = false; // property sets safely
		}

		public void Wait()
		{
			if (!IsCurrentThread())
			{
				throw new InvalidOperationException("Acquire the lock first.");
			}

			Unlock();
			MarkAsWaiting();
			while (!ShouldNotify && !ShouldNotifyAll)
			{
				Thread.Yield();
			}
			UnMarkAsWaiting();
			Lock();
			ShouldNotify = false;
		}

		public void Notify()
		{
			if (!IsCurrentThread())
			{
				throw new InvalidOperationException("Acquire the lock first.");
			}
			ShouldNotify = true;
		}

		public void NotifyAll()
		{
			if (!IsCurrentThread())
			{
				throw new InvalidOperationException("Acquire the lock first.");
			}

			ShouldNotifyAll = true;
			while (WaitingThreads != 0)
			{
				Thread.Yield();
			}
			ShouldNotifyAll = false;

		}

		private void MarkAsWaiting()
		{
			Interlocked.Increment(ref _waitingThreads);
		}

		private void UnMarkAsWaiting()
		{
			Interlocked.Decrement(ref _waitingThreads);
		}

		private bool IsCurrentThread()
		{
			return LockedThread == Thread.CurrentThread;
		}


		private bool CAS(ref int destination, int compared, int exchange)
		{
			return compared == Interlocked.CompareExchange(ref destination, exchange, compared);
		}
	}
}
