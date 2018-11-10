using System;
using System.Collections.Generic;
using System.Linq;
using Dynamo.Engine;
using Dynamo.Graph.Nodes;
using Dynamo.Visualization;

namespace Dynamo.Scheduler
{
    class UpdateRenderPackageParams
    {
        internal IRenderPackageFactory RenderPackageFactory { get; set; }
        internal string PreviewIdentifierName { get; set; }
        internal NodeModel Node { get; set; }
        internal EngineController EngineController { get; set; }
        internal IEnumerable<KeyValuePair<Guid, string>> DrawableIdMap { get; set; }

        internal bool ForceUpdate { get; set; }
    }

    /// <summary>
    /// An asynchronous task to regenerate render packages for a given node. 
    /// During execution the task retrieves the corresponding IGraphicItem from 
    /// EngineController through a set of drawable identifiers supplied by the 
    /// node. These IGraphicItem objects then fill the IRenderPackage objects 
    /// with tessellated geometric data. Each of the resulting IRenderPackage 
    /// objects is then tagged with a label.
    /// </summary>
    /// 
    class UpdateRenderPackageAsyncTask : AsyncTask
    {

        #region Class Data Members and Properties

        protected Guid nodeGuid;

        private bool displayLabels;
        private bool isNodeSelected;
        private string previewIdentifierName;
        private EngineController engineController;
        private IEnumerable<KeyValuePair<Guid, string>> drawableIdMap;
        private readonly RenderPackageCache renderPackageCache;
        private IRenderPackageFactory factory;

        internal RenderPackageCache RenderPackages
        {
            get { return renderPackageCache; }
        }

        public override TaskPriority Priority
        {
            get { return TaskPriority.Normal; }
        }

        #endregion

        #region Public Class Operational Methods

        internal UpdateRenderPackageAsyncTask(IScheduler scheduler)
            : base(scheduler)
        {
            nodeGuid = Guid.Empty;
            renderPackageCache = new RenderPackageCache();
        }

        internal bool Initialize(UpdateRenderPackageParams initParams)
        {
            if (initParams == null)
                throw new ArgumentNullException("initParams");
            if (initParams.Node == null)
                throw new ArgumentNullException("initParams.Node");
            if (initParams.EngineController == null)
                throw new ArgumentNullException("initParams.EngineController");
            if (initParams.DrawableIdMap == null)
                throw new ArgumentNullException("initParams.DrawableIdMap");

            var nodeModel = initParams.Node;
            if (nodeModel.WasRenderPackageUpdatedAfterExecution && !initParams.ForceUpdate)
                return false; // Not has not been updated at all.

            // If a node is in either of the following states, then it will not 
            // produce any geometric output. Bail after clearing the render packages.
            if (nodeModel.IsInErrorState || !nodeModel.IsVisible)
                return false;

            // Without AstIdentifierForPreview, a node cannot have MirrorData.
            if (string.IsNullOrEmpty(nodeModel.AstIdentifierForPreview.Value))
                return false;

            drawableIdMap = initParams.DrawableIdMap;
            if (!drawableIdMap.Any())
                return false; // Nothing to be drawn.

            displayLabels = nodeModel.DisplayLabels;
            isNodeSelected = nodeModel.IsSelected;
            factory = initParams.RenderPackageFactory;
            engineController = initParams.EngineController;
            previewIdentifierName = initParams.PreviewIdentifierName;

            nodeGuid = nodeModel.GUID;
            nodeModel.WasRenderPackageUpdatedAfterExecution = true;
            return true;
        }

        #endregion

        #region Protected Overridable Methods

        protected override void HandleTaskExecutionCore()
        {
            if (nodeGuid == Guid.Empty)
            {
                throw new InvalidOperationException(
                    "UpdateRenderPackageAsyncTask.Initialize not called");
            }

            var idEnum = drawableIdMap.GetEnumerator();
            while (idEnum.MoveNext())
            {
                var mirrorData = engineController.GetMirror(idEnum.Current.Value);
                if (mirrorData == null)
                    continue;
            }
        }

        protected override void HandleTaskCompletionCore()
        {
        }

        protected override TaskMergeInstruction CanMergeWithCore(AsyncTask otherTask)
        {
            var theOtherTask = otherTask as UpdateRenderPackageAsyncTask;
            if (theOtherTask == null)
                return base.CanMergeWithCore(otherTask);

            // If the two UpdateRenderPackageAsyncTask are for different nodes,
            // then there is no comparison to be made, keep both the tasks.
            // 
            if (nodeGuid != theOtherTask.nodeGuid)
                return TaskMergeInstruction.KeepBoth;

            // Comparing to another NotifyRenderPackagesReadyAsyncTask, the one 
            // that gets scheduled more recently stay, while the earlier one 
            // gets dropped. If this task has a higher tick count, keep this.
            // 
            if (ScheduledTime.TickCount > theOtherTask.ScheduledTime.TickCount)
                return TaskMergeInstruction.KeepThis;

            return TaskMergeInstruction.KeepOther; // Otherwise, keep the other.
        }

        #endregion
    }
}
