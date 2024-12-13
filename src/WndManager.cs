using System.Diagnostics;
using System.DirectoryServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Windows.Forms;
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

        public class SystemAPICalls
        {
            public virtual RECT GetAPIWindowRect(IntPtr hWnd)
            {
                RECT rect;
                GetWindowRect(hWnd, out rect);
                return rect;
            }

            public virtual WINDOWPLACEMENT GetAPIWindowPlacement(IntPtr hWnd)
            {
                WINDOWPLACEMENT wndPlacement;
                GetWindowPlacement(hWnd, out wndPlacement);
                return wndPlacement;
            }

            public virtual Process[] GetProcesses()
            {
                return Process.GetProcesses();
            }

            public virtual IntPtr GetProcessMainWindowHandle(Process process)
            {
                return process.MainWindowHandle;
            }

            public virtual string GetProcessName(Process process)
            {
                return process.ProcessName;
            }

        }

        public class ConfigFileOperations
        {
            public static void SaveEntriesToFile(string filename, List<SimpleEntry> entries)
            {
                File.WriteAllText(filename, JsonSerializer.Serialize(entries));
            }

            public static List<SimpleEntry> ReadEntriesFromFile(string filename)
            {
                string file = File.ReadAllText(filename, System.Text.Encoding.UTF8);
                List<SimpleEntry> entries = JsonSerializer.Deserialize<List<SimpleEntry>>(file);
                return entries;
            }
        }

        public WndManager()
        {
            //
        }

        public SimpleEntry GetProcessWindowGeometry(Process process, SystemAPICalls systemAPICalls)
        {
            IntPtr hWnd = systemAPICalls.GetProcessMainWindowHandle(process);
            RECT rect = systemAPICalls.GetAPIWindowRect(hWnd);
            WINDOWPLACEMENT wndPlacement = systemAPICalls.GetAPIWindowPlacement(hWnd);
            SimpleEntry entry = new()
            {
                Name = systemAPICalls.GetProcessName(process),
                Left = rect.Left,
                Top = rect.Top,
                Right = rect.Right,
                Bottom = rect.Bottom,
                ShowCmd = wndPlacement.showCmd
            };
            return entry;
        }

        private Dictionary<string, Process> GetAllWindows(SystemAPICalls systemAPICalls)
        {
            Dictionary<string, Process> wndList = new Dictionary<string, Process>();
            Process[] processes = systemAPICalls.GetProcesses();

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


        public List<SimpleEntry> SaveDictionary(Dictionary<string, Process> windows, SystemAPICalls systemAPICalls)
        {
            var list = new List<SimpleEntry>();
            foreach (string key in windows.Keys)
            {
                SimpleEntry entry = GetProcessWindowGeometry(windows[key], systemAPICalls);
                list.Add(entry);
            }
            return list;
        }

        public void SaveAllWindows(string fileName, SystemAPICalls systemAPICalls)
        {
            Dictionary<string, Process> windows = GetAllWindows(systemAPICalls);
            List<SimpleEntry> entries = SaveDictionary(windows, systemAPICalls);
            File.WriteAllText(fileName, JsonSerializer.Serialize(entries));
        }

        public void RestoreAllWindows(string fileName, SystemAPICalls systemAPICalls)
        {
            try
            {
                string file = File.ReadAllText(fileName, System.Text.Encoding.UTF8);
                List<SimpleEntry> entries = JsonSerializer.Deserialize<List<SimpleEntry>>(file);

                Dictionary<string, Process> windows = GetAllWindows(systemAPICalls);
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
            catch (Exception ex) when (ex is ArgumentNullException ||
                           ex is NotSupportedException ||
                           ex is FileNotFoundException ||
                           ex is JsonException)
            {
                //
            }
        }

        public void SelectWindowsAndSave(string fileName, SystemAPICalls systemAPICalls)
        {
            WindowsSelectionList theWindowsSelect = new WindowsSelectionList();
            Dictionary<string, Process> windows = GetAllWindows(systemAPICalls);
            Dictionary<string, Process> selectedWindows = new Dictionary<string, Process>();
            foreach (string key in windows.Keys)
            {
                theWindowsSelect.AddItem(key);
            }
            theWindowsSelect.ShowDialog();
            if (theWindowsSelect.DialogResult == DialogResult.OK)
            {
                List<string> selected = theWindowsSelect.SelectedItems();
                foreach (string selected_item in selected)
                {
                    if (windows.TryGetValue(selected_item, out Process process))
                        selectedWindows.Add(selected_item, process);
                }
                List<SimpleEntry> entries = SaveDictionary(selectedWindows, systemAPICalls);
                File.WriteAllText(fileName, JsonSerializer.Serialize(entries));
            }
        }

    }

}
