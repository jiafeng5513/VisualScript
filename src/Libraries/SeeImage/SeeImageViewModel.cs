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
using DelegateCommand = Dynamo.UI.Commands.DelegateCommand;

namespace Dynamo.Wpf
{
    public class SeeImageViewModel : NotificationObject 
    {
        private readonly SeeImageModel _seeImageModel;
        private readonly NodeViewModel nodeViewModel;
        private readonly NodeModel nodeModel;

        public DelegateCommand ZoomOutCommand { get; set; }
        public DelegateCommand ZoomInCommand { get; set; }
        public DelegateCommand ZoomToDefaultSizeCommand { get; set; }

        /// <summary>
        /// 图片控件的指导宽度
        /// 此处负责数据绑定
        /// </summary>
        public double ImageWidth
        {
            get => _seeImageModel.ImageWidth;
            set => _seeImageModel.ImageWidth = value;
        }
        /// <summary>
        /// 图片控件的指导高度
        /// 此处负责数据绑定
        /// </summary>
        public double ImageHeight
        {
            get => _seeImageModel.ImageHeight;
            set => _seeImageModel.ImageHeight = value;
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="model"></param>
        /// <param name="nodeView"></param>
        public SeeImageViewModel(SeeImageModel model, NodeView nodeView)
        {
            _seeImageModel = model;
            nodeViewModel = nodeView.ViewModel;
            nodeModel = nodeView.ViewModel.NodeModel;
            model.PropertyChanged +=model_PropertyChanged;
            InitializeDelegateCommands();
        }

        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ImageHeight":
                    RaisePropertyChanged("ImageHeight");
                    break;
                case "ImageWidth":
                    RaisePropertyChanged("ImageWidth");
                    break;
            }
        }
        /// <summary>
        /// 放大
        /// </summary>
        /// <param name="parameters"></param>
        private void ZoomOut(object parameters)
        {
            ImageWidth *= 1.1;
        }
        /// <summary>
        /// 缩小
        /// </summary>
        /// <param name="parameters"></param>
        private void ZoomIn(object parameters)
        {
            if (ImageWidth < 180)
            {
                return;
            }
            ImageWidth *= 0.9;
        }
        /// <summary>
        /// 缩放到默认大小
        /// </summary>
        /// <param name="parameters"></param>
        private void ZoomToDefaultSize(object parameters)
        {
            ImageWidth = 300;
        }
        /// <summary>
        /// 初始化Commands
        /// </summary>
        private void InitializeDelegateCommands()
        {
            ZoomOutCommand = new DelegateCommand(ZoomOut, o => true);
            ZoomInCommand = new DelegateCommand(ZoomIn, o => true);
            ZoomToDefaultSizeCommand = new DelegateCommand(ZoomToDefaultSize, o => true);
        }

    }
}
