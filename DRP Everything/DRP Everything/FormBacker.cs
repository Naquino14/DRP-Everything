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
        System.Windows.Forms.Button updateButton;

        Label connectionStatusLabel;

        public FormBacker(
            TextBox appIdTb,
            System.Windows.Forms.Button startButton,
            Label connectionStatusLabel,
            System.Windows.Forms.Button stopButton,
            System.Windows.Forms.Button updateButton
            ) {
            this.appIdTb = appIdTb;
            this.startButton = startButton;
            this.connectionStatusLabel = connectionStatusLabel;
            this.stopButton = stopButton;
            this.updateButton = updateButton;
        }

        public void StartOnClick()
        {
            if (!allowInitialize)
                return;
            appId = appIdTb.Text;
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
            OnClose();
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
            client.UpdateState("bad software");
            client.UpdateLargeAsset("what");
        }

        public void OnClose()
        {
            client.Dispose();
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
            initialized = false;
            allowInitialize = true;
        }

        #endregion
    }
}
