using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows;
using Dynamo.Controls;
using Dynamo.Graph;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Dynamo.Utilities;
using Dynamo.ViewModels;
using Dynamo.Wpf.Controls;
using Image = System.Windows.Controls.Image;
using ModelAnalyzerUI;
using SeeImage;

namespace Dynamo.Wpf.NodeViewCustomizations
{
    public class SeeImageViewCustomization : INodeViewCustomization<SeeImageModel>
    {
        private NodeModel nodeModel;
        private NodeViewModel nodeViewModel;

        private SeeImageView seeImageView;
        private SeeImageButtonControl seeImageButtonControl;
        private SeeImageModel convertModel;
        private SeeImageViewModel seeImageViewModel;

        private Image image;

        public void CustomizeView(SeeImageModel model, NodeView nodeView)
        {
            nodeModel = nodeView.ViewModel.NodeModel;
            nodeViewModel = nodeView.ViewModel;
            convertModel = model;

            seeImageButtonControl = new SeeImageButtonControl()
            {
                DataContext = new SeeImageViewModel(model, nodeView),
            };
            seeImageView = new SeeImageView(model, nodeView)
            {
                DataContext = new SeeImageViewModel(model, nodeView),
            };
            
            seeImageViewModel = seeImageView.DataContext as SeeImageViewModel;

            nodeView.PresentationGrid.Children.Add(seeImageView);
            nodeView.PresentationGrid.Visibility = Visibility.Visible;

            nodeView.inputGrid.Children.Add(seeImageButtonControl);
            nodeView.inputGrid.Visibility = Visibility.Visible;

            nodeModel.PropertyChanged += NodeModelOnPropertyChanged;
            seeImageView.Loaded += converterControl_Loaded;
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
            var data = nodeModel.CachedValue;
            if (data == null)
                return;

            var bitmap = data.Data as Bitmap;
            if (bitmap != null)
            {
                seeImageView.Dispatcher.Invoke(new Action<Bitmap>(SetImageSource), new object[] { bitmap });
            }
        }

        private void SetImageSource(Bitmap bmp)
        {
            seeImageView.imageBox2.Source = ResourceUtilities.ConvertToImageSource(bmp);
        }

        private void converterControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        public void Dispose()
        {
            nodeModel.PropertyChanged -= NodeModelOnPropertyChanged;
        }
    }
}
