using System;
using System.Threading.Tasks;

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
            bool success = false;
            //a)
            var parentTask = new Task<bool>(
                () =>             
                    {        
                                        
                        Console.WriteLine("working parent task");
                        return success;
                    });
            parentTask.ContinueWith((t) =>
            {                
                Console.WriteLine("working a) always task");
            });
            
            parentTask.ContinueWith((t) =>
            {                
                if (!parentTask.Result)
                {
                    Console.WriteLine("working b) without success task");
                }
                if (parentTask.IsFaulted)
                {
                    Console.WriteLine("working c) after Faulted task");
                }
                    
                if (parentTask.IsCanceled)
                {
                    Console.WriteLine("working d) after cancel task");
                }
            }
                
                );

            parentTask.Start();
            Console.ReadKey();
        }
    }
}
