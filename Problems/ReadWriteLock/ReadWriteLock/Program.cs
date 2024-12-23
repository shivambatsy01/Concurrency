using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadWriteLock
{
    internal class Program
    {
        static void Main(string[] args)
        {
        }
    }
}


/*
 * Problem statement and code modelling
 * we can have multiple read and write threads
 * -if a write thread is working: no other thread (read or write) should enter in critical section
 * -write thread can enter in critical section, if no other thread is inside
 * -multiple read threads can enter in critical section only when no write is inside.
 * Threads can enter inside, and leave by themselves
 * 
 * 
 * Critical section modelling:
 *  variable isWriting
 *  semaphores to define number of reads or directly can use number of reading thread
 *  
 *APIs:
 *  StartWrite()
 *  FinistWrite() or SaveAndExit()
 *  StartReading()
 *  FinistReading()
 *  
 *  it depends on process when they want to finish read or write.
 *  