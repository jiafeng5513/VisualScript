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
        public DelegateCommand ZoomToRealSizeCommand { get; set; }
        public DelegateCommand ZoomToDefaultSizeCommand { get; set; }


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
                case "ExportableNodeSource":
                    RaisePropertyChanged("ExportableNodeSource");
                    break;
                case "SelectedExportableNode":
                    RaisePropertyChanged("SelectedExportableNode");
                    break;
            }
        }
        /// <summary>
        /// 放大
        /// </summary>
        /// <param name="parameters"></param>
        private void ZoomOut(object parameters)
        {

        }
        /// <summary>
        /// 缩小
        /// </summary>
        /// <param name="parameters"></param>
        private void ZoomIn(object parameters)
        {

        }
        /// <summary>
        /// 缩放到真实大小
        /// </summary>
        /// <param name="parameters"></param>
        private void ZoomToRealSize(object parameters)
        {

        }
        /// <summary>
        /// 缩放到默认大小
        /// </summary>
        /// <param name="parameters"></param>
        private void ZoomToDefaultSize(object parameters)
        {

        }
        /// <summary>
        /// 初始化Commands
        /// </summary>
        private void InitializeDelegateCommands()
        {
            ZoomOutCommand = new DelegateCommand(ZoomOut, o => true);
            ZoomInCommand = new DelegateCommand(ZoomIn, o => true);
            ZoomToRealSizeCommand = new DelegateCommand(ZoomToRealSize, o => true);
            ZoomToDefaultSizeCommand = new DelegateCommand(ZoomToDefaultSize, o => true);
        }

    }
}
