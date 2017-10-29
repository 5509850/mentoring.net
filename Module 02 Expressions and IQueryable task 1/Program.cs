using System;

namespace Module_02_Expressions_and_IQueryable_task_1
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
            } while (userInput != 3);           
        }

        static public int DisplayMenu()
        {
            Console.WriteLine();
            Console.WriteLine("************************");
            Console.WriteLine("Сhoose an task number");
            Console.WriteLine();
            Console.WriteLine("1. task");
            Console.WriteLine("2. task");
           
            Console.WriteLine("----------");
            Console.WriteLine("3. Exit");
            var result = Console.ReadLine();
            int code = 0;
            int.TryParse(result, out code);
            return code;
        }
     
        static void Run(int menuItem)
        {
            switch (menuItem)
            {
                case 1:
                    {
                        var task = new Task1();
                        task.Run();
                        break;
                    }
                case 2:
                    {
                        var task = new Task2();
                        task.Run();
                        break;
                    }               
            }

        }

    }
}
