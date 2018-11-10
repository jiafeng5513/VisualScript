using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Dynamo.Core;
using Dynamo.Graph.Nodes;
using Dynamo.Interfaces;
using Dynamo.Logging;
using Dynamo.Models;
using Dynamo.Scheduler;
using Dynamo.ViewModels;
using Dynamo.Visualization;
using Dynamo.Wpf.Properties;

namespace Dynamo.Wpf.ViewModels.Watch3D
{
    public class Watch3DViewModelStartupParams
    {
        public IDynamoModel Model { get; set; }
        public IScheduler Scheduler { get; set; }
        public ILogger Logger { get; set; }
        public IPreferences Preferences { get; set; }
        public IEngineControllerManager EngineControllerManager { get; set; }
        public Watch3DViewModelStartupParams(DynamoModel model)
        {
            Model = model;
            Scheduler = model.Scheduler;
            Logger = model.Logger;
            Preferences = model.PreferenceSettings;
            EngineControllerManager = model;
        }
    }

    /// <summary>
    /// The DefaultWatch3DViewModel is the base class for all 3D previews in Dynamo.
    /// Classes which derive from this base are used to prepare geometry for 
    /// rendering by various render targets. The base class handles the registration
    /// of all necessary event handlers on models, workspaces, and nodes.
    /// </summary>
    public class DefaultWatch3DViewModel : NotificationObject, IWatch3DViewModel, IDisposable
    {
        protected readonly NodeModel watchModel;

        protected readonly IDynamoModel dynamoModel;
        protected readonly IScheduler scheduler;
        protected readonly IPreferences preferences;
        protected readonly IEngineControllerManager engineManager;
        protected IRenderPackageFactory renderPackageFactory;
        protected IDynamoViewModel viewModel;
        protected bool active;

        /// <summary>
        /// Represents the name of current Watch3DViewModel which will be saved in preference settings
        /// </summary>
        public virtual string PreferenceWatchName { get { return "IsBackgroundPreviewActive"; } }

        /// <summary>
        /// A flag which indicates whether this Watch3DView should process
        /// geometry updates. When set to False, the Watch3DView corresponding
        /// to this view model is not displayed.
        /// </summary>
        public bool Active
        {
            get { return active; }
            set
            {
                if (active == value)
                {
                    return;
                }

                active = value;
                preferences.SetIsBackgroundPreviewActive(PreferenceWatchName, value);               
                RaisePropertyChanged("Active");

                //OnActiveStateChanged();

                if (active)
                {
                    RegenerateAllPackages();
                }
            }
        }

        /// <summary>
        /// A name which identifies this view model when multiple
        /// Watch3DViewModel objects exist.
        /// </summary>
        public string Name { get; set; } 

        public bool CanBeActivated { get; internal set; }

        /// <summary>
        /// The DefaultWatch3DViewModel is used in contexts where a complete rendering environment
        /// cannot be established. Typically, this is machines that do not have GPUs, or do not
        /// support DirectX 10 feature levels. For most purposes, you will want to use a <see cref="HelixWatch3DViewModel"/>
        /// </summary>
        /// <param name="model">The NodeModel that this watch is displaying.</param>
        /// <param name="parameters">A Watch3DViewModelStartupParams object.</param>
        public DefaultWatch3DViewModel(NodeModel model, Watch3DViewModelStartupParams parameters)
        {
            watchModel = model;
            dynamoModel = parameters.Model;
            scheduler = parameters.Scheduler;
            preferences = parameters.Preferences;
            engineManager = parameters.EngineControllerManager;

            Name = Resources.BackgroundPreviewDefaultName;
            active = parameters.Preferences.IsBackgroundPreviewActive;

            CanBeActivated = true;
        }

        /// <summary>
        /// Call setup to establish the visualization context for the
        /// Watch3DViewModel. Because the Watch3DViewModel is passed into the DynamoViewModel,
        /// Setup is required to fully establish the rendering context. 
        /// </summary>
        /// <param name="viewModel">An IDynamoViewModel object.</param>
        /// <param name="renderPackageFactory">An IRenderPackageFactory object.</param>
        public void Setup(IDynamoViewModel viewModel, 
            IRenderPackageFactory renderPackageFactory)
        {
            this.viewModel = viewModel;
            this.renderPackageFactory = renderPackageFactory;
        }

        /// <summary>
        /// Forces a regeneration of the render packages for all nodes.
        /// </summary>
        public void RegenerateAllPackages()
        {
            foreach (var node in
                dynamoModel.CurrentWorkspace.Nodes)
            {
                node.RequestVisualUpdateAsync(scheduler, engineManager.EngineController,
                        renderPackageFactory, true);
            }
        }


        /// <summary>
        /// Call this method to remove render pakcages that created by node.
        /// </summary>
        /// <param name="node"></param>
        public virtual void RemoveGeometryForNode(NodeModel node)
        {
            // Override in inherited classes.
        }

        /// <summary>
        /// Call this method to add the render package. 
        /// </summary>
        /// <param name="packages"></param>
        /// <param name="forceAsyncCall"></param>
        public virtual void AddGeometryForRenderPackages(RenderPackageCache packages, bool forceAsyncCall = false)
        {
            // Override in inherited classes.
        }

        public virtual void AddLabelForPath(string path)
        {
            // Override in inherited classes.
        }

        /// <summary>
        /// Remove the labels (in Watch3D View) for geometry once the Watch node is disconnected
        /// </summary>
        /// <param name="path"></param>
        public virtual void ClearPathLabel(string path)
        {
            // Override in inherited classes.
        }

        public void Invoke(Action action)
        {
            var dynamoViewModel = viewModel as DynamoViewModel;
            if (dynamoViewModel != null)
            {
                dynamoViewModel.UIDispatcher.Invoke(action);
            }
        }

        internal event Func<MouseEventArgs, IRay> RequestClickRay;

        /// <summary>
        /// Returns a 3D ray from the camera to the given mouse location
        /// in world coordinates that can be used to perform a hit-test 
        /// on objects in the view
        /// </summary>
        /// <param name="args">mouse click location in screen coordinates</param>
        /// <returns></returns>
        public IRay GetClickRay(MouseEventArgs args)
        {
            return RequestClickRay != null ? RequestClickRay(args) : null;
        }

        internal event Func<Point3D> RequestCameraPosition;
        public Point3D? GetCameraPosition()
        {
            var handler = RequestCameraPosition;
            if (handler != null) return handler();
            return null;
        }

        public event Action<object, MouseButtonEventArgs> ViewMouseDown;

        public event Action<object, MouseButtonEventArgs> ViewMouseUp;
        //internal void OnViewMouseUp(object sender, MouseButtonEventArgs e)
        //{
        //    var handler = ViewMouseUp;
        //    if (handler != null) handler(sender, e);
        //}

        public event Action<object, MouseEventArgs> ViewMouseMove;
        //internal void OnViewMouseMove(object sender, MouseEventArgs e)
        //{
        //    var handler = ViewMouseMove;
        //    if (handler != null) handler(sender, e);
        //}

        public event Action<object, RoutedEventArgs> ViewCameraChanged;

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
