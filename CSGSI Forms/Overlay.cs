using CSGSI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSGSI_Forms
{
    public partial class Overlay : Form, IDisposable
    {
        public const string WINDOW_NAME = "Counter-Strike: Global Offensive";
        IntPtr handle;

        public struct RECT
        {
            public int left, top, right, bottom;
        }
        private RECT rect;

        #region DLLImport
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
        #endregion

        private GameState _gs;
        public Overlay()
        {
            InitializeComponent();
        }

        Font font = new Font("Arial", 16);
        SolidBrush brush = new SolidBrush(Color.Cyan);
        PointF point;
        StringFormat format = new StringFormat();
        private void Setup()
        {
            point = new PointF(10.0f, rect.bottom - 50.0f);
        }
        internal void SetGS(GameState gs)
        {
            _gs = gs;
            Invalidate();
        }
        internal new void Show()
        {
            Overlay_Load(null, null);
            base.Show();
        }
        private void Overlay_Load(object sender, EventArgs e)
        {
            if ((handle = FindWindow(null, WINDOW_NAME)) == (IntPtr)null)
                return;
            int initialStyle = GetWindowLong(this.Handle, -20);
            SetWindowLong(this.Handle, -20, initialStyle | 0x80000 | 0x20);

            GetWindowRect(handle, out rect);
            this.Size = new Size(rect.right - rect.left, rect.bottom - rect.top);
            this.Top = rect.top;
            this.Left = rect.left;
            Setup();
        }
        private Graphics g;
        private void Overlay_Paint(object sender, PaintEventArgs e)
        {
            g = e.Graphics;
            if (_gs != null)
                g.DrawString(_gs.Player.Name + "'s health: " + _gs.Player.State.Health
                    + " Armor: " + _gs.Player.State.Armor, font, brush, point);
            g.CopyFromScreen(Cursor.Position.X - 10, Cursor.Position.Y - 10, 0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            this.Invalidate();
        }

        public new void Dispose()
        {
            base.Dispose();
            font.Dispose();
            brush.Dispose();
            format.Dispose();
        }
    }
}
