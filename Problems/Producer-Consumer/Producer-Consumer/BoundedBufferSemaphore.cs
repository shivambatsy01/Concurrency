using System;
using System.Threading;

public class BoundedBufferSemaphore
{
    int maxSize;
    int[] items;
    int head, tail; //enqueue at tail, dequeue from head
    Semaphore producerSemaphore, consumerSemaphore;
    Semaphore lockingSemaphore; //this is used to guard critical section, at a time we want only one process (producer OR consumer) to enter into critical section


    public BoundedBufferSemaphore(int size)
	{
        maxSize = size;
        producerSemaphore = new Semaphore(size, size); //all available
        consumerSemaphore = new Semaphore(size, 0); //not available
        lockingSemaphore = new Semaphore(1, 1); //binary semaphore will work as mutex if ownership handled properly
        head = 0;
        tail = 0;
        items = new int[size];
    }

    public void Produce(int item)
    {
        lockingSemaphore.WaitOne();
        producerSemaphore.WaitOne();

        if (tail == maxSize)
        {
            tail = 0;
        }
        items[tail] = item;

        consumerSemaphore.Release(); //no ownership concept for semaphore
        lockingSemaphore.Release();
    }


    public int Consume()
    {
        lockingSemaphore.WaitOne();
        consumerSemaphore.WaitOne();

        if (head == maxSize)
        {
            head = 0;
        }
        int value = items[head];
        head++;

        producerSemaphore.Release();
        lockingSemaphore.Release();

        return value;
    }
}



/*
 * How to model this problem using semaphores?
 * Here we dont need queue curr size to be maintained, but semaphores will do that for us
 * Need two semaphores, one for producer and one for consumer
 * 
 * Once producer pushes item in queue, it Increase availability count of consumer semaphore
 * Producer can only try to push item in queue when there is a availability for producer semaphore
 * 
 * Once consumer consumes and item, it increases availability count for producer semaphore
 * Consumer can try consuming item only when there is availability for consumer semaphore
 * 
 * 
 * initially all producer semaphores are available and consumers are not
 */