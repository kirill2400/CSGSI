using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Unpacker
{
    public partial class Unpacker : Form
    {
        private string[] files;
        private int i = 0;
        public Unpacker()
        {
            InitializeComponent();
            using (WebClient wc = new WebClient())
            {
                var filesdata = wc.DownloadString(new Uri("http://docred.ml/Music/files.txt"));
                files = filesdata.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            labelTotal.Text = i + " из " + files.Length;
            barTotal.Maximum = files.Length;
        }

        private void DownloadFiles(object sender, EventArgs e)
        {
            RemoveCSGSI();
            foreach (string file in files)
            {
                ThreadPool.QueueUserWorkItem(DownloadFileAsync, file);
            }
        }

        private void DownloadFileAsync(object objfile)
        {
            string file = objfile as string;
            try
            {
                var path = Path.GetDirectoryName(file);
                if (path != string.Empty)
                    Directory.CreateDirectory(path);
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadProgressChanged += Wc_DownloadProgressChanged;
                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                    wc.DownloadFileAsync(new Uri("http://docred.ml/Music/" + file), file);
                }
            }
            catch (ArgumentException d) { MessageBox.Show(d.Message); }
        }

        private void DownloadZip(object sender, EventArgs e)
        {
            RemoveCSGSI();
            using (WebClient wc = new WebClient())
            {
                var stream = wc.OpenRead(new Uri("http://docred.ml/Music.zip"));
                new ZipArchive(stream, ZipArchiveMode.Read).ExtractToDirectory(Environment.CurrentDirectory);
            }
            Run();
        }

        private void Run()
        {
            if (File.Exists("CSGSI Forms.exe"))
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = "CSGSI Forms.exe";
                    proc.Start();
                    Environment.Exit(0);
                }
        }
        private void RemoveCSGSI()
        {
            foreach (string file in files)
            {
                try
                {
                    if (File.Exists(file)) File.Delete(file);
                    var path = Path.GetDirectoryName(file);
                    if (path != string.Empty && Directory.Exists(path))
                        Directory.Delete(path, true);
                }
                catch { }
            }
            if (Directory.Exists("ultimate")) Directory.Delete("ultimate", true);
        }

        private void Wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            labelFiles.Text = e.BytesReceived / 1024 + " КБ из " + e.TotalBytesToReceive / 1024 + " КБ";
            barFiles.Value = e.ProgressPercentage;
        }
        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            labelTotal.Text = ++i + " из " + files.Length;
            barTotal.Value = i;
            if (i == files.Length) Run();
        }
    }
}
