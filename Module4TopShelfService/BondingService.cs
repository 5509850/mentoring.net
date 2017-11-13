using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using ZXing;

namespace Module4TopShelfService
{
    internal class BondingService
    {
        private readonly Timer timer;
        private readonly string logName;
        private readonly string workimages;

        FileSystemWatcher watcher;
        Thread workThread;
        string inDir;
        string outDir;
        string prefix;
        string[] ext;
        string textBarcode;
        ManualResetEvent stopWorkEvent;
        AutoResetEvent newFileEvent;

        public BondingService(string inDir, string outDir, string prefix, string[] ext, string textBarcode)
        {
            timer = new Timer(WorkProcedureTimer);
            logName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TopShelfService.log");
            workimages = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "workimages");
            this.inDir = inDir;
            this.outDir = outDir;
            this.prefix = prefix;
            this.ext = ext;
            this.textBarcode = textBarcode;
            if (inDir == null || outDir == null || ext == null || ext.Length == 0 || textBarcode == null)
            {
                throw new ArgumentNullException();
            }
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }
            if (!Directory.Exists(inDir))
            {
                Directory.CreateDirectory(inDir);
            }
            if (!Directory.Exists(workimages))
            {
                Directory.CreateDirectory(workimages);
            }
            FirstScan();
            watcher = new FileSystemWatcher(inDir);
            watcher.Created += Watcher_Created;
            workThread = new Thread(WorkProcedure);
            stopWorkEvent = new ManualResetEvent(false);
            newFileEvent = new AutoResetEvent(false);
        }

        private void WorkProcedureTimer(object state)
        {
            File.AppendAllText(logName, DateTime.Now.ToLongTimeString() + " Work procedure\n");
        }

        public void Start()
        {
            timer.Change(0, 5 * 1000);
            workThread.Start();
            watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {   
            timer.Change(Timeout.Infinite, 0);
            watcher.EnableRaisingEvents = false;
            stopWorkEvent.Set();
            workThread.Join();            
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            newFileEvent.Set();
        }

        private void WorkProcedure(object obj)
        {
            do
            {
                foreach (var file in Directory.EnumerateFiles(inDir))
                {
                    if (stopWorkEvent.WaitOne(TimeSpan.Zero))
                    {
                        return;
                    }
                    var inFile = file;
                    var fullname = file.Split('\\')[file.Split('\\').Length - 1];
                    if (FilterOn(fullname))
                    {
                        var outFile = Path.Combine(workimages, Path.GetFileName(file));
                        if (TryOpen(inFile, 3))
                        {
                            try
                            {
                                if (File.Exists(outFile))
                                {
                                    File.Delete(outFile);
                                }
                                if (!File.Exists(outFile))
                                {
                                    File.Move(inFile, outFile);
                                }
                            }
                            catch (Exception)
                            {
                                Thread.Sleep(5000);
                            }
                        }
                    }
                }
                BondingFiles();
            }
            while (WaitHandle.WaitAny(new WaitHandle[] { stopWorkEvent, newFileEvent }, 1000) != 0);
        }

        private bool TryOpen(string fileName, int tryCount)
        {
            for (int i = 0; i < tryCount; i++)
            {
                try
                {
                    var file = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.None);
                    file.Close();
                    return true;
                }
                catch (IOException)
                {
                    Thread.Sleep(5000);
                }
            }
            return false;
        }

        private bool FilterOn(string fullname)
        {            
            var piecename = fullname.Split('_', '.');
            if (piecename == null || piecename.Length != 3)
            {
                return false;
            }
            int num = 0;
            if (piecename[(int)FilePart.prefix].Equals(prefix) 
                && int.TryParse(piecename[(int)FilePart.number], out num) 
                && (piecename[(int)FilePart.extension].Equals(ext[(int)FilePart.firstExtension]) 
                    || piecename[(int)FilePart.extension].Equals(ext[(int)FilePart.secondExtension])))
            {
                return true;
            }                
            return false;
        }

        private int GetFileNumber(string file)
        {
            var name = file.Split('\\')[file.Split('\\').Length - 1];
            var piecename = name.Split('_', '.');
            if (piecename == null || piecename.Length != 3)
            {
                return 0;
            }
            int num = 0;
            if (piecename[(int)FilePart.prefix].Equals(prefix)
                && int.TryParse(piecename[(int)FilePart.number], out num)
                && (piecename[(int)FilePart.extension].Equals(ext[(int)FilePart.firstExtension])
                    || piecename[(int)FilePart.extension].Equals(ext[(int)FilePart.secondExtension])))
            {
                return num;
            }
            return 0;
        }

        private void FirstScan()
        {
            foreach (var file in Directory.EnumerateFiles(inDir))
            {               
                var inFile = file;
                var fullname = file.Split('\\')[file.Split('\\').Length - 1];
                if (FilterOn(fullname))
                {
                    var outFile = Path.Combine(workimages, Path.GetFileName(file));
                    if (TryOpen(inFile, 3))
                    {
                        try
                        {
                            if (File.Exists(outFile))
                            {
                                File.Delete(outFile);
                            }
                            if (!File.Exists(outFile))
                            {
                                File.Move(inFile, outFile);
                            }
                        }
                        catch (Exception)
                        {
                            Thread.Sleep(5000);
                        }
                    }
                }
            }
            BondingFiles();
        }

        private void BondingFiles()
        {
            var document = new Document();
            var section = document.AddSection();
            int oldNumber = 0;
            bool firstfile = true;
            int currentNumber = 0;
            List<string> filesForDelete = new List<string>();
            foreach (var file in Directory.EnumerateFiles(workimages))
            {
                if (firstfile)
                {
                    firstfile = false;
                    currentNumber = oldNumber 
                        = GetFileNumber(file);
                }
                var number = GetFileNumber(file);
                if ((number - oldNumber) > 1)
                {
                    break;
                }
                if (IsBarCode(file))
                {
                    filesForDelete.Add(file);
                    break;
                }
                oldNumber = number;
                var img = section.AddImage(file);
                img.Height = document.DefaultPageSetup.PageHeight;
                img.Width = document.DefaultPageSetup.PageWidth;              
                filesForDelete.Add(file);
            }
            if (currentNumber != 0)
            {
                try
                {
                    var render = new PdfDocumentRenderer();
                    render.Document = document;
                    render.RenderDocument();
                    var outFile = Path.Combine(outDir, Path.GetFileName($"{currentNumber}.pdf"));
                    if (File.Exists(outFile))
                    {
                        File.Delete(outFile);
                    }
                    if (!File.Exists(outFile))
                    {
                        render.Save(outFile);
                    }                    
                    DeleteFiles(filesForDelete);
                }
                catch (Exception)
                {
                    Thread.Sleep(5000);
                }
            }
        }

        private void DeleteFiles(List<string> files)
        {
            try
            {
                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (IOException)
            {
                Thread.Sleep(5000);
            }
        }

        private bool IsBarCode(string file)
        {
            var reader = new BarcodeReader() { AutoRotate = true };            
            var bmp = (Bitmap)Image.FromFile(file);
            var result = reader.Decode(bmp); 
            return ((result != null) && result.Text.Equals(textBarcode));
        }
    }
}
