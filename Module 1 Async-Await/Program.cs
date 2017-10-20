using System;
using System.Threading;
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
            MainAsync(args).Wait();
        }

        public static async Task MainAsync(string[] args)
        {
            int userInput = 0;
            bool first = true; 
            do
            {   
                var ts = new CancellationTokenSource();
                CancellationToken ct = ts.Token;                
                if (userInput > 0)
                {
                    first = false;             
                    await Task.Factory.StartNew(async () =>
                    {
                        int sum = 0;
                        bool showResult = true;
                        for (int i = userInput; i > 0; i--)
                        {
                            if (ct.IsCancellationRequested)
                            {
                                showResult = false;                                
                                break;
                            }
                            sum += i;
                            await Task.Delay(500);
                        }
                        if (showResult)
                        {
                            Console.WriteLine();
                            Console.WriteLine($"for N = {userInput} sum = {sum}");
                            Console.WriteLine();
                            Console.Write("Enter number N = ");
                        }                       
                    }, ct);                                    
                }               
                userInput = DisplayMenu(userInput);
                if (!first)
                {
                    ts.Cancel();
                }
            } while (userInput != 0);           
        }

        private static int DisplayMenu(int oldValue)
        {            
            Console.WriteLine();
            Console.Write("Enter number N = ");
            var result = Console.ReadLine();
            int N = 0;
            int.TryParse(result, out N);
            return N;
        }
    }
}
