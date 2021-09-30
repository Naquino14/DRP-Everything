using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace DRP_Everything
{
    #pragma warning disable SYSLIB0011
    public class DataSerializer
    {
        public SaveData Load(string path)
        {
            SaveData result = new SaveData();
            try
            {
                if (!File.Exists(path))
                {
                    MessageBox.Show("Error!", $"File at path {path} could nto be found or does not exist", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    BinaryFormatter br = new BinaryFormatter();
                    result = (SaveData)br.Deserialize(fs);
                }
                return result;
            } catch (Exception ex)
            {
                MessageBox.Show("Error!", $"Failed to load save data. {ex.ToString()}", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
        public void SaveData(SaveData data, string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    DialogResult result = MessageBox.Show("Warning!", $"File {path} already exists! Would you like to overwrite the file?", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    switch (result)
                    {
                        case DialogResult.OK:
                            using (FileStream fs = new FileStream(path, FileMode.Create))
                                new BinaryFormatter().Serialize(fs, data);
                            break;
                        case DialogResult.No:
                            return;
                        case DialogResult.Cancel:
                            return;
                    }
                    MessageBox.Show("Saved configuration.");
                }
                using (FileStream fs = new FileStream(path, FileMode.CreateNew))
                    new BinaryFormatter().Serialize(fs, data);
                MessageBox.Show("Saved Configuraton.");
            } catch (Exception ex)
            {
                MessageBox.Show("Error!", $"Failed to save save data. {ex.ToString()}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
