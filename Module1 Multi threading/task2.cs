using System;
using System.Linq;
using System.Threading.Tasks;

namespace Module1_Multi_threading
{
    /// <summary>
    /// 2.	Write a program, which creates a chain of four Tasks. 
    /// First Task – creates an array of 10 random integer. 
    /// Second Task – multiplies this array with another random integer. 
    /// Third Task – sorts this array by ascending. 
    /// Fourth Task – calculates the average value. 
    /// All this tasks should print the values to console
    /// </summary>
    public class Task2
    {
        int[] array = new int[10];
        public void Run()
        {
            var firstTask = new Task(
                () => FirstTask());
            firstTask.ContinueWith((t) => SecondTask())
                .ContinueWith((t) => ThirdTask())
                .ContinueWith((t) => FourthTask());
            firstTask.Start();
            Console.ReadKey();
        }

        private void FirstTask()
        {
            var rand = new Random();            
            for (int i = 0; i < 10; i++)
            {
                array[i] = rand.Next(1, 10000000);
                Console.WriteLine(array[i]);
            }
            Console.WriteLine("***************");
        }

        private void SecondTask()
        {
            var rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                array[i] *= rand.Next(2, 15);
                Console.WriteLine(array[i]);
            }
            Console.WriteLine("***************");
        }

        private void ThirdTask()
        {
            array = array.OrderBy(i => i).ToArray();
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(array[i]);
            }
            Console.WriteLine("***************");
        }

        private void FourthTask()
        {            
            Console.WriteLine(array.Average());
        }
    }
}
