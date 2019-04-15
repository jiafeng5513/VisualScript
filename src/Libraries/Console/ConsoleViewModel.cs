using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
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
    public partial class ConsoleViewModel : NotificationObject 
    {
        private readonly ConsoleModel _consoleModel;
        private readonly NodeViewModel nodeViewModel;
        private readonly NodeModel nodeModel;

        /// <summary>
        /// Combox数据源,填充可展开节点的名字(key)
        /// 此处负责数据绑定
        /// </summary>
        public ObservableCollection<string> ExportableNodeSource
        {
            get => _consoleModel.ExportableNodeSource;
            set => _consoleModel.ExportableNodeSource= value;
        }
        /// <summary>
        /// Combox当前选中项
        /// 此处负责数据绑定
        /// </summary>
        public string SelectedExportableNode
        {
            get => _consoleModel.SelectedExportableNode;
            set => _consoleModel.SelectedExportableNode = value;
        }


        public ConsoleViewModel(ConsoleModel model, NodeView nodeView)
        {
            _consoleModel = model;
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


        
    }
}
