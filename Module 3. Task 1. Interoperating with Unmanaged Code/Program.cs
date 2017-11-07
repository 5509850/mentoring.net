using Module_3.Library.Interoperating_with_Unmanaged_Code;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Module_3.Task_1.Interoperating_with_Unmanaged_Code
{
    class Program
    {
        private const int exit = 9;
        static StringBuilder sb = new StringBuilder();
        static void Main(string[] args)
        {
            int userInput = 0;
            do
            {
                if (userInput > 0 && userInput < 9)
                {
                    Console.Clear();
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
            Console.WriteLine("Сhoose an task number");
            Console.WriteLine();
            Console.WriteLine("1. System Battery State");
            Console.WriteLine("2. System Power Information");
            Console.WriteLine("3. Sleep Mode");
            Console.WriteLine("4. Hibernate Mode");
            Console.WriteLine("5. System uptime");
            Console.WriteLine("6. Create hibernate file");
            Console.WriteLine("7. Delete hibernate file");
            Console.WriteLine("8. Last WakeUP/Sleep Time");
            Console.WriteLine("----------");
            Console.WriteLine($"{exit}. Exit");
            Console.WriteLine("==========");
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
                        GetSystemBatteryState();
                        break;
                    }
                case 2:
                    {
                        ShowsystemPowerInformation();                        
                        break;
                    }
                case 3:
                    {
                        SleepMode();
                        break;
                    }
                case 4:
                    {
                        HibernateMode();
                        break;
                    }
                case 5:
                    {
                        SystemUpTime();
                        break;
                    }

                case 6:
                    {
                        if (IsAdministrator() == true)
                        {
                            CreateHibernateFile();                            
                        }
                        else
                        {
                            return GetAdministrator();
                        }                        
                        break;
                    }

                case 7:
                    {
                        if (IsAdministrator() == true)
                        {
                             DeleteHibernateFile();
                        }
                        else
                        {
                            return GetAdministrator();
                        }                       
                        break;
                    }
                case 8:
                    {
                        SleepTime();
                        Thread.Sleep(1500);
                        Console.Write(sb.ToString());                      
                        break;
                    }
            }
            return false;
        }        

        private static bool GetAdministrator()
        {
            Console.WriteLine("App need Administrtor's privileges for it");
            var exeName = Process.GetCurrentProcess().MainModule.FileName;
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

        private static bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private static void SystemUpTime()
        {
            using (var uptime = new PerformanceCounter("System", "System Up Time"))
            {
                uptime.NextValue();
                Console.WriteLine((TimeSpan.FromSeconds(uptime.NextValue())).ToReadableString());
                Console.ReadKey();
            }
        }
              
        private static void HibernateMode()
        {
            var ps = new PowerState();
            ps.SetSuspendState(true, true, false);
        }

        private static void SleepMode()
        {
            var ps = new PowerState();
            ps.SetSuspendState(false, false, false);
        }

        private static void GetSystemBatteryState()
        {
            try
            {
                SystemBatteryState sbs;
                var ps = new PowerState();
                sbs = ps.GetSystemBatteryState();
                var fields = typeof(SystemBatteryState).GetFields(BindingFlags.Public | BindingFlags.Instance);
                if (fields != null)
                {
                    for (int i = 0; i < fields.Length; ++i)
                    {
                        Console.WriteLine($"{fields[i].Name}: {fields[i].GetValue(sbs)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }            
            Console.ReadKey();
        }

        private static void ShowsystemPowerInformation()
        {
            try
            { 
                SystemPowerInformation spi;
                var ps = new PowerState();
                spi = ps.ShowSystemPowerInformation();
                var fields = typeof(SystemPowerInformation).GetFields(BindingFlags.Public | BindingFlags.Instance);
                if (fields != null)
                {
                    for (int i = 0; i < fields.Length; ++i)
                    {
                        Console.WriteLine($"{fields[i].Name}: {fields[i].GetValue(spi)}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadKey();
        }

        private static void CreateHibernateFile()
        {
            var ps = new PowerState();
            ps.CreateHibernateFile();
            Console.WriteLine(@"file 'C:\pagefile.sys' has been created");
            Console.ReadKey();
        }

        private static void DeleteHibernateFile()
        {
            var ps = new PowerState();
            ps.DeleteHibernateFile();
            Console.WriteLine(@"file 'C:\pagefile.sys' has been deleted");
            Console.ReadKey();
        }

        private static void SleepTime()
        {
            char quotes = '"';
            RunCommandConsole($@"/k wevtutil qe System /q:{quotes}*[System[Provider[@Name = 'Microsoft-Windows-Power-Troubleshooter']]]{quotes} /rd:true /c:1 /f:text");            
        }

        private static void RunCommandConsole(string argument)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("cmd", argument)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                UseShellExecute = false,
                RedirectStandardOutput =  true, 
                CreateNoWindow = true
            };
            Process process = Process.Start(startInfo);           
            process.OutputDataReceived += Process_OutputDataReceived;
            process.BeginOutputReadLine();            
            process.WaitForExit(1000);
        }

        private static void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data.Contains("Sleep Time") || e.Data.Contains("Wake Time"))
            {
                sb.Append(e.Data.Replace('?', ' '));
                sb.Append(Environment.NewLine);
            }
        }
    }
}   

