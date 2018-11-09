using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Dynamo.Controls;
using Emgu.CV;
using ModelAnalyzerUI;


namespace Dynamo.Wpf.Controls
{
    /// <summary>
    /// Interaction logic for ExportAsSATControl.xaml
    /// </summary>
    public partial class SeeImageView : UserControl
    {
        private double oldwidth = 0;
        private double oldheight = 0;
        public SeeImageView(SeeImageModel Model, NodeView nodeView)
        {
            InitializeComponent();

            //imageBox.Image = CvInvoke.Imread("D:\\图片\\IMG_00000001.jpg");
            imageBox2.StretchDirection = StretchDirection.Both;
            imageBox2.Stretch = Stretch.Uniform;

            oldwidth = userGrid.ActualWidth;
            oldheight = userGrid.ActualHeight;

            userGrid.SizeChanged += OnSizeChanged;
            
        }
        private void OnSizeChanged(object sender, EventArgs eventArgs)
        {
            var w_ratio = userGrid.ActualWidth / oldwidth;
            var h_ratio = userGrid.ActualHeight / oldheight;
            var ratio = Math.Min(w_ratio, h_ratio);
            var size = new[] { ActualWidth, userGrid.ActualHeight };//这是现在的窗口尺寸

            //imageBox.SetZoomScale(ratio, new System.Drawing.Point(0,0));
            //imageBox.AutoSize = true;
            oldwidth = userGrid.ActualWidth;
            oldheight = userGrid.ActualHeight;
        }
    }
}
