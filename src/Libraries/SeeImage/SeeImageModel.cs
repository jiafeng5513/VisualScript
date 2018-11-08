using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dynamo.Utilities;
using ModelAnalyzer.Properties;
using ProtoCore.AST.AssociativeAST;
using System.Xml;
using Dynamo.Engine;
using Dynamo.Graph;
using Dynamo.Graph.Nodes;
using Dynamo.Scheduler;
using Dynamo.Visualization;
using Emgu.CV;
using Newtonsoft.Json;
/*
* 带客制化界面的元素的节点API
* 1.输出到bin/node中.
* 2.不需要在PathResolvers.cs中加载DLL,node目录中的dll是由NodeModelAssemblyLoader.cs加载的
* 3.与zerotouchlibrary的编写有较大的区别,不是通过函数的访问控制符来控制Node是否出现在list中的.
* 4.属性说明:
*       [NodeName("ExportToSAT")]Node的名字
*/
namespace ModelAnalyzerUI
{
    [NodeCategory("Core.View")]
    [NodeName("See Image")]
    //[InPortTypes("string")]
    [NodeDescription("ExportToSATDescripiton", typeof(Resources))]
    [NodeSearchTags("ExportWithUnitsSearchTags", typeof(Resources))]
    [IsDesignScriptCompatible]
    public class SeeImageModel : NodeModel
    {

        [JsonConstructor]
        private SeeImageModel(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
            //TODO:内部变量的初始化
            ShouldDisplayPreviewCore = true;
        }

        public SeeImageModel()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("image", "image to show")));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("image", "same as image to show")));

            ShouldDisplayPreviewCore = true;
            RegisterAllPorts();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputAstNodes"></param>
        /// <returns></returns>
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            yield return AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), inputAstNodes[0]);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="scheduler"></param>
        /// <param name="engine"></param>
        /// <param name="factory"></param>
        /// <param name="forceUpdate"></param>
        /// <returns></returns>
        public override bool RequestVisualUpdateAsync(
            IScheduler scheduler, EngineController engine, IRenderPackageFactory factory, bool forceUpdate = false)
        {
            //Do nothing
            return false;
        }




        #region 重载:序列化和解序列化方法
        protected override void SerializeCore(XmlElement element, SaveContext context)
        {
            base.SerializeCore(element, context); // Base implementation must be called.

            //var helper = new XmlElementHelper(element);
            //helper.SetAttribute("ExportableNodeSource","");//把slider的值存起来
        }

        protected override void DeserializeCore(XmlElement element, SaveContext context)
        {
            base.DeserializeCore(element, context); //Base implementation must be called.
            var helper = new XmlElementHelper(element);
            //var exportedUnit = helper.ReadString("ExportableNodeSource","");
            //ExportableNodeSource = new ObservableCollection<string>();
            //valueofslider = int.Parse(exportedUnit) is int ? int.Parse(exportedUnit) : 0;
        }

        #endregion
    }
}

