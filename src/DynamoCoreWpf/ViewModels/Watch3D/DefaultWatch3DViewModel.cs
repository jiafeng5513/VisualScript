using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Xml;
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
        protected readonly ILogger logger;
        protected readonly IEngineControllerManager engineManager;
        protected IRenderPackageFactory renderPackageFactory;
        protected IDynamoViewModel viewModel;

        protected List<NodeModel> recentlyAddedNodes = new List<NodeModel>();
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

        /// <summary>
        /// A flag which indicates whether this view model is used for a background preview.
        /// </summary>
        public virtual bool IsBackgroundPreview
        {
            get { return true; }
        }

        private bool canNavigateBackground = false;
        public bool CanNavigateBackground
        {
            get
            {
                return canNavigateBackground || navigationKeyIsDown;
            }
            set
            {
                canNavigateBackground = value;
                //Dynamo.Logging.Analytics.TrackScreenView(canNavigateBackground ? "Geometry" : "Nodes");
                RaisePropertyChanged("CanNavigateBackground");
            }
        }

        /// <summary>
        /// A flag which indicates whether Isolate Selected Geometry mode is activated.
        /// </summary>
        private bool isolationMode;
        public bool IsolationMode
        {
            get
            {
                return isolationMode;
            }
            set
            {
                isolationMode = value;
                RaisePropertyChanged("IsolationMode");
            }
        }

        /// <summary>
        /// A flag which indicates whether the user is holding the
        /// navigation override key (ESC).
        /// </summary>
        private bool navigationKeyIsDown = false;
        //public bool NavigationKeyIsDown
        //{
        //    get { return navigationKeyIsDown; }
        //    set
        //    {
        //        if (navigationKeyIsDown == value) return;

        //        navigationKeyIsDown = value;
        //        RaisePropertyChanged("NavigationKeyIsDown");
        //        RaisePropertyChanged("CanNavigateBackground");
        //    }
        //}

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
            logger = parameters.Logger;
            engineManager = parameters.EngineControllerManager;

            Name = Resources.BackgroundPreviewDefaultName;
            active = parameters.Preferences.IsBackgroundPreviewActive;
            logger = parameters.Logger;

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

        
        protected virtual void OnClear()
        {
            // Override in inherited classes.
        }


        /// <summary>
        /// Event to be handled when the background preview is toggled on or off
        /// On/off state is passed using the bool parameter
        /// </summary>
        public event Action<bool> CanNavigateBackgroundPropertyChanged;

        protected virtual void OnIsolationModeRequestUpdate()
        {
            // Override in inherited classes.
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


        protected virtual void OnWorkspaceOpening(object obj)
        {
            // Override in derived classes.
        }


        protected virtual void OnWorkspaceSaving(XmlDocument doc)
        {
            // Override in derived classes
        }

        public virtual CameraData GetCameraInformation()
        {
            // Override in derived classes.
            return null;
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

        public virtual void DeleteGeometryForNode(NodeModel node, bool requestUpdate = true)
        {
            // Override in derived classes.
        }

        public virtual void DeleteGeometryForIdentifier(string identifier, bool requestUpdate = true)
        {
            // Override in derived classes.
        }

        public virtual void HighlightNodeGraphics(IEnumerable<NodeModel> nodes)
        {
            // Override in derived classes.
        }

        public virtual void UnHighlightNodeGraphics(IEnumerable<NodeModel> nodes)
        {
            // Override in derived classes.
        }


        protected virtual void SelectionChangedHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Override in derived classes
        }

        protected virtual void OnRenderPackagesUpdated(NodeModel node, RenderPackageCache packages)
        {
            RemoveGeometryForNode(node);
            if (packages.IsEmpty())
                return;

            // If there is no attached model update for all render packages
            if (watchModel == null)
            {
                AddGeometryForRenderPackages(packages);
                return;
            }

            // If there are no input ports update for all render packages
            var inPorts = watchModel.InPorts;
            if (inPorts == null || inPorts.Count() < 1)
            {
                AddGeometryForRenderPackages(packages);
                return;
            }

            // If there are no connectors connected to the first (only) input port update for all render packages
            var inConnectors = inPorts[0].Connectors;
            if (inConnectors == null || inConnectors.Count() < 1 || inConnectors[0].Start == null)
            {
                AddGeometryForRenderPackages(packages);
                return;
            }

            // Only update for render packages from the connected output port
            var inputId = inConnectors[0].Start.GUID;
            foreach (var port in node.OutPorts)
            {
                if (port.GUID != inputId)
                {
                    continue;
                }

                RenderPackageCache portPackages = packages.GetPortPackages(inputId);
                if (portPackages == null)
                {
                    continue;
                }

                AddGeometryForRenderPackages(portPackages);
            }
        }

        /// <summary>
        /// Called from derived classes when a collection of render packages
        /// are available to be processed as render geometry.
        /// </summary>
        /// <param name="taskPackages">A collection of packages from which to 
        /// create render geometry.</param>
        public virtual void GenerateViewGeometryFromRenderPackagesAndRequestUpdate(
            RenderPackageCache taskPackages)
        {
            // Override in derived classes
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

        //internal void OnViewCameraChanged(object sender, RoutedEventArgs args)
        //{
        //    var handler = ViewCameraChanged;
        //    if (handler != null) handler(sender, args);
        //}

        protected virtual void OnNodePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Override in derived classes.
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
