using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class task2
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

        void FirstTask()
        {
            var rand = new Random();            
            for (int i = 0; i < 10; i++)
            {
                array[i] = rand.Next(1, 10000000);
                Console.WriteLine(array[i]);
            }
            Console.WriteLine("***************");
        }

        void SecondTask()
        {
            var rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                array[i] *= rand.Next(2, 15);
                Console.WriteLine(array[i]);
            }
            Console.WriteLine("***************");
        }

        void ThirdTask()
        {
            array = array.OrderBy(i => i).ToArray();
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine(array[i]);
            }
            Console.WriteLine("***************");
        }

        void FourthTask()
        {
            long total = 0;
            for (int i = 0; i < 10; i++)
            {
                total += array[i];
            }           
            Console.WriteLine(total/10);
        }

    }
}
