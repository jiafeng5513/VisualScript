using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CoreNodeModels;
using Dynamo.Controls;
using Dynamo.Graph.Nodes;
using Dynamo.Utilities;
using Dynamo.Wpf;
using Image = System.Windows.Controls.Image;

namespace WatchImage
{
    class WatchImageCustomization : INodeViewCustomization<WatchImageModel>
    {
        private Image image;
        private NodeModel nodeModel;
        private NodeView nodeView;

        private WatchImageView imageView;  //未初始化,未得到正确的值
        private WatchImageModel imageModel;//未初始化,未得到正确的值

        public void CustomizeView(WatchImageModel nodeModel, NodeView nodeView)
        {
            this.imageModel = nodeModel;
            this.nodeView = nodeView;

            imageView = new WatchImageView()
            {
                DataContext = new WatchImageViewModel(),
            };

            image = new Image
            {
                MaxWidth = 200,
                MaxHeight = 200,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                Name = "image1",
                VerticalAlignment = System.Windows.VerticalAlignment.Center
            };

            // Update the image when the property is updated

            nodeModel.PropertyChanged += NodeModelOnPropertyChanged;

            imageView.PresentationGrid.Children.Add(image);
            imageView.PresentationGrid.Visibility = Visibility.Visible;

            HandleMirrorData();
        }

        private void NodeModelOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != "CachedValue")
                return;

            HandleMirrorData();
        }

        private void HandleMirrorData()
        {
            var data = imageModel.CachedValue;
            if (data == null)
                return;

            var bitmap = data.Data as Bitmap;
            if (bitmap != null)
            {
                imageView.Dispatcher.Invoke(new Action<Bitmap>(SetImageSource), new object[] { bitmap });
            }
        }

        private void SetImageSource(Bitmap bmp)
        {
            image.Source = ResourceUtilities.ConvertToImageSource(bmp);
        }

        public void Dispose()
        {
            nodeModel.PropertyChanged -= NodeModelOnPropertyChanged;
        }
    }
}
