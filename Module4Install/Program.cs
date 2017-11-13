using System;
using System.Diagnostics;
using System.Security.Principal;
using System.ServiceProcess;

namespace Module4Install
{
    class Program
    {
        private const int exit = 9;

        static void Main(string[] args)
        {
            int userInput = 0;
            Console.WriteLine("FOR CHECK WORK THIS Service:");            
            Console.WriteLine("Copy files from 'ExampleImages' folder to 'input' folder.");
            Console.WriteLine("See in 'App.config' file (key = 'in')");
            Console.WriteLine("img_001.jpeg, img_002.jpeg, img_004.jpeg - good Images");
            Console.WriteLine("img_003.jpeg - BarCode Images");
            Console.WriteLine("See in 'App.config' file (key = 'textBarcode')");            
            Console.WriteLine("img_555.jpeg - bad broken format");
            Console.WriteLine("folder 'errorFormatFolder' for bad broken format files");            
            Console.WriteLine("result pdf file in 'output' folder (key = 'out')");
            Console.WriteLine("--------------------------------------------------");
            do
            {
                if (userInput > 0 && userInput < 9)
                {                  
                    if (Run(userInput))
                    {
                        userInput = exit;
                        return;
                    }
                }
                userInput = DisplayMenu();
            } while (userInput != exit);
        }

        private static int DisplayMenu()
        {
            Console.WriteLine();
            Console.WriteLine("************************");
            if (IsAdministrator())
            {
                Console.WriteLine("Сhoose an task number (ADMINISTRATOR)");
            }
            else
            {
                Console.WriteLine("Сhoose an task number");
            }   
            Console.WriteLine();
            Console.WriteLine("1. Install TopShelf Service");
            Console.WriteLine("2. Start TopShelf Service");
            Console.WriteLine("3. Stop TopShelf Service");
            Console.WriteLine("4. Uninstall TopShelf Service");
            Console.WriteLine("5. Controller Start TopShelf Service (Admin only)");
            Console.WriteLine("6. Controller Stop TopShelf Service (Admin only)");
            Console.WriteLine("-------------------------------");
            Console.WriteLine();           
            Console.WriteLine($"{exit}. Exit");
            Console.WriteLine("=================================");
            var result = Console.ReadLine();
            int code = 0;
            int.TryParse(result, out code);
            return code;
        }

        private static bool Run(int menuItem)
        {
            switch (menuItem)
            {
                case 1:
                    {
                        ExecuteCommand("install.cmd");                       
                        break;
                    }
                case 2:
                    {
                        ExecuteCommand("start.cmd");
                        break;
                    }
                case 3:
                    {
                        ExecuteCommand("stop.cmd");
                        break;
                    }
                case 4:
                    {
                        ExecuteCommand("uninstall.cmd");
                        break;
                    }
                case 5:
                    {
                        if (IsAdministrator())
                        {
                            try
                            {
                                var controller = new ServiceController("Module4TopShelfService");
                                controller.Start();
                                Console.WriteLine("=================> Module4TopShelfService service is started");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        else
                        {
                            return GetAdministrator();
                        }

                        break;
                    }

                case 6:
                    {
                        if (IsAdministrator())
                        {
                            try
                            {
                                var controller = new ServiceController("Module4TopShelfService");
                                controller.Stop();
                                Console.WriteLine("===================> Module4TopShelfService service is stopped");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                        else
                        {
                            return GetAdministrator();
                        }

                        break;
                    }
            }
            return false;
        }

        private static void ExecuteCommand(string command)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.StandardInput.WriteLine("cd ..");
            cmd.StandardInput.WriteLine("cd ..");
            cmd.StandardInput.WriteLine("cd cmd");
            cmd.StandardInput.WriteLine(command);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close(); 
            Console.WriteLine(cmd.StandardOutput.ReadToEnd());
        }

        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static bool GetAdministrator()
        {
            Console.WriteLine("App need Administrtor's privileges for it!!!! (Enter)");
            Console.ReadKey();
            var exeName = Process.GetCurrentProcess().MainModule.FileName.Replace(".vshost.", ".");
            ProcessStartInfo startInfo = new ProcessStartInfo(exeName);
            startInfo.Verb = "runas";
            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }
    }
}
