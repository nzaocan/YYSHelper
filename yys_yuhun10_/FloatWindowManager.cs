using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public DispatcherTimer xuanshang = new DispatcherTimer
        {
            Interval = new TimeSpan(0, 0, 0, 8),
        };


        public FloatWindowManager()
        {
            tmer.Tick += Tmer_Tick;
            xuanshang.Tick += Ac_xuanshang_Tick;
        }

        private async void Ac_xuanshang_Tick(object sender, EventArgs e)
        {
            var x = rdom.Next(100);
            await Task.Delay(x * 10);
            click(yysClient.xuanshang);
        }

        Client yysClient
        {
            get { return App.yys_client; }
        }

        public string yys_window_name = "阴阳师-网易游戏";

        IntPtr yys_window;
        HandleRef yys_window_HandleRef;
        int borderWidth;

        bool init()
        {
            yys_window = FindWindow(null, yys_window_name);

            if (yys_window == IntPtr.Zero)
            {
                MessageBox.Show("请先打开阴阳师桌面版");
                return false;
            }
            yys_window_HandleRef = new HandleRef(null, yys_window);

            RECT rct;
            if (!GetWindowRect(yys_window_HandleRef, out rct))
            {
                MessageBox.Show("获取阴阳师窗口大小时出错。");
                return false;
            }

            borderWidth = (int)DisplaySettings.Scaling * 7;

            rct.Left += borderWidth;
            rct.Right -= borderWidth;
            rct.Bottom -= borderWidth;
            yysClient.Init(rct.Width, rct.Height);

            BitmapColor.yys_win = yys_window;
            BitmapColor.width = (int)yysClient.check_size.X;
            BitmapColor.height = (int)yysClient.check_size.Y;

            return true;
        }

        YYSMode currentMode { get; set; }

        public async void testPos()
        {
            yys_window = FindWindow(null, yys_window_name);
            int x = 1780;
            int y = 870 + 120;
            for (int i = 0; i < 10; i++)
            {
                click(new Point(x, y));
                //await Task.Delay(500);
                click(new Point(x, y));
                App.MW.Log += x + "____" + y + '\n';
                //y += 4;
                await Task.Delay(1000);
            }
        }

        bool isstop = false;
        bool isUsingImageCapture = false;
        int RepeatCount = 0;
        public async void Run(int minute, YYSMode mode,bool isUsingImage,bool isacXuanshang)
        {
            var b=init();
            if (!b)
            {
                App.MW.SetStartButtonContentToDefault();
                return;
            } 

            isstop = false;

            currentMode = mode;
            isUsingImageCapture = isUsingImage;
            if ((int)mode > 2)
            {
                if (mode == YYSMode.single_ready)
                {
                    tmer.Interval = new TimeSpan(0, 0, 0, 3, 0);
                }
                else tmer.Interval = new TimeSpan(0, 0, 0, 10, 0);
            }
            else
            {
                tmer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            }

            tmer.Start();
            try
            {
                if (isacXuanshang) xuanshang.Start();
                else xuanshang.Stop();
            }
            catch { }

            //如果使用图像识别，并且刷御魂，则保持阴阳师窗口最前
            if (isUsingImage && (int)mode<=2) 
            {
                SetWindowPos(yys_window, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
            }

            //RepeatCount = 0;

            for (int i = minute; i > 0; i--)
            {
                for (int j = 60; j > 0; j--)
                {
                    if (isstop) return;
                    await Task.Delay(1000);
                    if (isstop) return;
                    App.MW.SetStartButtonContent(i - 1, j - 1, RepeatCount);
                }
            }
            App.MW.SetStartButtonContentToDefault();

            if (App.EndIndex == 0)
                KillProcess("client");
            else if (App.EndIndex == 1)
            {
                Process.Start("shutdown", "/s /t 0");
            }
            Stop();
        }

        private void KillProcess(string processName)
        {
            Process[] myproc = Process.GetProcesses();
            foreach (Process item in myproc)
            {
                if (item.ProcessName == processName)
                {
                    item.Kill();
                }
            }
        }

        public void Stop()
        {
            isstop = true;
            tmer.Stop();
            xuanshang.Stop();

        }


        //40-65,30-50,0-25
        ColorInteval end_color = new ColorInteval(27, 65, 30, 50, 0, 58);
        //210-240,175-215,140-160
        ColorInteval ready_color = new ColorInteval(175, 240, 170, 220, 140, 160);
        //80-100,75-85,75-85
        ColorInteval begin_group_color = new ColorInteval(80, 100, 75, 85, 75, 85);
        bool is_in_end_picture(System.Drawing.Color c)
        {
            return end_color.Contains(c);
        }
        bool is_in_ready_picture(System.Drawing.Color c)
        {
            return ready_color.Contains(c);
        }

        bool is_in_group_begin_picture(System.Drawing.Color c)
        {
            return begin_group_color.Contains(c);
        }

        bool begin_tag = false;
        RECT currentRct;
        int temp_count = 0;

        System.Drawing.Color lastcolor = new System.Drawing.Color();
        int color_count = 0;
        private void Tmer_Tick(object sender, EventArgs e)
        {
            if (currentMode == YYSMode.single_ready)
            {
                click(yysClient.ready);
                return;
            }
            if ((int)currentMode > 2)
            {
                click(yysClient.start_single);
                return;
            }

            int x = currentRct.Left + (int)yysClient.check_point.X;
            int y = currentRct.Top + (int)yysClient.check_point.Y;


            var c = BitmapColor.CalculateCurrentAverageColor(x, y);

            //if (lastcolor.R == c.R && lastcolor.G == c.G && lastcolor.B == c.B)
            //{
            //    color_count++;
            //}
            //else
            //{
            //    color_count = 0;
            //}

            //if (color_count > 10)
            //    Stop();

            //lastcolor = c;



            if (!isUsingImageCapture)
            {
                switch (currentMode)
                {
                    case YYSMode.group_invitee:
                        //click(yysClient.accept);
                        click(yysClient.accept);
                        return;
                    case YYSMode.group_inviter:
                        //click(yysClient.invite);
                        click(yysClient.start_group);
                        click(yysClient.ready);
                        return;
                    case YYSMode.single:
                        click(yysClient.start_single);
                        click(yysClient.ready);
                        return;
                }
                return;
            }

            if (GetWindowRect(yys_window_HandleRef, out currentRct))
            {
                currentRct.Left += borderWidth;
            }




            App.MW.Log += c.ToString() + "\n";

            if (begin_tag)
            {
                temp_count++;
                if (temp_count == 1) RepeatCount++;

                var b = is_in_ready_picture(c);

                if (b || temp_count > 40)
                {
                    begin_tag = false;
                    temp_count = 0;
                }
            }


            if (currentMode == YYSMode.group_inviter)
            {
                var b = is_in_group_begin_picture(c);
                if (b)
                {
                    click(yysClient.start_group);
                    //App.MW.Log += "点击【开始战斗】\n";
                    return;
                }

                var b2 = is_in_end_picture(c);
                if (b2)
                {
                    click(yysClient.ready);
                    begin_tag = true;
                    return;
                }
                return;
            }

            if (currentMode == YYSMode.group_invitee)
            {
                var b2 = is_in_end_picture(c);
                if (b2)
                {
                    begin_tag = true;
                    click(yysClient.ready);
                    return;
                }
            }

            if (currentMode == YYSMode.single)
            {
                var b = is_in_end_picture(c);
                if (b)
                {
                    click(yysClient.ready);
                    begin_tag = true;
                    return;
                }

                if (temp_count > 0)
                    click(yysClient.start_single);
            }

            //{
            //    var b = is_in_ready_picture(c);
            //    if (b)
            //    {
            //        begin_tag = false;
            //        click(yysClient.ready);
            //        App.MW.Log += "点击【准备】\n";
            //        return;
            //    }
            //}




            //if (begin_tag == false)
            //    begin_tag = is_in_begin_picture(c);

            //if (begin_tag)
            //{
            //    if (currentMode == YYSMode.single)
            //    {
            //        click(yysClient.start_single);
            //        App.MW.Log += "点击【挑战】\n";
            //    }
            //    else if (currentMode == YYSMode.group_inviter)
            //    {
            //        return;
            //        click(yysClient.invite);
            //        App.MW.Log += "点击【邀请队友】\n";
            //    }
            //    else if (currentMode == YYSMode.group_invitee)
            //    {
            //        click(yysClient.accept);
            //        App.MW.Log += "点击【接受邀请】\n";
            //    }
            //    return;
            //}

            App.MW.Log += "Click None\n";

        }


        Random rdom = new Random();
        async void click(Point p)
        {
            App.MW.Log += string.Format("Click x={0},y={1}\n", (int)p.X, (int)p.Y);

            var dis1 = rdom.Next(31);
            dis1 -= 30;
            p.X += dis1;


            var dis2 = rdom.Next(31);
            dis2 -= 30;
            p.Y += dis2;

            App.MW.Log += string.Format("Click dis1={0},dis2={1}\n", dis1, dis2);


            IntPtr lParam = MakeLParam((int)p.X, (int)p.Y);

            App.MW.Log += string.Format("Click x={0},y={1}\n", (int)p.X, (int)p.Y);

            var x = rdom.Next(100);

            await Task.Delay(x);

            PostMessage(yys_window, WM_LBUTTONDOWN, IntPtr.Zero, lParam);

            await Task.Delay(20);

            PostMessage(yys_window, WM_LBUTTONUP, IntPtr.Zero, lParam);
        }

        private static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }




        public static Point MousePos
        {
            get
            {
                var pos = System.Windows.Forms.Control.MousePosition;
                return new Point(pos.X, pos.Y);
            }
        }

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);


        [DllImport("user32.dll")]
        public static extern int PostMessage(IntPtr hWnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(HandleRef hWnd, out RECT lpRect);


        static int WM_LBUTTONDOWN = 0x0201;
        static int WM_LBUTTONUP = 0x0202;




    }











    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;        // x position of upper-left corner
        public int Top;         // y position of upper-left corner
        public int Right;       // x position of lower-right corner
        public int Bottom;      // y position of lower-right corner

        public int Height
        {
            get { return Bottom - Top; }
        }
        public int Width
        {
            get { return Right - Left; }
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
