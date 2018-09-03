using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace yys_yuhun10_
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow: INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            App.MW = this;
            this.DataContext = this;
            IsUsingImageCapture = 0;
            IsAutoAcceptXuanshang = true;
            ModeIndex = 1;
            EndOperationIndex = 1;
            SelectedTime = 360;
            this.SizeToContent = SizeToContent.Manual;
            // Automatically resize width relative to content
            this.SizeToContent = SizeToContent.Height;

        }

        public List<int> TimeList { get { return timeList; } }

        List<int> timeList = new List<int>()
        {
            10,
            30,
            60,
            120,
            180,
            240,
            360,
        };

        public int SelectedTime { get; set; }

        public bool IsAutoAcceptXuanshang { get; set; }
        public bool IsAutoShutDown
        {
            get { return App.IsAutoShutDown; }
            set
            {
                App.IsAutoShutDown = value;
            }
        }
        int mi;
        public int ModeIndex
        {
            get { return mi; }
            set
            {
                mi = value;
                if (mi > 1) ImageComboBox.Visibility = Visibility.Collapsed;
                else ImageComboBox.Visibility = Visibility.Visible;
            }
        }
        public int IsUsingImageCapture { get; set; }
        public bool IsEnableInput
        {
            get { return !isstart; }
        }

        string log;
        public string Log
        {
            get { return log; }
            set
            {
                log = value;
                if (log.Length > 5000)
                {
                    log=log.Remove(0, 100);
                }
                Notify("Log");

                textBox.ScrollToEnd();
            }
        }


        bool isstart = false;

        async void StopByUser()
        {
            StartButton.IsEnabled = false;
            StartButton.Content = "等待处理中断命令..";
            App.FM.Stop();
            await Task.Delay(1200);
            StartButton.IsEnabled = true;
            StartButton.Content = "开始";
        }

        List<string> noteTextList = new List<string>()
        {
            "将在 {0} 分钟 {1} 秒后关闭阴阳师",
            "将在 {0} 分钟 {1} 秒后关闭计算机",
            "脚本将在 {0} 分钟 {1} 秒后停止",
        };

        public void SetStartButtonContent(int m,int s,int count)
        {
            if (EndOperationIndex < 0) EndOperationIndex = 0;
            var c = string.Format(noteTextList[EndOperationIndex], m, s);

            if (ModeIndex <= 1 && IsUsingImageCapture==0)
            {
                c += string.Format("  已进行：{0} 次御魂", count);
            }

            StartButton.Content = c;

        }

        public void SetStartButtonContentToDefault()
        {
            isstart = false;
            StartButton.Content = "开始";
            Notify("IsStart");
            Notify("IsEnableInput");
        }

        public int EndOperationIndex
        {
            get { return App.EndIndex; }
            set { App.EndIndex = value; }
        }

        public bool IsStart
        {
            get { return isstart; }
            set
            {
                //if (value)
                //{
                //    var b = App.BeforeImportantOP("留意到窗口底部的红色文字了吗，点击确定将开始运行脚本");
                //    if (!b) return;
                //}

                isstart = value;
                Notify("IsEnableInput");
                if (isstart)
                {
                    var b = IsUsingImageCapture == 0;
                    var yysmode = ModeIndex;
                    if (ModeIndex > 1) yysmode++;
                    App.FM.Run(SelectedTime, (YYSMode)yysmode, b,IsAutoAcceptXuanshang);
                }
                else
                {
                    StopByUser();
                }
            }
        }         


        public event PropertyChangedEventHandler PropertyChanged;
        protected void Notify(string PropName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropName));
            }
        }















    }
}
