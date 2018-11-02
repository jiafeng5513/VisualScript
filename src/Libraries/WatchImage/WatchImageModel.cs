using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreNodeModels.Properties;
using Dynamo.Engine;
using Dynamo.Graph.Nodes;
using Dynamo.Scheduler;
using Dynamo.Visualization;
using ProtoCore.AST.AssociativeAST;
using Newtonsoft.Json;
namespace WatchImage
{
    [NodeName("Watch Image")]
    [NodeDescription("WatchImageDescription", typeof(Resources))]
    [NodeCategory("Watch")]
    [NodeSearchTags("WatchImageSearchTags", typeof(Resources))]
    [IsDesignScriptCompatible]
    [OutPortTypes("var")]
    public class WatchImageModel: NodeModel
    {
        [JsonConstructor]
        private WatchImageModel(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
            ShouldDisplayPreviewCore = false;
        }

        public WatchImageModel()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("image", Resources.PortDataImageToolTip)));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("image", Resources.PortDataImageToolTip)));

            RegisterAllPorts();

            ShouldDisplayPreviewCore = false;
        }

        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {
            yield return AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), inputAstNodes[0]);
        }

        public override bool RequestVisualUpdateAsync(
            IScheduler scheduler, EngineController engine, IRenderPackageFactory factory, bool forceUpdate = false)
        {
            //Do nothing
            return false;
        }
    }
}
