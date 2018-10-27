using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Structure;


namespace DeepLearning.Visible
{
    /// <summary>
    /// 将CV.Mat表示的图片转换为Tensor
    /// </summary>
    //public class ImgToTensor
    //{
    //    /// <summary>
    //    /// TFDataType.Float
    //    /// </summary>
    //    /// <returns></returns>
    //    public static TFDataType FLoat()
    //    {
    //        //float
    //        return TFDataType.Float;
    //    }
    //    /// <summary>
    //    /// TFDataType.Double
    //    /// </summary>
    //    /// <returns></returns>
    //    public static TFDataType Double()
    //    {
    //        //double
    //        return TFDataType.Double;
    //    }
    //    /// <summary>
    //    /// TFDataType.UInt8
    //    /// </summary>
    //    /// <returns></returns>
    //    public static TFDataType UInt8()
    //    {
    //        //byte
    //        return TFDataType.UInt8;
    //    }
    //    /// <summary>
    //    /// Convert Mat To Tensor
    //    /// </summary>
    //    /// <param name="imgMat"></param>
    //    /// <param name="TYPE"></param>
    //    /// <returns></returns>
    //    public static TFTensor MatToTensor(Mat imgMat, TFDataType TYPE)
    //    {
    //        switch (TYPE)
    //        {
    //            case TFDataType.Float:
    //                return MatToTensorFLoat(imgMat);
    //            case TFDataType.Double:
    //                //return MatToTensorDouble(imgMat);
    //                break;
    //            case TFDataType.UInt8:
    //                //return MatToTensorUInt8(imgMat);
    //                break;
    //        }
    //        return null;
    //    }
    //    /// <summary>
    //    /// 由于模板失效,这里只能这么写
    //    /// </summary>
    //    /// <param name="imgMat"></param>
    //    /// <returns></returns>
    //    private static TFTensor MatToTensorFLoat(Mat imgMat)
    //    {
    //        //图像矩阵
    //        float[,,] imgcontent = new float[imgMat.Height, imgMat.Width, imgMat.NumberOfChannels];
    //        //数据转移
    //        switch (imgMat.NumberOfChannels)
    //        {
    //            case 1:
    //                //单通道
    //                float[] temp = new float[imgMat.Height * imgMat.Width];
    //                Marshal.Copy(imgMat.DataPointer, temp, 0, imgMat.Height * imgMat.Width);
    //                float[,,,] contentExpanded = new float[1, imgMat.Height, imgMat.Width, 1];//最后一个数是通道数
    //                for (int i = 0; i < imgMat.Height; i++)
    //                {
    //                    for (int j = 0; j < imgMat.Width; j++)
    //                    {
    //                        //[0,255]-->[0,1]
    //                        var x = temp[i * imgMat.Height + j];
    //                        imgcontent[i, j, 0] = (float)((255 - x) * 1.0 / 255.0);
    //                    }
    //                }
    //                Buffer.BlockCopy(imgcontent, 0, contentExpanded, 0, imgcontent.Length * sizeof(float));
    //                var tensor = new TFTensor(contentExpanded);
    //                return tensor;
    //                break;
    //            case 3:
    //                //bgr三通道
    //                //Image<Bgr, Byte> image = imgMat.ToImage<Bgr, Byte>();
    //                //var p = image[0, 0];
    //                break;
    //            case 4:
    //                //bgra四通道
    //                break;
    //        }
    //        //图像增广矩阵
    //        return null;
    //    }
    //}
}
