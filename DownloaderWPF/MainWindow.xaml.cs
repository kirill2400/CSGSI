using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace DownloaderWPF
{
    public partial class MainWindow : Window
    {
        private string[] files;
        private int i = 0;
        public MainWindow()
        {
            InitializeComponent();
            using (WebClient wc = new WebClient())
            {
                var filesdata = wc.DownloadString(new Uri("http://docred.ml/Music/files.txt"));
                files = filesdata.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            label.Content = "0 из " + files.Length;
            TotalPB.Maximum = files.Length;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            #region RemoveCSGSI
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
            #endregion
            foreach (string file in files)
            {
                try
                {
                    var path = Path.GetDirectoryName(file);
                    if (path != string.Empty)
                        Directory.CreateDirectory(path);
                    using (WebClient wc = new WebClient())
                    {
                        var pb = new ProgressBar() { Width = 20, Height = 20 };
                        WrapPB.Children.Add(pb);
                        wc.DownloadProgressChanged += (s, es) => { pb.Value = es.ProgressPercentage; };
                        wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                        wc.DownloadFileAsync(new Uri("http://docred.ml/Music/" + file), file);
                    }
                }
                catch (ArgumentException d) { MessageBox.Show(d.Message); }
            }
        }
        private void Wc_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            label.Content = ++i + " из " + files.Length;
            TotalPB.Value = i;
            if (i == files.Length) // Run
                if (File.Exists("CSGSI Forms.exe"))
                    using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
                    {
                        proc.StartInfo.FileName = "CSGSI Forms.exe";
                        proc.Start();
                        Environment.Exit(0);
                    }
        }
    }
}
