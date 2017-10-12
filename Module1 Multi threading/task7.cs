using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Module1_Multi_threading
{
    /// <summary>   
    /// 
    ///  7.	Create a Task and attach continuations to it according to the following criteria:
    /// a.Continuation task should be executed regardless of the result of the parent task.
    /// b.Continuation task should be executed when the parent task finished without success.
    /// c.Continuation task should be executed when the parent task would be finished with fail and parent task thread should be reused for continuation
    /// d.	Continuation task should be executed outside of the thread pool when the parent task would be cancelled
    /// Demonstrate the work of the each case with console utility.*/
    /// </summary>
    class task7
    {        
        public void Run()
        {
            Console.Write("now working task 7");
            Console.ReadLine();
        }
    }
}
