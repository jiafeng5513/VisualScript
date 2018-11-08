using System;
using System.Windows;
using System.Windows.Controls;
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
        public SeeImageView(SeeImageModel Model, NodeView nodeView)
        {
            InitializeComponent();
           
            imageBox.Image = CvInvoke.Imread("D:\\图片\\素材\\timg.jpg");
            
        }
    }
}
