using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using Topshelf;

namespace Module4TopShelfService
{
    class Program
    {
        static void Main(string[] args)
        {
            var currentDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);           
            var inDir = Path.Combine(currentDir, ConfigurationManager.AppSettings["in"]);
            var outDir = Path.Combine(currentDir, ConfigurationManager.AppSettings["out"]);
            var prefix = ConfigurationManager.AppSettings["prefix"];            
            string[] ext = {ConfigurationManager.AppSettings["ext"], ConfigurationManager.AppSettings["ext2"]};
            var textBarcode = ConfigurationManager.AppSettings["textBarcode"];
            HostFactory.Run(
               x =>
               {
                   x.Service<BondingService>(
                   s =>
                   {
                       s.ConstructUsing(() => new BondingService(inDir, outDir, prefix, ext, textBarcode));
                       s.WhenStarted(serv => serv.Start());
                       s.WhenStopped(serv => serv.Stop());
                   });
                   x.SetServiceName("Module4TopShelfService");
                   x.SetDisplayName("Module 4 TopShelf Bonding Service");
                   x.StartAutomaticallyDelayed();
                   x.RunAsLocalService();
               });
        }
    }
}
