using System;
using System.Threading;

namespace Module1_Multi_threading
{
    /// <summary>
    /// 4.	Write a program which recursively creates 10 threads. 
    /// Each thread should be with the same body and receive a state with integer number, 
    /// decrement it, print and pass as a state into the newly created thread.
    /// Use Thread class for this task and Join for waiting threads.
    /// </summary>
    public class task4
    {
        int state = 20;    
        public void Run()
        {
            DateTime start = DateTime.Now;           
            Recursia(10);
            TimeSpan span = DateTime.Now - start;
            Console.WriteLine(string.Format("Finish: {0} ms", (int)span.TotalMilliseconds));
            Console.ReadKey();
        }

        int Recursia(int i)
        {
            if (i == 0)
            {
                return 0;
            }
            i -= 1;
            var thread = new Thread(CountDown);
            thread.Start(state);
            thread.Join();
            Recursia(i);
            return i;
        }
         
        void CountDown(object s)
        {
            var count = (int)s;
            state = count - 1;
            Console.WriteLine(state);
        }
    }
}
