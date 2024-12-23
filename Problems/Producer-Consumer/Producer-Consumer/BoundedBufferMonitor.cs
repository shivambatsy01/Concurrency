using System;
using System.Threading;

public class BoundedBufferMonitor
{
    int maxSize;
    int currSize;
    int[] items;
    int head, tail; //enqueue at tail, dequeue from head
    //CurrSize, items, head and tail should comes in critical section
    Object MonitorPadlock;

    public BoundedBufferMonitor(int size)
	{
        maxSize = size;
        currSize = 0;
        items = new int[maxSize];
        head = 0;
        tail = 0;
        MonitorPadlock = new Object();
    }

    public void Produce(int item)
    {
        //Acquire padlock before accessing any common variable (in other words Critical section)
        Monitor.Enter(MonitorPadlock);

        while (currSize == maxSize) //currsize should be accessed under critical section only
        {
            Monitor.Wait(MonitorPadlock);
            //Now this thread goes to wait queue after monitor wait statement and will be back only on receiving signal
            //when thread will wake, it will reacuire lock by itself (So easy implementation)
        }

        currSize++;
        if (tail == maxSize)
        {
            tail = 0;
        }
        items[tail] = item;

        Monitor.PulseAll(MonitorPadlock); //pulse all wait queue thread waiting for padlock and OS will see itself which one should be allowed
        Monitor.Exit(MonitorPadlock);
    }


    public int Consume()
    {
        //Acquire padlock before accessing any common variable (in other words Critical section)
        Monitor.Enter(MonitorPadlock);

        while (currSize == 0) //currsize should be accessed under critical section only
        {
            Monitor.Wait(MonitorPadlock);
            //Now this thread goes to wait queue after monitor wait statement and will be back only on receiving signal
            //when thread will wake, it will reacuire lock by itself (So easy implementation)
        }

        currSize--;
        if (head == maxSize)
        {
            head = 0;
        }
        int value = items[head];
        head++;

        Monitor.PulseAll(MonitorPadlock); //pulse all wait queue thread waiting for padlock and OS will see itself which one should be allowed
        Monitor.Exit(MonitorPadlock);

        return value;
    }


    /*
     * Why we used while loop here as we already have monitor.wait() and on waking up of thread, it will reacuire the lock
     * Thread will be waked only when the condition will be satisfied.
     * 
     * 
     * Two reasons:
     * 1. Spurious thread wake: lets suppose thread got woke due to some internal CPU reason, thus before proceeding we want to make sure if condition is satisfied
     * 2. If multiple producer and consumer threads are there, Another producer can try to wake another producer thread and same for consumers/
     * 
     * 
     * One more point to note: try releasing any lock in finally block, because
		Suppose a thread acuiring lock/mutex got aborted, process will go into deadlock
        Else use Lock statement (which is nothing but wrapper over monitor and takes care of all these edge cases)
     */
}
