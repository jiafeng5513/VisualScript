using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TensorFlow;

namespace TFSharpTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var graph = new TFGraph())
            {
                graph.Import(File.ReadAllBytes("E:\\VisualStudio\\VisualScript\\utils\\mnist\\out_without_dropout\\model\\model_minimal.pb"));
                var session = new TFSession(graph);
                var runner = session.GetRunner();


                runner.AddInput(graph["input"][0], tensor);
                runner.Fetch(graph["output"][0]);

                var output = runner.Run();

                // Fetch the results from output:
                TFTensor result = output[0];
            }
        }
    }
}
