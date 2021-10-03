using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
        public MainForm(InitializationInformation info)
        {
            InitializeComponent();
            fb = new FormBacker(
                info,
                AppIdTextBox, 
                StartButton,
                StatusLabel,
                StopButton,
                UpdateButton,
                UseTimestampCheckbox,
                OverrideTimestampCheckbox,
                DrpDetailTextbox,
                DrpStateTextbox,
                LargeImageTextTextBox,
                LargeImageKeyTextbox,
                SmallImageTextTextbox,
                SmallImageKeyTextBox,
                OverrideTimeDTP
                );
                this.Shown += new EventHandler(delegate { fb.OnFormReady(); });
        }

        private void AppIdTextBox_TextChanged(object sender, EventArgs e) => fb.OnAPPIDTBChanged();

        private void StartButton_Click(object sender, EventArgs e) => fb.StartOnClick();

        private void StopButton_Click(object sender, EventArgs e) => fb.StopOnClick();

        private void GithubButton_Click(object sender, EventArgs e) => Process.Start("https://github.com/naquino14/DRP-Everything");

        private void UseTimestampCheckbox_CheckedChanged(object sender, EventArgs e) => fb.UseTimestampOnUpdate();

        private void OverrideTimestampCheckbox_CheckedChanged(object sender, EventArgs e) => fb.OverrideTimestampOnUpdate();

        private void UpdateButton_Click(object sender, EventArgs e) => fb.Update();

        private void SaveButton_Click(object sender, EventArgs e) => fb.SaveOnClick();

        private void LoadButton_Click(object sender, EventArgs e) => fb.LoadOnClick();

        private void ShortcutGenButton_Click(object sender, EventArgs e) => fb.CreateShortcutOnClick();
    }
}
