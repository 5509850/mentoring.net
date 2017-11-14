using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace Module4TopShelfService
{
    internal class BondingService
    {        
        private readonly string logName;
        private readonly string workImagesFolder;
        private readonly string errorFormatFolder;
        FileSystemWatcher watcher;
        Thread workThread;
        string inDir;
        string outDir;
        string prefix;
        string[] ext;
        string textBarcode;
        int timeout = 1000;
        ManualResetEvent stopWorkEvent;
        AutoResetEvent newFileEvent;
        static Mutex sync = new Mutex();
        DateTime lastImageTime;
        bool stopNow = false;

        public BondingService(string inDir, string outDir, string prefix, string[] ext, string textBarcode, int timeout)
        {           
            logName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Module4TopShelfService.log");
            workImagesFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "workimages");
            errorFormatFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "errorFormatFolder");
            this.inDir = inDir;
            this.outDir = outDir;
            this.prefix = prefix;
            this.ext = ext;
            this.timeout = timeout;
            this.textBarcode = textBarcode;
            lastImageTime = new DateTime(1900, 1, 1);
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
            if (!Directory.Exists(workImagesFolder))
            {
                Directory.CreateDirectory(workImagesFolder);
            }
            if (!Directory.Exists(errorFormatFolder))
            {
                Directory.CreateDirectory(errorFormatFolder);
            }
            FirstScan();
            watcher = new FileSystemWatcher(inDir);
            watcher.Created += Watcher_Created;
            workThread = new Thread(WorkProcedure);
            stopWorkEvent = new ManualResetEvent(false);
            newFileEvent = new AutoResetEvent(false);
        }

        private void WriteLog(string text)
        {
            File.AppendAllText(logName, DateTime.Now.ToLongTimeString() + $" {text}\n");
        }

        public void Start()
        {
            WriteLog("Service is started");
            workThread.Start();
            watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            stopNow = true;
            WriteLog("Service is stopped");
            watcher.EnableRaisingEvents = false;
            stopWorkEvent.Set();
            workThread.Join();            
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (lastImageTime == new DateTime(1900, 1, 1))
            {
                lastImageTime = DateTime.Now;
            }            
            newFileEvent.Set();
        }

        private void WorkProcedure(object obj)
        {     
            do
            {
                if (!Directory.Exists(inDir))
                {
                    Directory.CreateDirectory(inDir);
                }
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
                        var outFile = Path.Combine(workImagesFolder, Path.GetFileName(file));
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
                                    WriteLog($"{fullname} is processed");
                                }
                            }
                            catch (Exception ex)
                            {
                                WriteLog($"{fullname} error when processed: {ex.Message}");
                                Thread.Sleep(5000);
                            }
                        }
                    }
                }
                if (lastImageTime != new DateTime(1900, 1, 1))
                {
                    TimeSpan span = DateTime.Now - lastImageTime;
                    if ((int)span.TotalMilliseconds > timeout)
                    {
                        if (BondingFiles())
                        {
                            lastImageTime = new DateTime(1900, 1, 1);
                        }
                        else
                        {
                            lastImageTime = DateTime.Now;
                        }
                    }
                }
            }
            while (WaitHandle.WaitAny(new WaitHandle[] { stopWorkEvent, newFileEvent }, 2000) != 0);
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
        //filter by mask files <prefix>_<number>.<png|jpeg|…>  
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
                    var outFile = Path.Combine(workImagesFolder, Path.GetFileName(file));
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
                                WriteLog($"{fullname} is processed");
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteLog($"{fullname} is error: {ex.Message}");
                            Thread.Sleep(5000);
                        }
                    }
                }
            }
            bool isAllFileProcessed = false;
            do
            {
                isAllFileProcessed = BondingFiles();
            }
            while (!isAllFileProcessed && !stopNow);            
        }

        private bool BondingFiles()
        {
            bool isAllFileProcessed = true;
            sync.WaitOne();           
            var document = new Document();
            var section = document.AddSection();
            int oldNumber = 0;
            bool firstfile = true;
            int currentNumber = 0;
            List<string> filesForDelete = new List<string>();
            try
            {
                foreach (var file in Directory.EnumerateFiles(workImagesFolder))
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
                        isAllFileProcessed = false;
                        break;
                    }
                    var result = BarCode.IsBarCode(file, textBarcode);
                    if (result == BarCodeResult.Equals)
                    {
                        filesForDelete.Add(file);
                        isAllFileProcessed = false;
                        break;
                    }
                    else
                    {
                        if (result == BarCodeResult.BrkokenFormat)
                        {
                            filesForDelete.Add(file);
                            MoveToErrorFormatFolder(filesForDelete);
                            WriteLog($"BrkokenFormat in {file}");
                            sync.ReleaseMutex();
                            isAllFileProcessed = false;
                            return isAllFileProcessed;
                        }
                    }                    
                    oldNumber = number;
                    var img = section.AddImage(file);
                    img.Height = document.DefaultPageSetup.PageHeight;
                    img.Width = document.DefaultPageSetup.PageWidth;
                    if (stopNow)
                    {
                        return isAllFileProcessed;
                    }
                    filesForDelete.Add(file);
                }
            }
            catch (Exception ex)
            {
                MoveToErrorFormatFolder(filesForDelete);
                WriteLog($"BondingFiles error: {ex.Message}");
                sync.ReleaseMutex();
                isAllFileProcessed = false;
                return isAllFileProcessed;
            }
            if (currentNumber != 0)
                {
                    try
                    {
                        if (stopNow)
                        {
                            return isAllFileProcessed;
                        }
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
                            WriteLog($"{currentNumber}.pdf is created");
                        }
                        DeleteFiles(filesForDelete);
                    }
                    catch (Exception ex)
                    {
                       MoveToErrorFormatFolder(filesForDelete);                       
                       Thread.Sleep(5000);
                       WriteLog($"{currentNumber}.pdf error: {ex.Message}");
                    }
                }           
            sync.ReleaseMutex();
            return isAllFileProcessed;
        }

        /// <summary>
        /// Одна или несколько страниц для многостраничного файла оказались «битыми» 
        /// (например, неверный формат). Возможная реакция – перемещение всей последовательности 
        /// в отдельную папку «битых», для дальнейшей ручной корректировки
        /// </summary>
        /// <param name="filesForDelete"></param>
        private void MoveToErrorFormatFolder(List<string> files)
        {           
            string currentfile = string.Empty;
            try
            {
                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        var fullname = file.Split('\\')[file.Split('\\').Length - 1];
                        var outFile = Path.Combine(errorFormatFolder, Path.GetFileName(file));                        
                        File.Move(file, outFile);
                        WriteLog($"{fullname} is broken format moved to errorFormat Folder");
                    }
                }
            }
            catch (IOException ex)
            {
                Thread.Sleep(5000);
                WriteLog($"{currentfile}.pdf moving broken format error: P{ex.Message}");
                DeleteFiles(files);
            }            
        }

        private void DeleteFiles(List<string> files)
        {
            string currentfile = string.Empty;
            try
            {
                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        currentfile = file.Split('\\')[file.Split('\\').Length - 1];
                        File.Delete(file);
                    }
                }
            }
            catch (IOException ex)
            {
                Thread.Sleep(5000);
                WriteLog($"{currentfile}.pdf deleting error: P{ex.Message}");
                DeleteFiles(files);
            }
        }
    }
}
