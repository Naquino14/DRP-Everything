using System;
using System.Windows.Forms;

using DiscordRPC;
using DiscordRPC.Message;

namespace DRP_Everything
{
    public class FormBacker
    {
        SaveFileDialog sfd;
        OpenFileDialog ofd;

        SaveData saveData;

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

        #region DRP Update Properties

        DateTime timeStamp;

        #endregion

        public FormBacker(
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

            if (useTimestamp)
            {
                if (overrideTimestamp)
                    client.UpdateStartTime(overrideTimeDTP.Value);
                else
                    client.UpdateStartTime(DateTime.Now);
            }
        }

        public void SaveOnClick()
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

            string path;
            sfd.Filter = "DRP Everything Config (*.drpec)|*.drpec";
            if (sfd.ShowDialog() == DialogResult.OK)
            { path = sfd.FileName; new DataSerializer().SaveData(saveData, path); }
        }

        public void LoadOnClick()
        {

        }

        public void CreateShortcutOnClick()
        {

        }

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

        #endregion
    }
}
