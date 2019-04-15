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
    public partial class CnnTrainerViewModel : NotificationObject 
    {
        private readonly CnnTrainerModel _cnnTrainerModel;
        private readonly NodeViewModel nodeViewModel;
        private readonly NodeModel nodeModel;

        public int NumOfClasses
        {
            get => _cnnTrainerModel.NumOfClasses;
            set => _cnnTrainerModel.NumOfClasses = value;
        }

        public int MiniBatchSize
        {
            get => _cnnTrainerModel.MiniBatchSize;
            set => _cnnTrainerModel.MiniBatchSize = value;
        }

        public int TopN
        {
            get => _cnnTrainerModel.TopN;
            set => _cnnTrainerModel.TopN = value;
        }

        public CnnTrainerViewModel(CnnTrainerModel model, NodeView nodeView)
        {
            _cnnTrainerModel = model;
            nodeViewModel = nodeView.ViewModel;
            nodeModel = nodeView.ViewModel.NodeModel;
            model.PropertyChanged +=model_PropertyChanged;
            InitializeDelegateCommands();
        }

        private void model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "NumOfClasses":
                    RaisePropertyChanged("NumOfClasses");
                    break;
                case "MiniBatchSize":
                    RaisePropertyChanged("MiniBatchSize");
                    break;
                case "TopN":
                    RaisePropertyChanged("TopN");
                    break;
            }
        }
        #region DelegateCommand
        /// <summary>
        /// 开始训练
        /// </summary>
        /// <param name="parameters"></param>
        private void Start(object parameters)
        {
            CanSeeProgressBar = !CanSeeProgressBar;
            var cmd = new DynamoModel.CreateNodeCommand(Guid.NewGuid().ToString(), "String", -1, -1, true, false);
            cmd.Execute(DynamoModel.getInstance());

            /*
             * 第一步,进度条
             * 第二步,开始按钮禁用,停止按钮启用
             * 第三步,创建一个控制台节点,开始重定向
             * 第四步,训练线程开始
             * 
             */

        }
        /// <summary>
        /// 训练停止
        /// </summary>
        /// <param name="parameters"></param>
        private void Stop(object parameters)
        {
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
