using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace yys_yuhun10_
{
    /// <summary>
    /// FloatWindow.xaml 的交互逻辑
    /// </summary>
    public partial class FloatWindow
    {
        public FloatWindow(string st)
        {
            InitializeComponent();
            Topmost = true;
            text.Text = st;
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }

        public async void flash()
        {
            SolidColorBrush src = back.Background as SolidColorBrush;
            SolidColorBrush x = FindResource("AppColor") as SolidColorBrush;
            back.Background = x;
            await Task.Delay(50);
            back.Background = src;
        }
    }
}
