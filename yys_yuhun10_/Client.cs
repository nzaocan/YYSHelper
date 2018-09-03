using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace yys_yuhun10_
{
    public class Client
    {
        public Point start_single_src = new Point(1800, 930);
        public Point ready_src = new Point(2130, 1060);
        public Point start_group_src = new Point(1944, 1123);
        public Point invite_src = new Point(1420, 810);
        public Point accept_src = new Point(270, 480);
        public Point size_src = new Point(2394, 1349);
        //src ready center position
        public Point check_point_src = new Point(2187, 1032);
        public Point check_size_src = new Point(200, 200);
        //780
        public Point xuanshang_src = new Point(1700, 950);

        public Point start_single { get; set; }
        public Point ready { get; set; }
        public Point start_group { get; set; }
        public Point invite { get; set; }
        public Point accept { get; set; }
        public Point size { get; set; }
        public Point check_size { get; set; }
        public Point check_point { get; set; }
        public Point xuanshang { get; set; }

        public static int top_height = 30 * (int)DisplaySettings.Scaling; 

        Point newPoint(Point std)
        {
            var x = (size.X / size_src.X) * std.X;
            var y = (size.Y / size_src.Y) * std.Y;
            Point p = new Point(x, y);
            return p;
        }

        public void Init(double width, double height)
        {
            size = new Point(width, height - 60);
            start_single = newPoint(start_single_src);
            ready = newPoint(ready_src);
            check_point = newPoint(check_point_src);
            start_group = newPoint(start_group_src);
            invite = newPoint(invite_src);
            accept = newPoint(accept_src);
            xuanshang = newPoint(xuanshang_src);
            var x =(size.Y / size_src.Y) * 200;
            check_size = new Point(x,x);
        }
    }


    public enum YYSMode
    {
        single=0,
        group_inviter=1,
        group_invitee=2,
        single_yyh=3,
        single_yl=4,
        single_ready=5
    };

}
