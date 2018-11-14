using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
//from fo-Dicom
using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Render;
using Dicom.Media;
using Emgu.CV;
using Emgu.CV.Structure;

/*
 * 用于读取Dicom图片以及处理一些DICOM文件的相关操作
 */
namespace DicomTools
{
    public class DicomReader
    {
        /// <summary>
        /// 关闭合成构造方法
        /// </summary>
        private DicomReader(){}
        /// <summary>
        /// 读取Dicom图像
        /// </summary>
        /// <param name="filepath">dicom图像路径</param>
        /// <param name="windowWidth">double[]</param>
        /// <param name="windowCenter">double[]</param>
        /// <returns></returns>
        public static Mat ReadDicomFromPath(string filepath,double windowWidth,double windowCenter)
        {
            DicomFile _file = DicomFile.Open(filepath);
            bool _grayscale;
            double _windowWidth;
            double _windowCenter;

            DicomImage _image = new DicomImage(_file.Dataset);
            _image.WindowWidth = windowWidth;
            
            _image.WindowCenter = windowCenter;
            _grayscale = !_image.PhotometricInterpretation.IsColor;
            if (_grayscale)
            {
                _windowWidth = _image.WindowWidth;
                _windowCenter = _image.WindowCenter;
            }
            Image<Bgr, Byte> currentFrame = new Image<Bgr, byte>((_image.RenderImage(0)).AsBitmap());
            Mat invert = new Mat();
            CvInvoke.BitwiseAnd(currentFrame, currentFrame, invert);
            return invert;

        }
    }
}
