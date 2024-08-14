using System.Windows.Forms;

namespace u3WindowsManager
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            /*
             * Note to myself:
             * If calling a command line option will be failing, when using auto exit functionality,
             * consider using:
             * static async Task Main(string[] args)
             */

            // command line args
            bool bAutoExit = false;
            if (args.Length > 0)
            {
                //
            }

            // Run the app
            if (!bAutoExit)
            {
                ApplicationConfiguration.Initialize();
                Application.Run(new TrayApp());
            }
        }
    }
}