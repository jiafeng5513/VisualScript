using System;
using System.IO;
using System.Runtime.InteropServices;
using DeepLearning.Inside;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TensorFlow;

namespace TFSharpTest
{
    [TestClass]
    public class UnitTest1
    {
        public static TFTensor CreateTensorFromImage()
        {
            Mat imgMat0 = CvInvoke.Imread("E:\\VisualStudio\\VisualScript\\data\\mnist\\8.png");
            Image<Gray, Single> img2 = imgMat0.ToImage<Gray, Single>();
            var imgMat = img2.Mat;
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

        [TestMethod]
        public void TestMethod1()
        {

            using (var graph = new TFGraph())
            {
                graph.Import(File.ReadAllBytes(
                    "E:\\VisualStudio\\VisualScript\\utils\\mnist\\out_without_dropout\\model\\model_minimal.pb"));
                var session = new TFSession(graph);
                var runner = session.GetRunner();

                var tensor = CreateTensorFromImage();


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
                Console.WriteLine("result is :" + bestIdx + " with the probability of " + probabilities[bestIdx]);
            }

        }
    }
}
