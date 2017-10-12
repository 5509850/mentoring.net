using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public void Run()
        {
            Console.Write("now working task 5");
            Console.ReadLine();
        }
    }
}
