using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace TensorProtocol
{
    /// <summary>
    /// 用于序列化传递网络预测的参数到网络展开
    /// </summary>
    [Serializable]
    public class ExplodeProto
    {
        public string ModelFile = string.Empty;
        public string LabelFile = string.Empty;
        public Mat inMat;
    }
}
