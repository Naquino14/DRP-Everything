// ver 1.0.0
using System;
using System.Windows.Forms;
using System.Diagnostics;

using IWshRuntimeLibrary;

using DiscordRPC;
using DiscordRPC.Message;
using System.IO;

namespace DRP_Everything
{
    public class FormBacker
    {
        SaveFileDialog sfd;
        OpenFileDialog ofd;

        SaveData saveData;
        InitializationInformation info;

        string appId;
        DiscordRpcClient client;
        bool allowInitialize = false;
        bool initialized = false;

        bool useTimestamp = false;
        bool overrideTimestamp = false;

        TextBox appIdTb;

        TextBox detailTb;
        TextBox stateTb;

        TextBox largeImageTextTb;
        TextBox largeImageKeyTb;
        TextBox smallImageTextTb;
        TextBox smallImageKeyTb;

        System.Windows.Forms.Button startButton;
        System.Windows.Forms.Button stopButton;
        System.Windows.Forms.Button updateButton;

        CheckBox useTimestampCb;
        CheckBox overrideTimestampCb;

        Label connectionStatusLabel;

        DateTimePicker overrideTimeDTP;

        Process shortcutProcess;

        #region DRP Update Properties

        DateTime timeStamp;

        readonly string filter = "DRP Everything Config (*.drpec)|*.drpec";

        #endregion

        public FormBacker(
            InitializationInformation info,
            TextBox appIdTb,
            System.Windows.Forms.Button startButton,
            Label connectionStatusLabel,
            System.Windows.Forms.Button stopButton,
            System.Windows.Forms.Button updateButton,
            CheckBox useTimestampCb,
            CheckBox overrideTimestampCb,
            TextBox detailTb,
            TextBox stateTb,
            TextBox largeImageTextTb,
            TextBox largeImageKeyTb,
            TextBox smallImageTextTb,
            TextBox smallImageKeyTb,
            DateTimePicker overrideTimeDTP
            ) {
            this.info = info;
            this.appIdTb = appIdTb;
            this.startButton = startButton;
            this.connectionStatusLabel = connectionStatusLabel;
            this.stopButton = stopButton;
            this.updateButton = updateButton;
            this.useTimestampCb = useTimestampCb;
            this.overrideTimestampCb = overrideTimestampCb;
            this.detailTb = detailTb;
            this.stateTb = stateTb;
            this.largeImageTextTb = largeImageTextTb;
            this.largeImageKeyTb = largeImageKeyTb;
            this.smallImageTextTb = smallImageTextTb;
            this.smallImageKeyTb = smallImageKeyTb;
            this.overrideTimeDTP = overrideTimeDTP;

            sfd = new SaveFileDialog();
            ofd = new OpenFileDialog();

            saveData = new SaveData();

            // check for shortcut
            if (this.info.useArgs == true)
            {
                saveData = new DataSerializer().Load(info.configPath);
                shortcutProcess = Process.Start(info.executablePath);
                shortcutProcess.Exited += new EventHandler(delegate { OnAttatchedApplicationClose(); });
            }
        }

        public void StartOnClick()
        {
            if (!allowInitialize)
                return;
            appId = appIdTb.Text;
            if (!overrideTimestamp)
                timeStamp = DateTime.Now;
            if (!initialized)
            { 
                client = new DiscordRpcClient(appId); 
                client.Initialize(); 
                initialized = true;
                // method subscription
                client.OnConnectionEstablished += OnConnectionEstablished;
                client.OnConnectionFailed += OnConnectionFailed;
                client.OnClose += OnConnectionClosed;
            }
        }

        public void StopOnClick()
        {
            if (!initialized)
            { MessageBox.Show("Client is not initialized!", "Not Initialized!", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            client.Deinitialize();
            // onDisconnect doesnt fire automatically so the event is prob not being triggered or somet
            OnConnectionClosed(null, null);
        }

        public void OnAPPIDTBChanged()
        {
            if (appIdTb.Text == null)
            { 
                allowInitialize = false;
                startButton.Enabled = false;
            }
            else 
            {
                allowInitialize = true;
                startButton.Enabled = true;
            }
        }

        public void Update()
        {
            client.UpdateState(stateTb.Text);
            client.UpdateDetails(detailTb.Text);

            client.UpdateLargeAsset(largeImageKeyTb.Text, largeImageTextTb.Text);
            client.UpdateSmallAsset(smallImageKeyTb.Text, smallImageTextTb.Text);

            if (useTimestamp) // knon bug: for some reason the default time starts at 4 hours. i think the override time isnt working either.
            {
                if (overrideTimestamp)
                    client.UpdateStartTime(overrideTimeDTP.Value);
                else
                    client.UpdateStartTime(DateTime.Now);
            }
        }

        public void SaveOnClick()
        {
            PrepareData();
            string path;
            sfd.Filter = filter;
            if (sfd.ShowDialog() == DialogResult.OK)
            { path = sfd.FileName; new DataSerializer().SaveData(saveData, path); }
        }

        private void PrepareData()
        {
            saveData.appId = appIdTb.Text;
            saveData.state = stateTb.Text;
            saveData.detail = detailTb.Text;
            saveData.largeImageKey = largeImageKeyTb.Text;
            saveData.largeImageText = largeImageTextTb.Text;
            saveData.smallImageKey = smallImageKeyTb.Text;
            saveData.smallImageText = smallImageTextTb.Text;

            saveData.useTimestamp = useTimestampCb.Checked;
            saveData.overrideTimestamp = overrideTimestampCb.Checked;

            if (useTimestamp)
            {
                if (overrideTimestamp)
                    saveData.timeStamp = overrideTimeDTP.Value;
                else
                    saveData.timeStamp = DateTime.Now;
            }
        }

        public void LoadOnClick()
        {
            string path;
            ofd.Filter = filter;
            if (ofd.ShowDialog() == DialogResult.OK)
            { path = ofd.FileName; saveData = new DataSerializer().Load(path); }

            Load();
        }

        private void Load()
        {
            #pragma warning disable IDE0003
            appIdTb.Invoke(new MethodInvoker(delegate { appIdTb.Text = this.saveData.appId; }));
            stateTb.Invoke(new MethodInvoker(delegate { stateTb.Text = this.saveData.state; }));
            detailTb.Invoke(new MethodInvoker(delegate { detailTb.Text = this.saveData.detail; }));
            largeImageKeyTb.Invoke(new MethodInvoker(delegate { largeImageKeyTb.Text = this.saveData.largeImageKey; }));
            largeImageTextTb.Invoke(new MethodInvoker(delegate { largeImageTextTb.Text = this.saveData.largeImageText; }));
            smallImageKeyTb.Invoke(new MethodInvoker(delegate { smallImageKeyTb.Text = this.saveData.smallImageText; }));
            smallImageTextTb.Invoke(new MethodInvoker(delegate { smallImageTextTb.Text = this.saveData.smallImageText; }));
            useTimestampCb.Invoke(new MethodInvoker(delegate { useTimestampCb.Checked = this.saveData.useTimestamp; }));
            overrideTimestampCb.Invoke(new MethodInvoker(delegate { overrideTimestampCb.Checked = this.saveData.overrideTimestamp; }));
            #pragma warning restore IDE0003
        }
        public void CreateShortcutOnClick()
        {
            string configPath = null;
            string execuablePath = null;
            string shortcutPath = null;
            string shortcutIconPath = null;
            var r = MessageBox.Show("Select a location for the shortcut.", "Select a file.", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (r == DialogResult.Cancel)
                return;
            sfd.Filter = "Windows shortcut files (*.lnk)|*.lnk";
            if (sfd.ShowDialog() == DialogResult.OK)
                shortcutPath = sfd.FileName;
            var r1 = MessageBox.Show("Select a save location for the current configuration.", "Select a file.", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (r1 == DialogResult.Cancel)
                return;
            sfd.Filter = filter;
            if (sfd.ShowDialog() == DialogResult.OK)
                configPath = sfd.FileName;
            var r2 = MessageBox.Show("Select executable to run when application is opened.", "Select a file.", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            if (r2 == DialogResult.Cancel)
                return;
            ofd.Filter = "All files (*.*)|*.*";
            if (ofd.ShowDialog() == DialogResult.OK)
                execuablePath = ofd.FileName;
            var r3 = MessageBox.Show("Would you like to specify an icon for the shortcut?", "Set a shortcut icon.", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (r3 == DialogResult.Cancel)
                return;
            else if (r3 == DialogResult.Yes)
            {
                ofd.Filter = "Icon files (*.ico)|*.ico";
                ofd.InitialDirectory = Path.GetDirectoryName(ofd.FileName);
                if (ofd.ShowDialog() == DialogResult.OK)
                    shortcutIconPath = ofd.FileName;
            }
            PrepareData();
            new DataSerializer().SaveData(saveData, configPath);

            // get name of application
            var appName = execuablePath.Split('\\')[execuablePath.Split('\\').Length - 1].Split('.')[0];

            var shortcut = (IWshShortcut)new WshShell().CreateShortcut(shortcutPath);
            shortcut.Arguments = $"\"{configPath}\" \"{execuablePath}\"";
            shortcut.TargetPath = $@"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\DRP Everything.exe";
            shortcut.Description = $"Start DRP Everything with {appName}";
            shortcut.IconLocation = $@"{shortcutIconPath ??= $@"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)}\icon.ico"}";
            shortcut.Save();
            MessageBox.Show($"Success! Created shortcut for {appName}", "Shortcut Created.", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnAttatchedApplicationClose() { StopOnClick(); OnClose(); Application.Exit(); }

        public void OnClose() => client.Dispose();

        public void UseTimestampOnUpdate()
        {
            switch (useTimestampCb.Checked)
            {
                case true:
                    useTimestamp = true;
                    overrideTimestampCb.Invoke(new MethodInvoker(delegate { overrideTimestampCb.Enabled = true; }));
                    break;
                case false:
                    useTimestamp = false;
                    overrideTimestampCb.Invoke(new MethodInvoker(delegate { overrideTimestampCb.Enabled = false; }));
                    break;
            }
        }

        public void OverrideTimestampOnUpdate()
        {
            switch (overrideTimestampCb.Checked)
            {
                case true:
                    overrideTimestamp = true;
                    overrideTimeDTP.Invoke(new MethodInvoker(delegate { overrideTimeDTP.Enabled = true; }));
                    break;
                case false:
                    overrideTimestamp = false;
                    overrideTimeDTP.Invoke(new MethodInvoker(delegate { overrideTimeDTP.Enabled = false; }));
                    timeStamp = DateTime.Now;
                    break;
            }
        }

        #region events

        private void OnConnectionEstablished(object sender, ConnectionEstablishedMessage args)
        {
            connectionStatusLabel.Invoke(new MethodInvoker(delegate { connectionStatusLabel.Text = "Connected"; })); // thread safe, idk it throws exceptions otherwise
            stopButton.Invoke(new MethodInvoker(delegate { stopButton.Enabled = true; }));
            startButton.Invoke(new MethodInvoker(delegate { startButton.Enabled = false; }));
            updateButton.Invoke(new MethodInvoker(delegate { updateButton.Enabled = true; }));
            allowInitialize = false;
            Update();
        }

        private void OnConnectionFailed(object sender, ConnectionFailedMessage args)
        {
            MessageBox.Show("DRP could not initialize!", $"Connection Failure. {args.Type}. Pipe: {args.FailedPipe}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        private void OnConnectionClosed(object sender, CloseMessage args)
        {
            connectionStatusLabel.Invoke(new MethodInvoker(delegate { connectionStatusLabel.Text = "Disconnected"; }));
            stopButton.Invoke(new MethodInvoker(delegate { stopButton.Enabled = false; }));
            startButton.Invoke(new MethodInvoker(delegate { startButton.Enabled = true; }));
            updateButton.Invoke(new MethodInvoker(delegate { updateButton.Enabled = false; }));
            OnClose();
            initialized = false;
            allowInitialize = true;
        }

        public void OnFormReady()
        {
            if (info.useArgs)
            {
                Load();
                StartOnClick();
                Update();
            }
        }

        #endregion
    }
}
