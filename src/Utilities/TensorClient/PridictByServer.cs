using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using TensorProtocol;

namespace TensorClient
{
    public class PridictByServer
    {
        public static string request(string ModelFile = " ", string LabelFile = " ", string inputFile = " ")
        {
            //1.启动TensorServer
            string str = System.Environment.CurrentDirectory;
            str += "//TensorServer.exe";
            System.Diagnostics.Process.Start(str);
            //2.启动TCP客户端
            bool run = true;
            BinaryReader br;
            TcpClient client = new TcpClient("127.0.0.1", 8688);

            NetworkStream clientStream = client.GetStream();

            BinaryWriter bw = new BinaryWriter(clientStream);

            TensorProto _msg = new TensorProto();
            _msg.Labels = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };


            _msg.m_MessageTpye = TensorProto.MessageTpye.ask;
            _msg.pbFileName = ModelFile;
            Mat imgMat0 = CvInvoke.Imread(inputFile);
            Image <Gray, Single> img2 = imgMat0.ToImage<Gray, Single>();
            _msg.imgIN = img2.Mat;


            string json1 = JsonConvert.SerializeObject(_msg);
            bw.Write(json1);
            //textBox.Text += "向服务器发送:\r\n" + json1;
            string returnStr = string.Empty;
            while (run)
            {
                try
                {
                    clientStream = client.GetStream();
                    br = new BinaryReader(clientStream);
                    string receive = null;

                    receive = br.ReadString();
                    TensorProto result = JsonConvert.DeserializeObject<TensorProto>(receive);
                    returnStr = result.outputString;
                    bw = new BinaryWriter(clientStream);
                    bw.Write("stop");
                    run = false;
                }
                catch
                {
                    //MessageBox.Show("接收失败！");
                }
            }

            return returnStr;
        }
    }
}
