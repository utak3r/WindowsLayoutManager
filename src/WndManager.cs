using System.Diagnostics;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Text.Json;
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

        public class SimpleEntry
        {
            public string Name { get; set; }
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
            public int ShowCmd { get; set; }
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
            List<SimpleEntry> entries = new();
            //JsonSerializer.Deserialize()
            //Dictionary<string, Process> windows = GetAllWindows();
            /*
            GetWindowPlacement(hWnd, out placement);
            placement.showCmd = 1
            SetWindowPlacement(hWnd, placement)
            SetForegroundWindow(hWnd)
            placement.rcNormalPosition.Left = entry.Left;
            placement.rcNormalPosition.Top = entry.Top;
            placement.rcNormalPosition.Bottom = entry.Bottom;
            placement.rcNormalPosition.Right = entry.Right;
            placement.showCmd = entry.showCmd;
            SetWindowPlacement(hWnd, placement)
            */
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
