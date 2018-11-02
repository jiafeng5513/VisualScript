using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Newtonsoft.Json;
using TensorFlow;
using TensorProtocol;

namespace TensorServer
{
    class Program
    {
        /// <summary>
        /// 启动一个服务器,等待客户端发来的张量计算请求,解析请求并分发到具体函数中计算,将计算结果返回
        /// 客户端收到计算结果后,向服务器发送退出指令,服务器关闭
        /// </summary>
        private const int LOCAL_PORT = 8688;
        private TcpListener _listener = null;
        private TcpListener myListener;
        private TcpClient newClient;
        public BinaryReader br;
        public BinaryWriter bw;
        private bool run = true;

        /// <summary>
        /// 服务器监听过程
        /// </summary>
        public void ListenProcess()
        {
            myListener = new TcpListener(IPAddress.Any, LOCAL_PORT); //创建TcpListener实例
            myListener.Start(); //start
            newClient = myListener.AcceptTcpClient(); //等待客户端连接
            //客户端连接成功后往下执行
            NetworkStream clientStream;
            Console.WriteLine("连接成功");
            while (run)
            {
                try
                {
                    clientStream = newClient.GetStream(); //利用TcpClient对象GetStream方法得到网络流

                    br = new BinaryReader(clientStream);
                    string receive = null;
                    receive = br.ReadString(); //读取
                    if (receive == "stop")
                    {
                        run = false;
                        Console.WriteLine("收到命令,关闭");
                        break;
                    }


                    TensorProto stu2 = JsonConvert.DeserializeObject<TensorProto>(receive);
                    //TODO:如果要处理多种任务,在此处进行任务分发
                    var mmm = PredictFromImage(stu2);

                    bw = new BinaryWriter(clientStream);
                    //向客户端回传计算结果
                    bw.Write(mmm);

                }
                catch
                {
                    Console.WriteLine("接收失败");
                    clientStream = newClient.GetStream();
                    bw = new BinaryWriter(clientStream);
                    //向客户端回传计算结果
                    bw.Write("crash");
                }
            }
        }
        /// <summary>
        /// 使用预训练网络从图片推理结论
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string PredictFromImage(TensorProto msg)
        {
            using (var graph = new TFGraph())
            {
                graph.Import(File.ReadAllBytes(msg.pbFileName));
                var session = new TFSession(graph);
                var runner = session.GetRunner();

                var tensor = CreateTensorFromMat(msg.imgIN);


                runner.AddInput(graph["input"][0], tensor);
                runner.Fetch(graph["out"][0]);

                var output = runner.Run();

                // Fetch the results from output:
                TFTensor result = output[0];
                var probabilities = ((float[][])result.GetValue(jagged: true))[0];
                var bestIdx = 0;
                float p = 0, best = 0;
                for (int i = 0; i < probabilities.Length; i++)
                {
                    if (probabilities[i] > best)
                    {
                        bestIdx = i;
                        best = probabilities[i];
                    }
                }
                TensorProto returnvalue = new TensorProto();
                returnvalue.outputString =
                    "result is :" + msg.Labels[bestIdx] + "\n with " + probabilities[bestIdx]+ "of probability";

                string returnjson = JsonConvert.SerializeObject(returnvalue);

                return returnjson;
            }
        }
        /// <summary>
        /// Mat To TFTensor Converter
        /// </summary>
        /// <param name="imgMat"></param>
        /// <returns></returns>
        public static TFTensor CreateTensorFromMat(Mat imgMat)
        {
            //图像矩阵
            float[,,] imgcontent = new float[imgMat.Height, imgMat.Width, imgMat.NumberOfChannels];
            //数据转移
            switch (imgMat.NumberOfChannels)
            {
                case 1:
                    //单通道
                    float[] temp = new float[imgMat.Height * imgMat.Width];
                    Marshal.Copy(imgMat.DataPointer, temp, 0, imgMat.Height * imgMat.Width);
                    float[,,,] contentExpanded = new float[1, imgMat.Height, imgMat.Width, 1];//最后一个数是通道数
                    for (int i = 0; i < imgMat.Height; i++)
                    {
                        for (int j = 0; j < imgMat.Width; j++)
                        {
                            //tva = [(255 - x) * 1.0 / 255.0 for x in tv]
                            var x = temp[i * imgMat.Height + j];
                            imgcontent[i, j, 0] = (float)((255 - x) * 1.0 / 255.0);
                        }
                    }
                    Buffer.BlockCopy(imgcontent, 0, contentExpanded, 0, imgcontent.Length * sizeof(float));
                    var tensor = new TFTensor(contentExpanded);
                    return tensor;
                    break;
                case 3:
                    //bgr三通道
                    //Image<Bgr, Byte> image = imgMat.ToImage<Bgr, Byte>();
                    //var p = image[0, 0];
                    break;
                case 4:
                    //bgra四通道
                    break;
            }
            //图像增广矩阵
            return null;
        }

        static void Main(string[] args)
        {
            Program m_server = new Program();
            m_server.ListenProcess();
            Console.WriteLine("监听退出,服务器关闭");
        }
    }
}
