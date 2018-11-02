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
    public partial class AnalyzerViewModel : NotificationObject 
    {
        private readonly AnalyzerModel _analyzerModel;
        private readonly NodeViewModel nodeViewModel;
        private readonly NodeModel nodeModel;

        /// <summary>
        /// Combox数据源,填充可展开节点的名字(key)
        /// 此处负责数据绑定
        /// </summary>
        public ObservableCollection<string> ExportableNodeSource
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
                case "ExportableNodeSource":
                    RaisePropertyChanged("ExportableNodeSource");
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
        private void Explode(object parameters)
        {
            var cmd = new DynamoModel.CreateNodeCommand(Guid.NewGuid().ToString(), "String", -1, -1, true, false);
            cmd.Execute(DynamoModel.getInstance());

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        private void Predict(object parameters)
        {
            CanSeeProgressBar = !CanSeeProgressBar;
            using (StreamReader sw = new StreamReader(System.Environment.CurrentDirectory+"\\ModelDecompilerParams.json"))
            {
                string json = sw.ReadToEnd();
                ExplodeProto param=JsonConvert.DeserializeObject<ExplodeProto>(json);
                var ModelFile = param.ModelFile;
                var LabelFile = param.LabelFile;
                var inputMat = param.inMat;
                ProtoTools _protoTools = new ProtoTools(ModelFile);
                foreach (var node in _protoTools.Map)
                {
                    ExportableNodeSource.Add(node.Value.Name);
                }
            }

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
