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
using Image = System.Windows.Controls.Image;
using ModelAnalyzerUI;
using SeeImage;

namespace Dynamo.Wpf.NodeViewCustomizations
{
    public class SeeImageViewCustomization : INodeViewCustomization<SeeImageModel>
    {
        private NodeModel nodeModel;
        private SeeImageView seeImageView;
        private SeeImageButtonControl seeImageButtonControl;
        private NodeViewModel nodeViewModel;
        private SeeImageModel convertModel;
        private SeeImageViewModel exporterViewModel;
        private Image image;

        public void CustomizeView(SeeImageModel model, NodeView nodeView)
        {
            nodeModel = nodeView.ViewModel.NodeModel;
            nodeViewModel = nodeView.ViewModel;
            convertModel = model;
            
            seeImageView = new SeeImageView(model, nodeView)
            {
                DataContext = new SeeImageViewModel(model, nodeView),
            };
            seeImageButtonControl = new SeeImageButtonControl();
            exporterViewModel = seeImageView.DataContext as SeeImageViewModel;
            nodeView.PresentationGrid.Children.Add(seeImageView);
            nodeView.PresentationGrid.Visibility = Visibility.Visible;

            nodeView.inputGrid.Children.Add(seeImageButtonControl);
            nodeView.inputGrid.Visibility = Visibility.Visible;

            seeImageView.Loaded += converterControl_Loaded;
            //exporterControl.SelectExportedUnit.PreviewMouseUp += SelectExportedUnit_PreviewMouseUp;
        }

        //private void SelectExportedUnit_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    nodeViewModel.WorkspaceViewModel.HasUnsavedChanges = true;
        //    var undoRecorder = nodeViewModel.WorkspaceViewModel.Model.UndoRecorder;
        //    WorkspaceModel.RecordModelForModification(nodeModel, undoRecorder);
        //}

        private void converterControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        public void Dispose()
        {
            //exporterControl.SelectExportedUnit.PreviewMouseUp -= SelectExportedUnit_PreviewMouseUp;
        }
    }
}
