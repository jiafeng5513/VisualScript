using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;

namespace TensorProtocol
{
    [Serializable]
    public class TensorProto
    {
        public enum MessageTpye
        {
            ask,//客户端发往服务器
            answer//服务器发往客户端
        };

        public MessageTpye m_MessageTpye;

        public Mat imgIN = new Mat();
        public Mat imgOUT = new Mat();

        public string[] Labels;

        public string pbFileName = string.Empty;

        public string outputString = string.Empty;
    }
}
