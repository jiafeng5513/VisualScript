using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtobufTools;

namespace ModelDecompileTest
{
    [TestClass]
    public class UnitTest1
    {
        //[TestMethod]
        //public void TestMethod1()
        //{
        //    ProtoTools.Decompile();
        //}

        [TestMethod]
        public void TestMapCut()
        {
            ProtoTools m_testprotoTool=new ProtoTools();
            m_testprotoTool.Decompile("E:/VisualStudio/VisualScript/utils/mnist/out/model/saved_model.pb");

            foreach (var node in m_testprotoTool.Map)
            {
                Console.WriteLine(node.Value.Name);
                for (int i = 0; i < node.Value.Input.Count; i++)
                {
                    Console.WriteLine("---"+node.Value.Input[i]);
                }                
            }

            m_testprotoTool.MapCut();
            Console.WriteLine("经过图约减之后的情况");
            foreach (var node in m_testprotoTool.Map)
            {
                Console.WriteLine(node.Value.Name);
                for (int i = 0; i < node.Value.Input.Count; i++)
                {
                    Console.WriteLine("---" + node.Value.Input[i]);
                }
            }
        }
    }
}
