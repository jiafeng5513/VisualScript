using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using DeepLearning.Visible;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.TF;
using Emgu.TF.Models;
using Newtonsoft.Json;
using TensorProtocol;

namespace DeepLearning
{
    public class Example
    {
        private Example() { }

        //public static string Predict(string ModelFile = " ", string LabelFile=" ", string inputFile=" ")
        //{
        //    ModelDecompiler.ParamGet(ModelFile, LabelFile, inputFile);
        //    ModelDeploy modelDecoderGraph = new ModelDeploy(  );
        //    modelDecoderGraph.Init(new string[] { ModelFile, LabelFile }, "Mul", "final_result");

        //    Tensor imageTensor = ImageIO.ReadTensorFromImageFile(inputFile, 299, 299, 128.0f, 1.0f / 128.0f);
        //    //modelDecoderGraph.ImportGraph();
        //    Stopwatch sw = Stopwatch.StartNew();
        //    float[] probability = modelDecoderGraph.Recognize(imageTensor);
        //    sw.Stop();

        //    String resStr = String.Empty;
        //    if (probability != null)
        //    {
        //        String[] labels = modelDecoderGraph.Labels;
        //        float maxVal = 0;
        //        int maxIdx = 0;
        //        for (int i = 0; i < probability.Length; i++)
        //        {
        //            if (probability[i] > maxVal)
        //            {
        //                maxVal = probability[i];
        //                maxIdx = i;
        //            }
        //        }
        //        resStr = String.Format("Object is {0} with {1}% probability. \n Recognition done in {2} milliseconds.", labels[maxIdx], maxVal * 100, sw.ElapsedMilliseconds);
        //    }

        //    return resStr;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ModelFile"></param>
        /// <param name="LabelFile"></param>
        /// <param name="inputFile"></param>
        /// <returns></returns>
        public static string Predict(string ModelFile = " ", string LabelFile = " ", string inputFile = " ")
        {
            ModelDecompiler.ParamGet(ModelFile, LabelFile, inputFile);
            ModelDeploy modelDecoderGraph = new ModelDeploy();
            modelDecoderGraph.Init(new string[] { ModelFile, LabelFile }, "input", "out");
            
            Tensor imageTensor = ImageIO.ReadTensorFromImageFile(inputFile, 28, 28, 0, 1/255f);
            
            //modelDecoderGraph.ImportGraph();
            Stopwatch sw = Stopwatch.StartNew();
            float[] probability = modelDecoderGraph.Recognize(imageTensor);
            sw.Stop();

            String resStr = String.Empty;
            if (probability != null)
            {
                String[] labels = modelDecoderGraph.Labels;
                float maxVal = 0;
                int maxIdx = 0;
                for (int i = 0; i < probability.Length; i++)
                {
                    if (probability[i] > maxVal)
                    {
                        maxVal = probability[i];
                        maxIdx = i;
                    }
                }
                resStr = String.Format("Object is {0} with {1}% probability. \n Recognition done in {2} milliseconds.", labels[maxIdx], maxVal * 100, sw.ElapsedMilliseconds);
            }

            return resStr;
        }

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
            Image<Gray, Single> img2 = imgMat0.ToImage<Gray, Single>();
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
        public static string TestFunc(string a, string b, string c)
        {
            return "Fuckiing succeed!";
        }
    }
}
