using System;
namespace DRP_Everything
{
    [Serializable]
    public class SaveData
    {
        public string appId;
        public DateTime timeStamp;
        public bool useTimestamp;
        public bool overrideTimestamp;

        public string detail;
        public string state;

        public string largeImageText;
        public string largeImageKey;
        public string smallImageText;
        public string smallImageKey;
    }
}
