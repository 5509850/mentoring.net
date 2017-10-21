using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Module_1_Async_Await_2
{
    public partial class MainForm : Form
    {            
        private int count = 0;       

        public MainForm()
        {
            InitializeComponent();
        }

        private void button_help_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Напишите простейший менеджер закачек. 
Пользователь задает адрес страницы, которую необходимо загрузить. 
В процессе загрузки пользователь может ее отменить. 
Пользователь может задавать несколько источников для закачки. 
Скачивание страниц не должно блокировать интерфейс приложения.", "Описание программы",MessageBoxButtons.OK , MessageBoxIcon.Information);
        }

        private void button_download_Click(object sender, EventArgs e)
        {
            var uiContext = SynchronizationContext.Current;
            if (Vaidate(textBox_URL.Text))
            {
                label_error.Visible = false;
                var ts = new CancellationTokenSource();
                CancellationToken ct = ts.Token;
                Guid guid = Guid.NewGuid();                
                DownLoadPage(textBox_URL.Text, ct, guid, uiContext);
                AddProgressBar(textBox_URL.Text, ts, guid);
                textBox_URL.Text = "http://";
            }
            else
            {
                label_error.Visible = true;
                textBox_URL.Text = "http://";
            }
        }          

        private async void DownLoadPage(string url, CancellationToken ct, Guid guid, SynchronizationContext uiContext)
        {
            var split = url.Split('/');
            string saveFolder = Path.Combine(Directory.GetCurrentDirectory(), "Download");
            Directory.CreateDirectory(saveFolder);
            string savelocation = Path.Combine(saveFolder, split[split.Length - 1]);
            await Task.Factory.StartNew(() =>
            {
                using (WebClient client = new WebClient())
                {
                    Task.Delay(1500);
                    if (ct.IsCancellationRequested)
                    {
                            return;
                    }
                    try
                    {
                        client.DownloadFile(new Uri(url), savelocation);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                                        
                    if (ct.IsCancellationRequested)
                    {                       
                        if (File.Exists(savelocation))
                        {
                            File.Delete(savelocation);
                        }
                    }
                    else
                    {
                        uiContext.Post(Complete, guid);                       
                    }
                }
            }, ct);           
        }
    
        private void Complete(object obj)
        {            
            try
            {
                Guid guid = (Guid)obj;
                bool alldownloaded = true;
                foreach (Control ctrl in groupBox2.Controls)
                {
                    if (ctrl.Name.Contains("progressBar"))
                    {
                        var prB = (CustomProgressBar)ctrl;
                        if (ctrl.Name.Equals($"progressBar{guid}"))
                        {                           
                            prB.Value = 100;                         
                        }
                        if (prB.Value != 100)
                        {
                            alldownloaded = false;
                        }
                    }                    
                }
                if (alldownloaded)
                {
                    var saveFolder = Path.Combine(Directory.GetCurrentDirectory(), "Download");
                    System.Diagnostics.Process.Start("explorer.exe", saveFolder);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
        }

        private void AddProgressBar(string url, CancellationTokenSource ts, Guid guid)
        {
            count++;
            int Y = count * 19;
            label_count.Text = $"count = {count}";
            int progressX = 10;
            int buttonX = 474;

            Button btn = new Button()
            {
                BackColor = Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0))))),
                Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204))),
                Location = new Point(buttonX, Y),
                Name = guid.ToString(),
                Size = new Size(31, 23),
                Text = "X",
                UseVisualStyleBackColor = false,
                Tag = ts
            };
            btn.Click += Btn_Click;
            groupBox2.Controls.Add(btn);
            CustomProgressBar progressBar = new CustomProgressBar()
            {
                Location = new Point(progressX, Y),
                Name = $"progressBar{guid}",
                Size = new Size(458, 23),
                DisplayStyle = ProgressBarDisplayText.CustomText,
                CustomText = url
            };
            progressBar.Value = 20;
            groupBox2.Controls.Add(progressBar);
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Guid index;           
            Button button = (Button)sender;
            Guid.TryParse(button.Name, out index);
            CancellationTokenSource ts = (CancellationTokenSource)button.Tag;
            if (ts != null)
            {
                ts.Cancel();
            }
            CustomProgressBar prB = null;
            Button btn = null;
            foreach (Control ctrl in groupBox2.Controls)
                {
                    if (ctrl.Name.Equals($"{index}"))
                    {
                        btn = (Button)ctrl;
                        btn.Click -= Btn_Click;
                    }
                    if (ctrl.Name.Equals($"progressBar{index}"))
                    {
                        prB = (CustomProgressBar)ctrl;                        
                    }                    
                }
                if (prB != null && btn != null)
                {
                    groupBox2.Controls.Remove(prB);
                    groupBox2.Controls.Remove(btn);
                    ReBuildControl();
                }            
        }

        private void ReBuildControl()
        {
            count = 0;                    
            int progressX = 10;
            int buttonX = 474;
            List<Control> contrlols = new List<Control>();
            foreach (Control ctrl in groupBox2.Controls)
            {
                contrlols.Add(ctrl);
            }
            groupBox2.Controls.Clear();
            int Y = 0;
            int line = 0;
            foreach (Control ctrl in contrlols)
            {
                line++;
                Y = (count + 1) * 19;
                if (line % 2 != 0)
                {
                    ctrl.Location = new Point(buttonX, Y);
                }
                else
                {
                    ctrl.Location = new Point(progressX, Y);
                    count++;
                }
                groupBox2.Controls.Add(ctrl);
            }
            label_count.Text = $"count = {count}";
        }

        private bool Vaidate(string uriName)
        {
            if (string.IsNullOrEmpty(uriName))
            {
                return false;
            }
            if (!Uri.IsWellFormedUriString(uriName, UriKind.RelativeOrAbsolute))
            {
                return false;
            }
            foreach (Control ctrl in groupBox2.Controls)
            {
                if (ctrl.Name.Contains("progressBar"))
                {
                    var prB = (CustomProgressBar)ctrl;
                    if (prB.Value != 100 && prB.CustomText.Equals(uriName))
                    {
                        MessageBox.Show("Duplicate URL!");
                        return false;
                    }
                }
            }
            var split = uriName.Split('/');
            if (split == null || split.Length < 2)
            {
                return false;
            }
            string saveFolder = Path.Combine(Directory.GetCurrentDirectory(), "Download");
            string savelocation = Path.Combine(saveFolder, split[split.Length - 1]);
            if (File.Exists(savelocation))
            {
                try
                {
                    File.Delete(savelocation);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Wait for Cancel!");
                    return false;
                }
                if (File.Exists(savelocation))
                {
                    MessageBox.Show("Wait for Cancel!");
                    return false;
                }
            }
            return true; ;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox_URL.Text = "http://7-zip.org/a/7z1701-x64.exe";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox_URL.Text = "https://downloads.sourceforge.net/project/openofficeorg.mirror/4.1.3/binaries/ru/Apache_OpenOffice_4.1.3_Win_x86_install_ru.exe";            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox_URL.Text = "http://aimp.su/storage/aaccd58e9dc040471baec47c976e1b75/aimp_4.13.1897.exe";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBox_URL.Text = "https://download.tortoisegit.org/tgit/2.5.0.0/TortoiseGit-2.5.0.0-64bit.msi";
            
        }
    }
}
