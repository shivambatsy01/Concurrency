using System;
using System.Threading;

public class BoundedBufferMutex
{
	int maxSize;
	int currSize;
	int[] items;
	int head, tail; //enqueue at tail, dequeue from head
					//CurrSize, items, head and tail should comes in critical section
	Mutex padlock;


	public BoundedBufferMutex(int size)
	{
		maxSize = size;
		currSize = 0;
		items = new int[maxSize];
		head = 0; tail = 0;
		padlock = new Mutex(false); //false as not owned, True if want to owned by Current thread
	}

	public void Produce(int item)
	{
		//Acquire padlock before accessing any common variable (in other words Critical section)
		padlock.WaitOne();

		while (currSize == maxSize) //currsize should be accessed under critical section only
		{
			padlock.ReleaseMutex();
			//again try to acquire
			padlock.WaitOne();
		}

		currSize++;
		if (tail == maxSize)
		{
			tail = 0;
		}
		items[tail] = item;

		padlock.ReleaseMutex();
	}


	public int Consume()
	{
		padlock.WaitOne();

		while (currSize == 0) //currsize should be accessed under critical section only
		{
			padlock.ReleaseMutex();
			//again try to aquire before entering into critical section
			padlock.WaitOne();
		}

		currSize--;
		if (head == maxSize)
		{
			head = 0;
		}
		int value = items[head];
		head++;

		padlock.ReleaseMutex();

		return value;
	}

	/*
	 * Problem with this Mutex solution: Busy wait solution
	 * In While loop, after releasing the mutex, same thread is trying to re-acquire it just to check if the while condition is breaking.
	 * Problems with this:
	 *	1. Same loop can reaquire again immediatly after releasing the Mutex, It will be kind of starvation for another threads
	 *	2. If same loop is acquiring Mutex again, no change in state and hence unnecessary CPU cycles will be exhausted.
	 *	
	 *	Solution: instead of Giving full responsibility to thread i.e. to re-acuire Mutex and check condition,
	 *	Implement some sort of mechanism so that thread will go in wait queue and comes back only when there are chances to satisfy condition.
	 *	This can be done by signalling mechanisms which are provided by Monitors
	 *	

		One more point to note: try releasing any lock in finally block, because
		Suppose a thread acuiring lock/mutex got aborted, process will go into deadlock
	 */
}
