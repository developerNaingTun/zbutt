using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.IO;
using System.Reflection;

namespace zbutt
{
    static class Program
    {
        [DllImport("user32.Dll")]
        private static extern int EnumWindows(EnumWinCallBack callBackFunc, int lParam);

        [DllImport("User32.Dll")]
        private static extern void GetWindowText(IntPtr hWnd, StringBuilder str, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern Boolean ShowWindow(IntPtr hWnd, Int32 nCmdShow);

        [DllImport("user32")]
        public static extern int SetWindowPos(IntPtr hwnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        private static bool EnumWindowCallBack(int hwnd, int lParam)
        {
            windowHandle = (IntPtr)hwnd;
            StringBuilder sbuilder = new StringBuilder(256);
            GetWindowText(windowHandle, sbuilder, sbuilder.Capacity);
            string strTitle = sbuilder.ToString();
            if (strTitle == ApplicationTitle)
            {
                ShowWindow(windowHandle, 1);
                SetForegroundWindow(windowHandle);
                string[] argv = Environment.GetCommandLineArgs();
                if (argv.Length > 1)
                {
                    CopyDataHelper.SendMsgString(windowHandle, argv[1]);
                }
                return false;
            }
            return true;
        }

        private static bool IsAlreadyRunning()
        {
            string strLoc = Assembly.GetExecutingAssembly().Location;

            FileSystemInfo fileInfo = new FileInfo(strLoc);
            string sExeName = fileInfo.Name;
            mutex = new Mutex(true, sExeName);

            if (mutex.WaitOne(0, false))
            {
                return false;
            }
            return true;

        }

        static Mutex mutex;
        const int SW_RESTORE = 9;
        static string sTitle = "Transcode";
        static IntPtr windowHandle;
        delegate bool EnumWinCallBack(int hwnd, int lParam);

        public static string ApplicationTitle
        {
            get
            {
                return sTitle;
            }
        }      


        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (IsAlreadyRunning())
            {
                MessageBox.Show("Transcode is already running.","Transcode",MessageBoxButtons.OK,MessageBoxIcon.Exclamation);
                EnumWindows(new EnumWinCallBack(EnumWindowCallBack), 0);
                return;
            }
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new formMain());
            }
            catch
            {

            }
        }
    }
}