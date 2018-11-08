using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;
using DeepLearning.Visible;
using Dynamo.Controls;
using Dynamo.Core;
using Dynamo.Graph.Nodes;
using Dynamo.Models;
using Dynamo.UI.Commands;
using Dynamo.ViewModels;

using ModelAnalyzerUI;
using Newtonsoft.Json;
using ProtobufTools;
using TensorProtocol;

namespace Dynamo.Wpf
{
    public class SeeImageViewModel : NotificationObject 
    {
        private readonly SeeImageModel _seeImageModel;
        private readonly NodeViewModel nodeViewModel;
        private readonly NodeModel nodeModel;

        public SeeImageViewModel(SeeImageModel model, NodeView nodeView)
        {
            _seeImageModel = model;
            nodeViewModel = nodeView.ViewModel;
            nodeModel = nodeView.ViewModel.NodeModel;
            model.PropertyChanged +=model_PropertyChanged;
            
        }

        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ExportableNodeSource":
                    RaisePropertyChanged("ExportableNodeSource");
                    break;
                case "SelectedExportableNode":
                    RaisePropertyChanged("SelectedExportableNode");
                    break;
            }
        }


        private bool canSeeProgressBar=false;
        public bool CanSeeProgressBar
        {
            get { return canSeeProgressBar; }
            set { canSeeProgressBar = value;
                RaisePropertyChanged("CanSeeProgressBar");
            }
        }

        public static BitmapSource GetBitmapImage_002()
        {
            //获取文件流
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            //格式为：项目名称-文件夹地址-文件名称
            Stream myStream = myAssembly.GetManifestResourceStream("SeeImage.ico.plus.ico");
            //图片格式
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = myStream;
            image.EndInit();
            myStream.Dispose();
            myStream.Close();
            return image;
        }
    }
}
