using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Module1_Multi_threading
{
    /// <summary>
    /// 5.	Write a program which recursively creates 10 threads. 
    /// Each thread should be with the same body and receive a state with integer number, 
    /// decrement it, print and pass as a state into the newly created thread. 
    /// Use ThreadPool class for this task and Semaphore for waiting threads.
    /// </summary>
    public class task5
    {
        int state = 20;
        static Semaphore sem = new Semaphore(1, 1);
        public void Run()
        {           
            Recursia(10);           
            Console.ReadKey();
        }

        int Recursia(int i)
        {
            if (i == 0)
            {
                return 0;
            }
            i -= 1;
            ThreadPool.QueueUserWorkItem(CountDown);            
            Recursia(i);
            return i;
        }

        void CountDown(object obj)
        {   
            sem.WaitOne();
            state -= 1;
            sem.Release();
            Console.WriteLine(state);
            
        }
    }
}
