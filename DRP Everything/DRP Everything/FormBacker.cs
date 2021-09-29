using System;
using System.Windows.Forms;

using DiscordRPC;
using DiscordRPC.Message;

namespace DRP_Everything
{
    public class FormBacker
    {
        string appId;
        DiscordRpcClient client;
        bool allowInitialize = false;
        bool initialized = false;

        TextBox appIdTb;
        System.Windows.Forms.Button startButton;
        System.Windows.Forms.Button stopButton;
        Label connectionStatusLabel;

        public FormBacker(
            TextBox appIdTb,
            System.Windows.Forms.Button startButton,
            Label connectionStatusLabel,
            System.Windows.Forms.Button stopButton
            ) {
            this.appIdTb = appIdTb;
            this.startButton = startButton;
            this.connectionStatusLabel = connectionStatusLabel;
            this.stopButton = stopButton;
        }

        public void Start()
        {
            if (!allowInitialize)
                return;
            appId = appIdTb.Text;
            if (!initialized)
            { 
                client = new DiscordRpcClient(appId); 
                client.Initialize(); 
                initialized = true;
                client.OnConnectionEstablished += OnConnectionEstablished;
            }
        }

        public void Stop()
        {
            if (initialized)
                client.Dispose();
            else
                MessageBox.Show("Client is not initialized!", "Not Initialized!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        private void OnConnectionEstablished(object sender, ConnectionEstablishedMessage args)
        {
            connectionStatusLabel.Invoke(new MethodInvoker(delegate { connectionStatusLabel.Text = "Connected"; }));
            stopButton.Invoke(new MethodInvoker(delegate { stopButton.Enabled = true; }));
            Update();
        }

        private void OnConnectionFailed()
        {
            MessageBox.Show("Client could not initialize!", "Connection Failure.", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }

        public void Update()
        {
            client.UpdateState("test");
            client.UpdateLargeAsset("tst");
        }

        public void OnClose()
        {
            client.Dispose();
        }
    }
}
