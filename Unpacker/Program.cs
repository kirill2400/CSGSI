using System.Windows.Forms;

namespace Downloader
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Unpacker.Unpacker());
        }
    }
}
