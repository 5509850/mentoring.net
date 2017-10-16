using System;

namespace Module1_Multi_threading
{
    class Program
    {
        static void Main(string[] args)
        {          
            int userInput = 0;
            do
            {
                if (userInput > 0 && userInput < 8)
                {
                    Run(userInput);
                }
                userInput = DisplayMenu();
            } while (userInput != 8);
        }

        static public int DisplayMenu()
        {
            Console.WriteLine();
            Console.WriteLine("Сhoose an task number");
            Console.WriteLine();
            Console.WriteLine("1. task");
            Console.WriteLine("2. task");
            Console.WriteLine("3. task");
            Console.WriteLine("4. task");
            Console.WriteLine("5. task");
            Console.WriteLine("6. task");
            Console.WriteLine("7. task");
            Console.WriteLine("----------");
            Console.WriteLine("8. Exit");
            var result = Console.ReadLine();
            return Convert.ToInt32(result);
        }

        static void Run(int menuItem)
        {
            switch (menuItem)
            {
                case 1:
                    {
                        var task = new task1();
                        task.Run();
                        break;
                    }
                case 2:
                    {
                        var task = new task2();
                        task.Run();
                        break;
                    }
                case 3:
                    {
                        var task = new task3();
                        task.Run();
                        break;
                    }
                case 4:
                    {
                        var task = new task4();
                        task.Run();
                        break;
                    }
                case 5:
                    {
                        var task = new task5();
                        task.Run();
                        break;
                    }
                case 6:
                    {
                        var task = new task6();
                        task.Run();
                        break;
                    }
                case 7:
                    {
                        var task = new task7();
                        task.Run();
                        break;
                    }
            }
            
        }


    }
}



