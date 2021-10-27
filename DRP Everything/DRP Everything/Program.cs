using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DRP_Everything
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args) // args[0] is config, args[1] is app path
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 0)
            {
                InitializationInformation info = new InitializationInformation();
                info.useArgs = false;

                MainForm form = new MainForm(info);
                form.AutoSize = false;
                //form.ShowInTaskbar = false;
                Application.Run(form);
            } else if (args.Length == 1)
            {
                InitializationInformation info = new InitializationInformation();
                info.useArgs = true;
                info.configPath = args[0];
                info.configOnly = true;

                MainForm form = new MainForm(info);
                form.AutoSize = false;
                form.WindowState = FormWindowState.Minimized;
                Application.Run(form);
            }
            else
            {
                InitializationInformation info = new InitializationInformation();
                info.useArgs = true;
                info.configPath = args[0];
                info.executablePath = args[1];

                MainForm form = new MainForm(info);
                form.AutoSize = false;
                //form.ShowInTaskbar = false;
                form.WindowState = FormWindowState.Minimized;
                Application.Run(form);
            }
            
        }
    }
}
