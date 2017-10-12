using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            Console.Write("now working task1");
            Console.ReadLine();
        }
    }
}
