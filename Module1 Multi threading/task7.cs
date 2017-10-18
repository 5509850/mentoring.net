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
    public class Task7
    { 
        public void Run()
        {            
            var parentTask = new Task(
                () =>
                {
                    ParentTask();         
                });
         
            parentTask.ContinueWith(t =>
            {
                Console.WriteLine("Working a) always task");
            })
            .ContinueWith(t => Console.WriteLine("working b) without success task"), TaskContinuationOptions.OnlyOnFaulted)
            .ContinueWith(t => Console.WriteLine("working d) task would be cancelled"), TaskContinuationOptions.OnlyOnCanceled)
            .ContinueWith(t => Console.WriteLine("working c) parent task would be finished with fail and parent task thread should be reused for continuation"), 
                        TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.RunContinuationsAsynchronously);
            parentTask.Start();
            Console.ReadKey();
        }

        private void ParentTask()
        {
            Console.WriteLine("Working parent task");            
        }
    }
}
