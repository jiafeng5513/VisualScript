using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProtobufTools;

namespace ModelDecompileTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            ProtoTools.Decompile();
        }
    }
}
