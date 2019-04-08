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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;

namespace BrowserCore
{
    /// <summary>
    /// EmbeddedBrowser.xaml 的交互逻辑
    /// </summary>
    public partial class EmbeddedBrowser : Window
    {
        public EmbeddedBrowser()
        {
            InitializeComponent();
        }
        ChromiumWebBrowser webView;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var setting = new CefSettings();
            if (Cef.IsInitialized == false)
            {
                // Cef.Initialize(setting, true, false);
                Cef.Initialize(setting);
            }
            webView = new ChromiumWebBrowser();
            grid.Children.Add(webView);

            /*string path= AppDomain.CurrentDomain.BaseDirectory + @"\gis.offline\index.html";
            webView.Address = path;*/
            webView.Address = "http://www.baidu.com";
            webView.LifeSpanHandler = new OpenPageSelf();
            // webView.PreviewTextInput += new TextCompositionEventHandler(OnPreviewTextInput);
        }
        // 修复中文的bug
        protected void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (char t in e.Text)
            {
                if (IsChinese(t))
                    webView.GetBrowser().GetHost().SendKeyEvent((int)CefSharp.Wpf.WM.CHAR, (int)t, 0);
            }
            base.OnPreviewTextInput(e);
        }
        /// <summary>
        /// 判断是否中文
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public bool IsChinese(char Text)
        {

            if ((int)Text > 127)
                return true;

            return false;
        }
    }
}
