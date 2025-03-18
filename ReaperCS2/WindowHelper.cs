using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace ReaperCS2
{
    public static class WindowHelper
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static (Vector2 size, Vector2 position) GetCS2WindowBounds()
        {
            Process? cs2 = Process.GetProcessesByName("cs2").FirstOrDefault();
            if (cs2 == null || cs2.MainWindowHandle == IntPtr.Zero)
                return (new Vector2(1920, 1080), new Vector2(0, 0));

            if (GetWindowRect(cs2.MainWindowHandle, out RECT rect))
            {
                Vector2 size = new Vector2(rect.Right - rect.Left, rect.Bottom - rect.Top);
                Vector2 position = new Vector2(rect.Left, rect.Top);
                return (size, position);
            }

            return (new Vector2(1920, 1080), new Vector2(0, 0));
        }
    }

}
