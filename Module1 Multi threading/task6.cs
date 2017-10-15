using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Module1_Multi_threading
{
    /// <summary>
    /// 6.	Write a program which creates two threads and a shared collection: 
    /// the first one should add 10 elements into the collection and the second 
    /// should print all elements in the collection after each adding. 
    /// Use Thread, ThreadPool or Task classes for thread creation and 
    /// any kind of synchronization constructions.
    /// </summary>
    class task6
    {
        static Mutex sync = new Mutex();
        List<int> list = null;    
        bool printstarted = false;
        public void Run()
        {
            list = new List<int>();
            DateTime start = DateTime.Now;
            var taskWorkWithCollection = Task.Factory.StartNew(WorkWithCollection);
            var taskPrintNewElement = Task.Factory.StartNew(PrintNewElement);      
            Task.WaitAll(taskWorkWithCollection, taskPrintNewElement);
            TimeSpan span = DateTime.Now - start;
            Console.WriteLine(string.Format("Finish: {0} ms", (int)span.TotalMilliseconds));
            Console.ReadKey();
        }



        void WorkWithCollection()
        {
            while (!printstarted)
            {
                Task.Delay(1);
            }
            var rand = new Random();
            for (int i = 1; i <= 10; i++)
            {
                sync.WaitOne();
                list.Add(rand.Next());
                Console.WriteLine(string.Format("added {0} = ", i));
                sync.ReleaseMutex();
            }
            
        }

        void PrintNewElement()
        {
            printstarted = true;
            while (list.Count < 10)
            {
                if (list != null && list.Count != 0)
                {
                    sync.WaitOne();
                    Console.WriteLine(list[list.Count - 1]);                    
                    sync.ReleaseMutex();
                }
            }
            Console.WriteLine(Environment.NewLine);
        }
    }
}
