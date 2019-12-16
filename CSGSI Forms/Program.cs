using System;
using System.Windows.Forms;

namespace CSGSI_Forms
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 0)
                Application.Run(new Menu(3000));
            else
                Application.Run(new Menu(Convert.ToInt32(args[0])));
        }
    }
}
