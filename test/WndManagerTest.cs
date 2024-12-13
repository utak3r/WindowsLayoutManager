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
            Mock<WndManager.SystemAPICalls> systemAPICallsMock = new Mock<WndManager.SystemAPICalls>();
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

            Assert.Pass();
        }
    }
}
