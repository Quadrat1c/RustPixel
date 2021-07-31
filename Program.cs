using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Input;

namespace PixelRust
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("user32.dll")]
        static extern Int32 ReleaseDC(IntPtr hwnd, IntPtr hdc);

        [DllImport("gdi32.dll")]
        static extern uint GetPixel(IntPtr hdc, int nXPos, int nYPos);

        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;

        [DllImport("user32.dll")]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern int GetWindowRect(IntPtr hwnd, out Rectangle rect);
        static private Point ConvertToScreenPixel(Point point, IntPtr hwnd)
        {
            Rectangle rect;

            GetWindowRect(hwnd, out rect);

            Point ret = new Point();

            ret.X = rect.Location.X + point.X;
            ret.Y = rect.Location.Y + point.Y;

            return ret;
        }

        static void Main(string[] args)
        {
            IntPtr RustHandle = IntPtr.Zero;
            bool IsRustOpen = false;

            while (!IsRustOpen)
            {
                RustHandle = getRustWinHandle();

                if (RustHandle != IntPtr.Zero)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Found Rust Handle: " + RustHandle.ToString());
                    Console.ForegroundColor = ConsoleColor.White;
                    IsRustOpen = true;
                }
            }

            while (IsRustOpen)
            {
                RustHandle = getRustWinHandle();

                if (RustHandle != IntPtr.Zero)
                {
                    //Bitmap bitmap = new Bitmap(Screen.)

                    Color pixColor = GetPixelColor(RustHandle, 150, 30);
                    Console.WriteLine(RustHandle.ToString());
                    Console.WriteLine("Color @ 150x150: " + pixColor.Name);

                    switch (pixColor.Name)
                    {
                        case "ff404040":    // Intro
                            Console.WriteLine("Intro.");
                            MoveMouseToPosition(RustHandle, 150, 30 + 25);
                            Thread.Sleep(1000);
                            MouseClick();
                            break;
                        case "ff241c10":    // Main Menu
                            Console.WriteLine("Main Menu.");
                            Console.WriteLine("Move Cursor.");
                            MoveMouseToPosition(RustHandle, 450, 300 + 25);
                            Thread.Sleep(50);
                            Console.WriteLine("Mouse Click");
                            MouseClick();
                            Thread.Sleep(2000);
                            break;
                        case "ff1c1410":    // Server Select
                            Console.WriteLine("Server Select");
                            Thread.Sleep(1000);
                            MoveMouseToPosition(RustHandle, 450, 300 + 25);
                            MouseClick();
                            break;
                        default:
                            Console.WriteLine("Color @ 150x150: " + pixColor.Name);
                            break;
                    }
                    //Thread.Sleep(1000);
                    //Console.ReadLine();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Rust has been closed. Reopen Rust.");
                    Console.ForegroundColor = ConsoleColor.White;
                    IsRustOpen = false;
                }
            }

            Console.ReadLine();
        }

        private static void MoveMouseToPosition(IntPtr hwnd, int x, int y)
        {
            Point point = new Point(0, 0);
            Point windowPoint = ConvertToScreenPixel(point, hwnd);
            Console.WriteLine("Ret: " + ConvertToScreenPixel(point, hwnd));
            SetCursorPos(windowPoint.X + x, windowPoint.Y + y);
        }

        private static void MouseClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public static IntPtr getRustWinHandle()
        {
            Process[] processes = Process.GetProcessesByName("RustClient");

            if (processes.Count() > 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: More than 1 process detected.");
                Console.ForegroundColor = ConsoleColor.White;
                return IntPtr.Zero;
            }

            if (processes.Count() == 0)
            {
                Console.WriteLine("Open Rust...");
                Thread.Sleep(2000);
                return IntPtr.Zero;
            }

            if (processes.Count() == 1)
            {
                return processes[0].MainWindowHandle;
            }

            return IntPtr.Zero;
        }

        static public Color GetPixelColor(IntPtr hwnd, int x, int y)
        {
            IntPtr hdc = GetDC(hwnd);
            uint pixel = GetPixel(hdc, x, y);
            ReleaseDC(hwnd, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
                            (int)(pixel & 0x0000FF00) >> 8,
                            (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }
    }
}
