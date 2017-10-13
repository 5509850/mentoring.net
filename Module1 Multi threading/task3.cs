using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module1_Multi_threading
{
    /// <summary>
    /// 3.	Write a program, which multiplies two matrices and uses class Parallel. 
    /// </summary>
    public class task3
    {
        public void Run()
        {
            int[] matrix1 = { 1, 4, 3, 6, 4 };
            int[] matrix2 = { 2, 2, 5, 2, 8 };
            List<int[]> list = new List<int[]>();
            list.Add(matrix1);
            list.Add(matrix2);
            Parallel.ForEach(list, Multiply);
            Console.ReadKey();
        }

        //http://ru.onlinemschool.com/math/assistance/matrix/multiply/
        private void Multiply(int[] matrix)
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                Console.WriteLine(matrix[i]);
            }
        }       
    }
}
