using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Dynamo.Controls;
using Dynamo.Graph;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Workspaces;
using Dynamo.Models;
using Dynamo.ViewModels;
using Dynamo.Wpf.Controls;

using ModelAnalyzerUI;

namespace Dynamo.Wpf.NodeViewCustomizations
{
    public class CnnTrainerViewCustomization : INodeViewCustomization<CnnTrainerModel>
    {
        private NodeModel nodeModel;
        private CnnTrainerView exporterControl;
        private NodeViewModel nodeViewModel;
        private CnnTrainerModel convertModel;
        private CnnTrainerViewModel exporterViewModel;

        public void CustomizeView(CnnTrainerModel model, NodeView nodeView)
        {
            nodeModel = nodeView.ViewModel.NodeModel;
            nodeViewModel = nodeView.ViewModel;
            convertModel = model;
            
            exporterControl = new CnnTrainerView(model, nodeView)
            {
                DataContext = new CnnTrainerViewModel(model, nodeView),
            };

            exporterViewModel = exporterControl.DataContext as CnnTrainerViewModel;
            nodeView.inputGrid.Children.Add(exporterControl);//inputGrid
            nodeView.inputGrid.Visibility = Visibility.Visible;

            exporterControl.Loaded += converterControl_Loaded;
            //exporterControl.SelectExportedUnit.PreviewMouseUp += SelectExportedUnit_PreviewMouseUp;
        }

        private void converterControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        public void Dispose()
        {
            //exporterControl.SelectExportedUnit.PreviewMouseUp -= SelectExportedUnit_PreviewMouseUp;
        }
    }
}
