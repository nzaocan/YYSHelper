using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
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
        int ready_x, ready_y, begin_x, begin_y;
        public MainWindow()
        {
            InitializeComponent();
            App.MW = this;
            this.DataContext = this;

            begin_win = new FloatWindow("开");
            ready_win = new FloatWindow("准");
            color_win = new FloatWindow("组");

            App.FM.ready_win = ready_win;
            App.FM.begin_win = begin_win;
            App.FM.color_win = color_win;

        }

        string log;
        public string Log
        {
            get { return log; }
            set
            {
                log = value;
                if (log.Length > 500)
                {
                    log=log.Remove(0, 100);
                }
                Notify("Log");

                textBox.ScrollToEnd();
            }
        }

        int index = 0;

        public void SetText(int x, int y)
        {
            IsEnabled = true;
            string s = x + " , " + y;
            if (index == 0)
            {
                ReadyText.Text = s;
                ready_x = x;
                ready_y = y;
            }
            else
            {
                BeginText.Text = s;
                begin_x = x;
                begin_y = y;
            }

            ReadyButton.IsChecked = false;
            BeginButton.IsChecked = false;
        }

        FloatWindow begin_win;
        FloatWindow ready_win;
        FloatWindow color_win;

        public bool IsShowBeginWin { get; set; }
        public bool IsShowReadyWin { get; set; }
        public bool IsShowColorWin { get; set; }

        bool isstart = false;
        public bool IsStart
        {
            get { return isstart; }
            set
            {
                if (value)
                {
                    if (!IsShowBeginWin  || !IsShowReadyWin)
                    {
                        MessageBox.Show("信息没有采集完毕");
                        return;
                    }
                    var b = App.BeforeImportantOP("点击确定将开始挂机御魂10");
                    if (!b) return;
                }

                isstart = value;
                if (isstart)
                {
                    StartButton.Content = "中断";
                    App.FM.Begin();
                }
                else
                {
                    StartButton.Content = "开始";
                    App.FM.Stop();
                }
            }
        }         

        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsShowColorWin)
                color_win.Show();
            else
                color_win.Hide();
        }

        private void BeginButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsShowBeginWin)
                begin_win.Show();
            else
                begin_win.Hide();

        }

        private void ReadyButton_Click(object sender, RoutedEventArgs e)
        {
            FloatWindowManager.test();
            if (IsShowReadyWin)
                ready_win.Show();
            else
                ready_win.Hide();
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
