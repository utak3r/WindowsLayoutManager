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

        public interface ISystemAPICalls
        {
            RECT GetAPIWindowRect(IntPtr hWnd);
            WINDOWPLACEMENT GetAPIWindowPlacement(IntPtr hWnd);
            Process[] GetProcesses();
            IntPtr GetProcessMainWindowHandle(Process process);
            string GetProcessMainWindowTitle(Process process);
            string GetProcessName(Process process);
            bool IsProcessResponding(Process process);
            void FileWriteAllText(string dir, string file, string text);
            string FileReadAllText(string dir, string file);
        }

        public class SystemAPICalls : ISystemAPICalls
        {
            public RECT GetAPIWindowRect(IntPtr hWnd)
            {
                RECT rect;
                GetWindowRect(hWnd, out rect);
                return rect;
            }

            public WINDOWPLACEMENT GetAPIWindowPlacement(IntPtr hWnd)
            {
                WINDOWPLACEMENT wndPlacement;
                GetWindowPlacement(hWnd, out wndPlacement);
                return wndPlacement;
            }

            public Process[] GetProcesses()
            {
                return Process.GetProcesses();
            }

            public IntPtr GetProcessMainWindowHandle(Process process)
            {
                return process.MainWindowHandle;
            }

            public string GetProcessMainWindowTitle(Process process)
            {
                return process.MainWindowTitle;
            }

            public string GetProcessName(Process process)
            {
                return process.ProcessName;
            }

            public bool IsProcessResponding(Process process)
            {
                return process.Responding;
            }

            public void FileWriteAllText(string dir, string file, string text)
            {
                string dirpath = Environment.ExpandEnvironmentVariables(dir);
                if (!Directory.Exists(dirpath))
                {
                    Directory.CreateDirectory(dirpath);
                }
                string fullPath = Path.Combine(dirpath, file);
                File.WriteAllText(fullPath, text);
            }

            public string FileReadAllText(string dir, string file)
            {
                string dirpath = Environment.ExpandEnvironmentVariables(dir);
                return File.ReadAllText(Path.Combine(dirpath, file), System.Text.Encoding.UTF8);
            }

        }

        public WndManager()
        {
            //
        }

        public SimpleEntry GetProcessWindowGeometry(Process process, ISystemAPICalls systemAPICalls)
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

        public Dictionary<string, Process> GetAllWindows(ISystemAPICalls systemAPICalls)
        {
            Dictionary<string, Process> wndList = new Dictionary<string, Process>();
            Process[] processes = systemAPICalls.GetProcesses();

            foreach (Process process in processes)
            {
                string winTitle = systemAPICalls.GetProcessMainWindowTitle(process);
                if (!String.IsNullOrEmpty(winTitle))
                {
                    try
                    {
                        string name = systemAPICalls.GetProcessName(process);
                        if (!wndList.ContainsKey(name) &&
                            systemAPICalls.IsProcessResponding(process) &&
                            name != "ApplicationFrameHostx" &&
                            name != "ApplicationFrameHost" &&
                            name != "u3WindowsManager" &&
                            name != "TextInputHost" &&
                            winTitle != ""
                            )
                        {
                            wndList.Add(name, process);
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

        public List<SimpleEntry> SaveDictionary(Dictionary<string, Process> windows, ISystemAPICalls systemAPICalls)
        {
            var list = new List<SimpleEntry>();
            foreach (string key in windows.Keys)
            {
                SimpleEntry entry = GetProcessWindowGeometry(windows[key], systemAPICalls);
                list.Add(entry);
            }
            return list;
        }

        public void SaveAllWindows(string dir, string file, ISystemAPICalls systemAPICalls)
        {
            Dictionary<string, Process> windows = GetAllWindows(systemAPICalls);
            List<SimpleEntry> entries = SaveDictionary(windows, systemAPICalls);
            systemAPICalls.FileWriteAllText(dir, file, JsonSerializer.Serialize(entries));
        }

        public void RestoreAllWindows(string dir, string file, ISystemAPICalls systemAPICalls)
        {
            try
            {
                string jsonfile = systemAPICalls.FileReadAllText(dir, file);
                List<SimpleEntry> entries = JsonSerializer.Deserialize<List<SimpleEntry>>(jsonfile);

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

        public void SelectWindowsAndSave(string dir, string file, ISystemAPICalls systemAPICalls)
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
                systemAPICalls.FileWriteAllText(dir, file, JsonSerializer.Serialize(entries));
            }
        }

    }

}
