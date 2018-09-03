using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace yys_yuhun10_
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static FloatWindowManager FM = new FloatWindowManager();
        public static MainWindow MW { get; set; }
        public static Client yys_client = new Client();
        public static bool IsAutoShutDown { get; set; }

        public static int EndIndex { get; set; }
        public static bool BeforeImportantOP(string title)
        {
            return MessageBox.Show(title, "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }
    }
}
