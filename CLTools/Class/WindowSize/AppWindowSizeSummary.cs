using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace CLTools.Class.WindowSize
{
    internal class AppWindowSizeSummary
    {
        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(
            IntPtr hwnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern int MoveWindow(
            IntPtr hwnd, int x, int y, int nWidth, int nHeight, int bRepaint);

        [DllImport("dwmapi.dll")]
        static extern int DwmGetWindowAttribute(
            IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, out RECT pvAttribute, int cbAttribute);


        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        enum DWMWINDOWATTRIBUTE : uint
        {
            NCRenderingEnabled = 1,
            NCRenderingPolicy,
            TransitionsForceDisabled,
            AllowNCPaint,
            CaptionButtonBounds,
            NonClientRtlLayout,
            ForceIconicRepresentation,
            Flip3DPolicy,
            ExtendedFrameBounds,
            HasIconicBitmap,
            DisallowPeek,
            ExcludedFromPeek,
            Cloak,
            Cloaked,
            FreezeRepresentation
        }

        #region Public Parameter

        public string Name { get; set; }
        public IntPtr WindowHandle { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int sX { get; set; }
        public int sY { get; set; }
        public int sWidth { get; set; }
        public int sHeight { get; set; }
        public bool IsWindowProcess { get; set; }

        #endregion

        public AppWindowSizeSummary() { }
        public AppWindowSizeSummary(Process proc)
        {
            this.Name = proc.ProcessName;
            this.WindowHandle = proc.MainWindowHandle;

            if (this.WindowHandle.ToInt32() != 0)
            {
                this.IsWindowProcess = true;

                DwmGetWindowAttribute(WindowHandle,
                    DWMWINDOWATTRIBUTE.ExtendedFrameBounds,
                    out RECT rect,
                    Marshal.SizeOf(typeof(RECT)));
                this.X = rect.left;
                this.Y = rect.top;
                this.Width = rect.right - rect.left;
                this.Height = rect.bottom - rect.top;

                GetWindowRect(WindowHandle, out RECT sRect);
                this.sX = sRect.left;
                this.sY = sRect.top;
                this.sWidth = sRect.right - sRect.left;
                this.sHeight = sRect.bottom - sRect.top;
            }
        }

        /// <summary>
        /// ウィンドウサイズ変更
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void ChangeWindowSize(int x, int y, int width, int height)
        {
            MoveWindow(this.WindowHandle, x, y, width, height, 1);
        }
    }
}
