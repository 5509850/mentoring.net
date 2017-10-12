using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Module1_Multi_threading
{
    /// <summary>
    /// 1.	Write a program, which creates an array of 100 Tasks, 
    /// runs them and wait all of them are not finished. 
    /// Each Task should iterate from 1 to 1000 and print into the console the following string:
    /// “Task #0 – {iteration number}”.
    /// </summary>
    public class task1
    {
        public void Run()
        {
            var events = new List<ManualResetEvent>();
            Console.WriteLine("Now working task 1");
            DateTime start = DateTime.Now;            
            Task[] tasks = Enumerable.Range(1, 100).Select(i =>            
                Task.Factory.StartNew(() => {
                Callback(i);
            }
            )).ToArray();

            // Wait on all the tasks.
            Task.WaitAll(tasks);
            TimeSpan span = DateTime.Now - start;
            Console.WriteLine(string.Format("{0} ms",(int)span.TotalMilliseconds));
            Console.ReadKey();
        }

        void Callback(object numberTask)
        {
            for (int iteration = 1; iteration <= 1000; iteration++)
            {
                Console.WriteLine(string.Format("Task #{0} – {1}", (int)numberTask, iteration));
            }
            
        }       
    }
}
