using System;
using System.Drawing;
using System.Windows.Forms;
namespace PAIN.EXE {
    public partial class Form1 : Form {
        Random r = new Random();
        public Form1() => InitializeComponent();
        private void timer1_Tick(object sender, EventArgs e) {
            BackColor = Color.FromArgb(r.Next(0, 255), r.Next(0, 255), r.Next(0, 255));
        }
    }
}
