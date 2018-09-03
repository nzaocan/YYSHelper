using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace yys_yuhun10_
{
    public class FloatWindowManager
    {
        public DispatcherTimer tmer = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 0, 0, 500),

        };
        public FloatWindowManager()
        {
            tmer.Tick += Tmer_Tick;
        }


        public FloatWindow begin_win { get; set; }
        public FloatWindow ready_win { get; set; }
        public FloatWindow color_win { get; set; }
        Point beginPoint, readyPoint, colorPoint;
        double win_width, win_height;

        Point setPoint(FloatWindow f, Point p)
        {
            p.X = (f.Left + win_width) * DisplaySettings.Scaling;
            p.Y = (f.Top + win_height) * DisplaySettings.Scaling;
            return p;
        }

        public string yys_window_name = "阴阳师-网易游戏";

        IntPtr yys_window;
        void init()
        {
            win_width = begin_win.Width / 2;
            win_height = begin_win.Height / 2;
            beginPoint = setPoint(begin_win, beginPoint);
            readyPoint = setPoint(ready_win, readyPoint);
            colorPoint = setPoint(color_win, colorPoint);

            ready_win.Hide();
            begin_win.Hide();
            ImageCompare.ready_x = (int)readyPoint.X;
            ImageCompare.ready_y = (int)readyPoint.Y;

            yys_window = FindWindow(null, yys_window_name);
        }

        public async void Begin()
        {
            init();
            tmer.Start();
            await Task.Delay(30 * 60 * 1000);
            Stop();
        }
        public void Stop()
        {
            tmer.Stop();
        }


        ColorInteval begin_color = new ColorInteval(50, 70, 40, 50, 10, 20);
        ColorInteval ready_color = new ColorInteval(175, 240, 140, 180, 100, 130);

        bool is_in_begin_picture()
        {
            var c= ImageCompare.CalculateCurrentAverageColor();
            App.MW.Log += c.ToString() + "\n";
            return begin_color.Contains(c);
        }
        bool is_in_ready_picture()
        {
            var c = ImageCompare.CalculateCurrentAverageColor();
            //App.MW.Log += c.ToString()+ "ready\n";

            return ready_color.Contains(c);
        }

        void get_color()
        {
            int x = (int)readyPoint.X;
            int y = (int)readyPoint.Y;

            int r = 0, g = 0, b = 0;
            int n = 0;

            App.MW.textBox.Text += "开始计算：";
            double zoom = DisplaySettings.Scaling;


            var c=ImageCompare.CalculateCurrentAverageColor();

            r = c.R;
            g = c.G;
            b = c.B;

            App.MW.textBox.Text += "R" + r + ",G" + g + ",B" + b + "\n";

        }

        bool begin_tag = true, ready_tag = false;

        private void Tmer_Tick(object sender, EventArgs e)
        {
            {
                var b = is_in_ready_picture();
                if (b)
                {
                    begin_tag = false;
                    click(ready_win, readyPoint);
                    App.MW.Log += "Click Ready\n";
                    return;
                }
            }

            if(begin_tag==false)
                begin_tag = is_in_begin_picture();

            if (begin_tag)
            {
                click(begin_win, beginPoint);
                App.MW.Log += "Click Begin\n";

                return;
            }

            App.MW.Log += "Click None\n";


        }
















        void click(FloatWindow f, Point p)
        {
            int x = 1280;
            int y = 377;
            IntPtr lParam = MakeLParam(x, y);
            PostMessage(yys_window, WM_LBUTTONDOWN, IntPtr.Zero, lParam);
            PostMessage(yys_window, WM_LBUTTONUP, IntPtr.Zero, lParam);



            //SetCursorPos((int)p.X, (int)p.Y);
            //MouseSimulator.ClickLeftMouseButton();
            //PerformLeftKlick((int)p.X, (int)p.Y);
            //LeftMouseClick((int)p.X, (int)p.Y);
            //await Task.Delay(100);
        }


        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }




        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern long mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 cButtons, Int32 dwExtraInfo);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern void SetCursorPos(Int32 x, Int32 y);

        public const Int32 MOUSEEVENTF_ABSOLUTE = 0x8000;
        public const Int32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        public const Int32 MOUSEEVENTF_LEFTUP = 0x0004;
        public const Int32 MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        public const Int32 MOUSEEVENTF_MIDDLEUP = 0x0040;
        public const Int32 MOUSEEVENTF_MOVE = 0x0001;
        public const Int32 MOUSEEVENTF_RIGHTDOWN = 0x0008;
        public const Int32 MOUSEEVENTF_RIGHTUP = 0x0010;

        public void PerformLeftKlick(Int32 x, Int32 y)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }







        //This simulates a left mouse click
        public static async void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            await Task.Delay(20);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        public static Point MousePos
        {
            get
            {
                var pos = System.Windows.Forms.Control.MousePosition;
                return new Point(pos.X, pos.Y);
            }
        }


        [DllImport("user32.dll")]
        public static extern int PostMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        static int WM_LBUTTONDOWN = 0x0201;
        static int WM_LBUTTONUP = 0x0202;

        private static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }

        public static void test()
        {
            IntPtr windowPtr = FindWindow( null,"阴阳师-网易游戏");


            RECT rct;

            if (!GetWindowRect(new HandleRef(null, windowPtr), out rct))
            {
                MessageBox.Show("ERROR");
                return;
            }
            MessageBox.Show(rct.ToString());


            int x = 1280;
            int y = 377;
            IntPtr lParam = MakeLParam(x, y);
            PostMessage(windowPtr, WM_LBUTTONDOWN, IntPtr.Zero, lParam);
            PostMessage(windowPtr, WM_LBUTTONUP, IntPtr.Zero, lParam);

            //var point = new System.Drawing.Point(x, y);

            //ClickOnPointTool.ClickOnPoint(windowPtr, point);

        }



    }











    public static class DisplaySettings
    {
        static double scal = 0;
        public static double Scaling
        {
            get
            {
                if (scal == 0)
                    scal = getScalingFactor();

                return scal;
            }
        }














        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
        }

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        private static double getScalingFactor()
        {
            var g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();

            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            var ScreenScalingFactor = (double)PhysicalScreenHeight / (double)SystemParameters.PrimaryScreenHeight;

            return ScreenScalingFactor;
        }


       
    }

    public class ColorInteval
    {
        public int R1 { get; set; }
        public int R2 { get; set; }

        public int G1 { get; set; }
        public int G2 { get; set; }

        public int B1 { get; set; }
        public int B2{ get; set; }

        public ColorInteval(int r1, int r2, int g1, int g2, int b1, int b2)
        {
            R1 = r1;
            R2 = r2;
            B1 = b1;
            B2 = b2;
            G1 = g1;
            G2 = g2;
        }

        public bool Contains(System.Drawing.Color c)
        {
            if (c.R >= R1 && c.R <= R2)
            {
                if (c.G >= G1 && c.G <= G2)
                {
                    if (c.B >= B1 && c.B <= B2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
