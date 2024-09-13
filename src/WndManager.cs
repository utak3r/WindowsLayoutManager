using System.Diagnostics;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using u3WindowsManager;

namespace u3WindowsManager
{

    public class WndManager
    {

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;                 // The current show state of the window, like SW_SHOW, SW_MINIMIZE, SW_SHOWMINIMIZED etc.
            public POINT ptMinPosition;         // The coordinates of the window's upper-left corner when the window is minimized.
            public POINT ptMaxPosition;         // The coordinates of the window's upper-left corner when the window is maximized.
            public RECT rcNormalPosition;       // The window's coordinates when the window is in the restored position.
            public RECT rcDevice;
        }

        [DllImport("user32.dll")]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpPlacement);

        [DllImport("user32.dll")]
        static extern bool SetWindowPlacement(IntPtr hWnd, WINDOWPLACEMENT lpPlacement);

        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public class SimpleEntry
        {
            public string Name { get; set; }
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
            public int ShowCmd { get; set; }

            public bool Equals(SimpleEntry other)
            {
                if (other == null) return false;
                return Name.Equals(other.Name);
            }

            public bool Equals(string name)
            {
                if (name == null) return false;
                return Name.Equals(name);
            }
        }


        public WndManager()
        {
            //
        }

        public void SaveAllWindows()
        {
            List<SimpleEntry> entries = new();
            Dictionary<string, Process> windows = GetAllWindows();
            foreach (string key in windows.Keys)
            {
                Process process = windows[key];
                IntPtr hWnd = process.MainWindowHandle;
                WINDOWPLACEMENT wndPlacement;
                RECT rect;

                GetWindowRect(hWnd, out rect);
                GetWindowPlacement(hWnd, out wndPlacement);
                SimpleEntry entry = new()
                {
                    Name = key,
                    Left = rect.Left,
                    Top = rect.Top,
                    Right = rect.Right,
                    Bottom = rect.Bottom,
                    ShowCmd = wndPlacement.showCmd
                };
                entries.Add(entry);
            }
            File.WriteAllText("testentries.json", JsonSerializer.Serialize(entries));
        }

        public void RestoreAllWindows()
        {
            string file = File.ReadAllText("testentries.json", System.Text.Encoding.UTF8);
            List<SimpleEntry> entries = JsonSerializer.Deserialize<List<SimpleEntry>>(file);

            Dictionary<string, Process> windows = GetAllWindows();
            foreach (string key in windows.Keys)
            {
                if (entries.Exists(x => x.Name == key))
                {
                    // The below IF is just for DEBUG!!
                    // if (key == "TOTALCMD64")
                    {
                        SimpleEntry entry = entries.Find(x => x.Name == key);
                        Process process = windows[key];
                        IntPtr hWnd = process.MainWindowHandle;
                        WINDOWPLACEMENT wndPlacement;
                        RECT rect;

                        GetWindowPlacement(hWnd, out wndPlacement);
                        wndPlacement.showCmd = 1;
                        SetWindowPlacement(hWnd, wndPlacement);
                        SetForegroundWindow(hWnd);
                        wndPlacement.rcNormalPosition.Left = entry.Left;
                        wndPlacement.rcNormalPosition.Top = entry.Top;
                        wndPlacement.rcNormalPosition.Bottom = entry.Bottom;
                        wndPlacement.rcNormalPosition.Right = entry.Right;
                        wndPlacement.showCmd = entry.ShowCmd;
                        SetWindowPlacement(hWnd, wndPlacement);
                    }
                }
            }

        }

        private Dictionary<string, Process> GetAllWindows()
        {
            Dictionary<string, Process> wndList = new Dictionary<string, Process>();
            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    try
                    {
                        if (!wndList.ContainsKey(process.ProcessName) && 
                            process.Responding && 
                            process.ProcessName != "ApplicationFrameHostx" &&
                            process.ProcessName != "u3WindowsManager" &&
                            process.MainWindowTitle != ""
                            )
                        {
                            wndList.Add(process.ProcessName, process);
                        }
                    }
                    catch
                    {
                        //
                    }
                }
            }

            return wndList;
        }

    }

}
