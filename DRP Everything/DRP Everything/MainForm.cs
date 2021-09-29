using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DRP_Everything
{
    public partial class MainForm : Form
    {
        FormBacker fb;
        public MainForm()
        {
            InitializeComponent();
            fb = new FormBacker(
                AppIdTextBox, 
                StartButton,
                StatusLabel,
                StopButton
                );
        }

        private void AppIdTextBox_TextChanged(object sender, EventArgs e) => fb.OnAPPIDTBChanged();

        private void StartButton_Click(object sender, EventArgs e) => fb.Start();
    }
}
