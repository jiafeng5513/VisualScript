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
    public class ConsoleViewCustomization : INodeViewCustomization<ConsoleModel>
    {
        private NodeModel nodeModel;
        private AnalyzerView exporterControl;
        private NodeViewModel nodeViewModel;
        private ConsoleModel convertModel;
        private ConsoleViewModel exporterViewModel;

        public void CustomizeView(ConsoleModel model, NodeView nodeView)
        {
            nodeModel = nodeView.ViewModel.NodeModel;
            nodeViewModel = nodeView.ViewModel;
            convertModel = model;
            
            exporterControl = new AnalyzerView(model, nodeView)
            {
                DataContext = new ConsoleViewModel(model, nodeView),
            };

            exporterViewModel = exporterControl.DataContext as ConsoleViewModel;
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
