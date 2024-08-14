using System.Runtime.Versioning;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Resources;

namespace u3WindowsManager
{
    internal class TrayApp : ApplicationContext
    {
        NotifyIcon? theTray = null;

        public TrayApp()
        {
            // version info
            Version version = new();
            string versionString = version.GetFullVersion();

            // context menu
            ToolStripMenuItem mnuExit = new ToolStripMenuItem("Exit");
            mnuExit.Click += MnuExit_Click;
            ContextMenuStrip mnuMainMenu = new ContextMenuStrip();
            mnuMainMenu.Items.AddRange([mnuExit]);

            // tray icon
            theTray = new NotifyIcon();
            theTray.Icon = LoadIconFromResources("u3WindowsManager.TrayAppIcon.ico");
            theTray.ContextMenuStrip = mnuMainMenu;
            theTray.Text = "u3WindowsManager";

            theTray.Visible = true;

            // this is JUST FOR TESTS!
            WndManager wndManager = new WndManager();
            wndManager.SaveAllWindows();
        }

        private Icon? LoadIconFromResources(string resId)
        {
            Icon? icon = null;
            Assembly ass = Assembly.GetExecutingAssembly();
            if ( ass != null)
            {
                //string[] names = ass.GetManifestResourceNames();
                Stream? stream = ass.GetManifestResourceStream(resId);
                if (stream != null)
                    icon = new(stream);
            }
            return icon;
        }

        private void MnuExit_Click(object? sender, EventArgs e)
        {
            if (theTray != null)
            {
                theTray.Visible = false;
            }
            ExitThread();
        }


    }
}
