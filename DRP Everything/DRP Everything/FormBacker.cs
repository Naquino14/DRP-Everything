using System;
using System.Windows.Forms;

using DiscordRPC;

namespace DRP_Everything
{
    public class FormBacker
    {
        string appId;
        DiscordRpcClient client;
        bool initialized = false;
        public FormBacker()
        {
        }

        public void Initialize(string appId)
        {
            this.appId = appId;
            if (!initialized)
            { 
                client = new DiscordRpcClient(this.appId); 
                client.Initialize(); 
                initialized = true; 
            }
        }

        public void Stop()
        {
            if (initialized)
                client.Dispose();
            else
                MessageBox.Show("Client is not initialized!", "Not Initialized!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
