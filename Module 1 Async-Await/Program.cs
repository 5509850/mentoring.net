using System;
using System.Threading.Tasks;

namespace Module_1_Async_Await
{
    /// <summary>
    /// 1.	Напишите консольное приложение для асинхронного расчета суммы целых чисел от 0 до N. 
    /// N задается пользователем из консоли. Пользователь вправе внести новую границу 
    /// в процессе вычислений, что должно привести к перезапуску расчета. 
    /// Это не должно привести к «падению» приложения.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync(args);//.Wait();
        }

        public static void MainAsync(string[] args)
        {
            int userInput = 0;
            do
            {
                if (userInput > 0)
                {
                    Console.WriteLine($"sum = {CalculateSum(userInput)}");
                }
                userInput = DisplayMenu(userInput);
            } while (userInput != 0);            
        }

        private static int DisplayMenu(int oldValue)
        {
            if (oldValue != 0)
            {
                Console.WriteLine($"Old N = {oldValue}");
            }
            Console.WriteLine();
            Console.Write("Enter number N = ");
            var result = Console.ReadLine();
            int N = 0;
            int.TryParse(result, out N);
            return N;
        }

        private static int CalculateSum(int N)
        {
            int sum = 0;
            for (int i = N; i > 0; i--)
            {
                sum += i;
            }
            return sum;
        }

        //private static async Task<int> Run(int userInput)
        //{
        //    int a = 0;
        //    try
        //    {
        //        a = await Task.Factory.StartNew<int>(Func <int>(Go()));
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    return await Complete(a);
        //}       

        private static Task Complete()
        {
            var result = new TaskCompletionSource<bool>();
            result.SetResult(true);
            return result.Task;
        }

        private static Task<T> Complete<T>(T result)
        {
            var r = new TaskCompletionSource<T>();
            r.SetResult(result);
            return r.Task;
        }
    }
}
