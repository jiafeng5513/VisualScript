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

namespace DeepLearning.Visible
{
    public class Predict
    {
        private Predict() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ModelFile"></param>
        /// <param name="LabelFile"></param>
        /// <param name="inMat"></param>
        /// <returns></returns>
        public static string request(string ModelFile , string LabelFile , Mat inMat)
        {
            string currentDirectory = System.Environment.CurrentDirectory;
            //0.保存ModelFile,LabFile和InMat,序列化存储
            ExplodeProto pass=new ExplodeProto();
            pass.ModelFile = ModelFile;
            pass.LabelFile = LabelFile;
            pass.inMat = inMat;
            string jsonForSave= JsonConvert.SerializeObject(pass);

            FileStream fs = File.Create(currentDirectory + "\\ModelDecompilerParams.json");

            StreamWriter sw = new StreamWriter(fs);
            sw.Write(jsonForSave);
            sw.Close();

            //1.启动TensorServer

            currentDirectory += "//TensorServer.exe";
            System.Diagnostics.Process.Start(currentDirectory);
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

            _msg.imgIN = inMat;


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
                    if (receive == "crash")
                    {
                        bw.Write("stop");
                        run = false;
                        returnStr = "Server Crash with code 001!";
                        return returnStr;
                    }
                    TensorProto result = JsonConvert.DeserializeObject<TensorProto>(receive);
                    returnStr = result.outputString;
                    bw = new BinaryWriter(clientStream);
                    bw.Write("stop");
                    run = false;
                }
                catch
                {
                    //MessageBox.Show("接收失败！");
                    bw.Write("stop");
                    run = false;
                    returnStr = "Server Crash with code 002!";
                }
            }

            return returnStr;
        }
    }
}
