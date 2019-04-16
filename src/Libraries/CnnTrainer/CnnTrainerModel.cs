using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Dynamo.Utilities;
using ProtoCore.AST.AssociativeAST;
using System.Xml;
using CnnTrainer.Properties;
using Dynamo.Graph;
using Dynamo.Graph.Nodes;
using Dynamo.Graph.Nodes.CustomNodes;
using Emgu.CV;
using Newtonsoft.Json;
using TensorCore;
using CNTK;

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
    [NodeCategory("TensorCore")]
    [NodeName("CnnTrainer")]
    [InPortTypes(new string[]{"int[]","string", "CNTK.EFunction", "CNTK.DeviceDescriptor", "CNTK.MinibatchSource", "CNTK.TrainingParameterScheduleDouble" })]
    [NodeDescription("ExportToSATDescripiton", typeof(Resources))]
    [NodeSearchTags("ExportWithUnitsSearchTags", typeof(Resources))]
    [IsDesignScriptCompatible]
    public class CnnTrainerModel : NodeModel
    {
        private int numOfClasses = 0;
        private int miniBatchSize = 0;
        private int topN = 0;

        public int NumOfClasses
        {
            get => numOfClasses;
            set
            {
                numOfClasses = value;
                RaisePropertyChanged("NumOfClasses");
            }
        }
        public int MiniBatchSize
        {
            get => miniBatchSize;
            set
            {
                miniBatchSize = value;
                RaisePropertyChanged("MiniBatchSize");
            }
        }
        public int TopN
        {
            get => topN;
            set
            {
                topN = value;
                RaisePropertyChanged("TopN");
                this.OnNodeModified();
            }
        }


        [JsonConstructor]
        private CnnTrainerModel(IEnumerable<PortModel> inPorts, IEnumerable<PortModel> outPorts) : base(inPorts, outPorts)
        {
            ShouldDisplayPreviewCore = true;
            //TODO:内部变量的初始化
        }

        public CnnTrainerModel()
        {
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("Input Dim", "输入维度")));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("Model File", "模型文件")));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("Classifier", "分类器")));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("Device", "计算硬件")));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("Mini Batch Source", "")));
            InPorts.Add(new PortModel(PortType.Input, this, new PortData("Learning Rate", "学习率")));
            OutPorts.Add(new PortModel(PortType.Output, this, new PortData("Result", "are you ok?")));

            ShouldDisplayPreviewCore = true;
            RegisterAllPorts();
        }
        /// <summary>
        /// 构建输出节点
        /// 这个函数的触发条件是输入节点发生了变化,即this.OnNodeModified();
        /// </summary>
        /// <param name="inputAstNodes"></param>
        /// <returns></returns>
        public override IEnumerable<AssociativeNode> BuildOutputAst(List<AssociativeNode> inputAstNodes)
        {

            if (!InPorts[0].IsConnected || !InPorts[1].IsConnected || !InPorts[2].IsConnected || !InPorts[3].IsConnected
                || !InPorts[4].IsConnected || !InPorts[5].IsConnected)
            {
                //只有所有的输入节点都链接了,才能继续,否则构建一个默认输出
                return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), AstFactory.BuildBooleanNode(false)) };

            }

            //校验UI参数

            //Func调用,构造,
            /*
             * 把训练线程类设计成单例类,此处只是构建
             * 另一边从全局获取这个实例,然后run他
             * 重定向
             */

            var input1 = inputAstNodes[0];                          //m_imageDim
            var input2 = AstFactory.BuildIntNode(numOfClasses);     //m_numClasses,
            var input3 = AstFactory.BuildIntNode(topN);             //m_TopN,
            var input4 = inputAstNodes[1];                          //m_modelFile
            var input5 = AstFactory.BuildIntNode(miniBatchSize);    //m_minibatchSize
            var input6 = inputAstNodes[2];                          //m_classifierOutput
            var input7 = inputAstNodes[3];                          //m_device
            var input8 = inputAstNodes[4];                          //minibatchSource
            var input9 = inputAstNodes[5];                          //m_learningRatePerSample

            AssociativeNode node = null;

            //TensorCore.CnnTrainer temp =TensorCore.CnnTrainer.getInstance();

            node = AstFactory.BuildFunctionCall(
                        new Func<int[], int, int, string,int, CNTK.Function, CNTK.DeviceDescriptor, CNTK.MinibatchSource,
                            CNTK.TrainingParameterScheduleDouble,bool>(TensorCore.CnnTrainer.CnnTrainerInti),
                        new List<AssociativeNode> { input1, input2, input3,input4,input5,input6,input7,input8,input9});

            return new[] { AstFactory.BuildAssignment(GetAstIdentifierForOutputIndex(0), node) };
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