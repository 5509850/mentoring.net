using System;
using System.Text;
using System.Threading.Tasks;

namespace Module1_Multi_threading
{
    /// <summary>
    /// 3.	Write a program, which multiplies two matrices and uses class Parallel. 

    /// For check Result look
    /// http://ru.onlinemschool.com/math/assistance/matrix/multiply/
    /// </summary>
    public class Task3
    {
        int m = 5;
        int n = 4;
        int q = 6;              

        public void Run()
        {
            int[,] A = new int[m, n];
            int[,] B = new int[n, q];
           
            Init(A);
            Init(B);
            Console.WriteLine("A:");
            Show(A);
            Console.WriteLine("B");
            Show(B);
            Console.WriteLine("A * B =");
            DateTime start = DateTime.Now;
            Show(Multiply(A, B));
            TimeSpan span = DateTime.Now - start;
            Console.WriteLine(string.Format("Finish: {0} ms", (int)span.TotalMilliseconds));
            Console.ReadKey();
        }

      private void Init(int[,] array)
        {
            var rand = new Random(array.GetLength(0) + array.GetLength(1));
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i,j] = rand.Next(1, 100); ;
                }
            }                 
        }

        private int[,] Multiply(int[,] a, int[,] b)
        {
            int[,] Result = new int[a.GetLength(0), b.GetLength(1)];
            Parallel.For(0, a.GetLength(0), i => {
                for (int j = 0; j < b.GetLength(1); j++)
                {
                    for (int k = 0; k < a.GetLength(1); k++)
                    {
                        Result[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
                      );
            return Result;
        }

        private void Show(int[,] array)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                   sb.Append(array[i, j]);
                   sb.Append(" ");
                }
                sb.Append(Environment.NewLine);
            }
            Console.WriteLine(sb.ToString());
        }
    }
}
