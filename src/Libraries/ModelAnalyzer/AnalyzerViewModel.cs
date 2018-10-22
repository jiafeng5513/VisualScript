using System;
using System.Collections.Generic;
using Dynamo.Controls;
using Dynamo.Core;
using Dynamo.Graph.Nodes;
using Dynamo.Models;
using Dynamo.UI.Commands;
using Dynamo.ViewModels;

using ModelAnalyzerUI;

namespace Dynamo.Wpf
{
    public partial class AnalyzerViewModel : NotificationObject 
    {
        private readonly AnalyzerModel _analyzerModel;
        private readonly NodeViewModel nodeViewModel;
        private readonly NodeModel nodeModel;

        /// <summary>
        /// Combox数据源,填充可展开节点的名字(key)
        /// 此处负责数据绑定
        /// </summary>
        public List<string> ExportableNodeSource
        {
            get => _analyzerModel.ExportableNodeSource;
            set => _analyzerModel.ExportableNodeSource= value;
        }
        /// <summary>
        /// Combox当前选中项
        /// 此处负责数据绑定
        /// </summary>
        public string SelectedExportableNode
        {
            get => _analyzerModel.SelectedExportableNode;
            set => _analyzerModel.SelectedExportableNode = value;
        }


        public AnalyzerViewModel(AnalyzerModel model, NodeView nodeView)
        {
            _analyzerModel = model;
            nodeViewModel = nodeView.ViewModel;
            nodeModel = nodeView.ViewModel.NodeModel;
            model.PropertyChanged +=model_PropertyChanged;
            InitializeDelegateCommands();
        }

        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedExportableNodeSource":
                    RaisePropertyChanged("SelectedExportableNodeSource");
                    break;
                case "SelectedExportableNode":
                    RaisePropertyChanged("SelectedExportableNode");
                    break;
            }
        }
        #region DelegateCommand
        /// <summary>
        /// 网络展开
        /// </summary>
        /// <param name="parameters"></param>
        private void Explore(object parameters)
        {
            var cmd = new DynamoModel.CreateNodeCommand(Guid.NewGuid().ToString(), "String", -1, -1, true, false);
            cmd.Execute(DynamoModel.getInstance());

        }
        private void Predict(object parameters)
        {
            var cmd = new DynamoModel.CreateNodeCommand(Guid.NewGuid().ToString(), "String", -1, -1, true, false);
            cmd.Execute(DynamoModel.getInstance());
            CanSeeProgressBar = !CanSeeProgressBar;
        }
        #endregion

        private bool canSeeProgressBar=false;
        public bool CanSeeProgressBar
        {
            get { return canSeeProgressBar; }
            set { canSeeProgressBar = value;
                RaisePropertyChanged("CanSeeProgressBar");
            }
        }
    }
}
