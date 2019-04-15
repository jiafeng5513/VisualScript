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

namespace ConsoleView
{
    /// <summary>
    /// ConsoleView.xaml 的交互逻辑
    /// </summary>
    public partial class ConsoleView : Window
    {
        public ConsoleView()
        {
            InitializeComponent();
            Console.SetOut(new TextBoxWriter(ConsoleEditor));//重定向开始

            Console.Write("helloword");
            Console.WriteLine("helloword");

        }
    }
}
