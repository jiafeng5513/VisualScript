using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WatchImage
{
    /// <summary>
    /// WatchImageView.xaml 的交互逻辑
    /// </summary>
    public partial class WatchImageView : UserControl
    {
        public WatchImageView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 调整大小
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThumbResizeThumbOnDragDeltaHandler(object sender, DragDeltaEventArgs e)
        {
            var yAdjust = ActualHeight + e.VerticalChange;
            var xAdjust = ActualWidth + e.HorizontalChange;
        }
    }
}
