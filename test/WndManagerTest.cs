using u3WindowsManager;
using NUnit.Framework;
using Moq;
using System.Diagnostics;
using static u3WindowsManager.WndManager;

namespace u3WindowsManagerTests
{
    [TestFixture]
    public class WndManagerTest
    {

        [SetUp]
        public void SetUp()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void WndManagerTest_GetProcessWindowGeometry()
        {
            Mock<WndManager.ISystemAPICalls> systemAPICallsMock = new Mock<WndManager.ISystemAPICalls>();
            systemAPICallsMock.Setup(r => r.GetAPIWindowRect(It.IsAny<nint>()))
                .Returns(new WndManager.RECT
                {
                    Left = 0, Top = 0, Right = 600, Bottom = 400
                });
            systemAPICallsMock.Setup(p => p.GetAPIWindowPlacement(It.IsAny<nint>()))
                .Returns(new WndManager.WINDOWPLACEMENT
                {
                    length = 0, flags = 0, showCmd = 0, 
                    ptMinPosition = new WndManager.POINT { x = 0, y = 0 }, 
                    ptMaxPosition = new WndManager.POINT { y = 0, x = 0 },
                    rcNormalPosition = new WndManager.RECT { Left = 0, Top = 0, Right = 600, Bottom = 400 },
                    rcDevice = new WndManager.RECT { Left = 0, Top = 0, Right = 600, Bottom = 400 }
                });
            systemAPICallsMock.Setup(h => h.GetProcessMainWindowHandle(It.IsAny<Process>()))
                .Returns((IntPtr)1234);
            systemAPICallsMock.Setup(n => n.GetProcessName(It.IsAny<Process>()))
                .Returns("TestingProcessName");

            Process process = new();
            WndManager wndManager = new();
            SimpleEntry entry = wndManager.GetProcessWindowGeometry(process, systemAPICallsMock.Object);

            Assert.That(entry, Is.Not.Null);
            Assert.That(entry.Name, Is.EqualTo("TestingProcessName"));
            Assert.That(entry.Left, Is.EqualTo(0));
            Assert.That(entry.Top, Is.EqualTo(0));
            Assert.That(entry.Right, Is.EqualTo(600));
            Assert.That(entry.Bottom, Is.EqualTo(400));
            Assert.That(entry.ShowCmd, Is.EqualTo(0));
        }

        [Test]
        public void WndManagerTest_SaveDictionary()
        {
            Mock<WndManager.ISystemAPICalls> systemAPICallsMock = new Mock<WndManager.ISystemAPICalls>();
            systemAPICallsMock.Setup(r => r.GetAPIWindowRect(It.IsAny<nint>()))
                .Returns(new WndManager.RECT
                {
                    Left = 0,
                    Top = 0,
                    Right = 600,
                    Bottom = 400
                });
            systemAPICallsMock.Setup(p => p.GetAPIWindowPlacement(It.IsAny<nint>()))
                .Returns(new WndManager.WINDOWPLACEMENT
                {
                    length = 0,
                    flags = 0,
                    showCmd = 0,
                    ptMinPosition = new WndManager.POINT { x = 0, y = 0 },
                    ptMaxPosition = new WndManager.POINT { y = 0, x = 0 },
                    rcNormalPosition = new WndManager.RECT { Left = 0, Top = 0, Right = 600, Bottom = 400 },
                    rcDevice = new WndManager.RECT { Left = 0, Top = 0, Right = 600, Bottom = 400 }
                });
            systemAPICallsMock.Setup(h => h.GetProcessMainWindowHandle(It.IsAny<Process>()))
                .Returns((IntPtr)1234);
            systemAPICallsMock.Setup(n => n.GetProcessName(It.IsAny<Process>()))
                .Returns("TestingProcessName");

            Dictionary<string, Process> windows = new();
            windows.Add("window_one", new Process());
            windows.Add("window_two", new Process());
            windows.Add("window_three", new Process());
            WndManager wndManager = new();

            List<SimpleEntry> wndlist = wndManager.SaveDictionary(windows, systemAPICallsMock.Object);

            Assert.That(wndlist.Count, Is.EqualTo(3));
        }

        [Test]
        public void WndManagerTest_GetAllWindows()
        {
            var processNames = new Queue<string>();
            processNames.Enqueue("Fantastic process");
            processNames.Enqueue("Great process");
            processNames.Enqueue("ApplicationFrameHost");
            processNames.Enqueue("Nice process");
            processNames.Enqueue("ApplicationFrameHostx");
            processNames.Enqueue("Good process");
            processNames.Enqueue("Ok process");
            processNames.Enqueue("u3WindowsManager");
            processNames.Enqueue("Yet Another Process");
            processNames.Enqueue("Another process");
            processNames.Enqueue("TextInputHost");
            processNames.Enqueue("Boring stuff");

            Mock<WndManager.ISystemAPICalls> systemAPICallsMock = new Mock<WndManager.ISystemAPICalls>();
            systemAPICallsMock.Setup(p => p.GetProcesses()).Returns(new Process[12]);
            systemAPICallsMock.Setup(w => w.GetProcessMainWindowTitle(It.IsAny<Process>())).Returns("Nice main window title");
            systemAPICallsMock.Setup(n => n.GetProcessName(It.IsAny<Process>())).Returns(processNames.Dequeue);
            systemAPICallsMock.Setup(r => r.IsProcessResponding(It.IsAny<Process>())).Returns(true);

            WndManager wndManager = new();
            Dictionary<string, Process> windows = wndManager.GetAllWindows(systemAPICallsMock.Object);

            // we had 12 processes, but 4 of them have illegal names and shouldn't be added to the list.
            Assert.That(windows.Count, Is.EqualTo(8));
            Assert.That(windows.ContainsKey("ApplicationFrameHostx"), Is.False);
            Assert.That(windows.ContainsKey("ApplicationFrameHost"), Is.False);
            Assert.That(windows.ContainsKey("TextInputHost"), Is.False);
            Assert.That(windows.ContainsKey("u3WindowsManager"), Is.False);
        }

        [Test]
        public void WndManagerTest_SaveAllWindows()
        {
            var processNames = new Queue<string>();
            // GetProcessName will be called twice
            processNames.Enqueue("Test Process 1");
            processNames.Enqueue("Test Process 2");
            processNames.Enqueue("Test Process 3");
            processNames.Enqueue("Test Process 1");
            processNames.Enqueue("Test Process 2");
            processNames.Enqueue("Test Process 3");

            Mock<WndManager.ISystemAPICalls> systemAPICallsMock = new Mock<WndManager.ISystemAPICalls>();
            systemAPICallsMock.Setup(p => p.GetProcesses()).Returns(new Process[3]);
            systemAPICallsMock.Setup(w => w.GetProcessMainWindowTitle(It.IsAny<Process>())).Returns("Nice main window title");
            systemAPICallsMock.Setup(n => n.GetProcessName(It.IsAny<Process>())).Returns(processNames.Dequeue);
            systemAPICallsMock.Setup(r => r.IsProcessResponding(It.IsAny<Process>())).Returns(true);
            systemAPICallsMock.Setup(h => h.GetProcessMainWindowHandle(It.IsAny<Process>())).Returns((IntPtr)1234);
            systemAPICallsMock.Setup(c => c.GetAPIWindowRect(It.IsAny<nint>()))
                .Returns(new WndManager.RECT
                {
                    Left = 0,
                    Top = 0,
                    Right = 600,
                    Bottom = 400
                });
            systemAPICallsMock.Setup(m => m.GetAPIWindowPlacement(It.IsAny<nint>()))
                .Returns(new WndManager.WINDOWPLACEMENT
                {
                    length = 0,
                    flags = 0,
                    showCmd = 0,
                    ptMinPosition = new WndManager.POINT { x = 0, y = 0 },
                    ptMaxPosition = new WndManager.POINT { y = 0, x = 0 },
                    rcNormalPosition = new WndManager.RECT { Left = 0, Top = 0, Right = 600, Bottom = 400 },
                    rcDevice = new WndManager.RECT { Left = 0, Top = 0, Right = 600, Bottom = 400 }
                });

            string test_dir = "";
            string test_filename = "";
            string test_text = "";
            systemAPICallsMock
                .Setup(f => f.FileWriteAllText(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Callback<string, string, string>((cdir, cfile, ctext) => { test_dir = cdir; test_filename = cfile; test_text = ctext; });

            WndManager wndManager = new();
            string appDataDir = @"c:\Programs\MyApp";
            string appDataFile = "DataFile.dat";
            wndManager.SaveAllWindows(appDataDir, appDataFile, systemAPICallsMock.Object);

            Assert.That(test_dir, Is.EqualTo(appDataDir));
            Assert.That(test_filename, Is.EqualTo(appDataFile));
            Assert.That(test_text, Is.EqualTo("[{\"Name\":\"Test Process 1\",\"Left\":0,\"Top\":0,\"Right\":600,\"Bottom\":400,\"ShowCmd\":0},{\"Name\":\"Test Process 2\",\"Left\":0,\"Top\":0,\"Right\":600,\"Bottom\":400,\"ShowCmd\":0},{\"Name\":\"Test Process 3\",\"Left\":0,\"Top\":0,\"Right\":600,\"Bottom\":400,\"ShowCmd\":0}]"));

        }

    }
}
