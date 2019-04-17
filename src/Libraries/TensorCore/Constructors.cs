using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNTK;

namespace TensorCore
{
    /*
     * Constructors for inner types
     */
    public class Constructors
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static StreamConfiguration CreateStreamConfiguration(string name,int size)
        {
            return new StreamConfiguration(name, size);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="streamConfigurations"></param>
        /// <returns></returns>
        public static MinibatchSource CreateMinibatchSource(string filepath, IList<StreamConfiguration> streamConfigurations)
        {
            return MinibatchSource.TextFormatMinibatchSource(filepath, streamConfigurations);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapFilePath"></param>
        /// <param name="meanFilePath"></param>
        /// <param name="imageDims"></param>
        /// <param name="numClasses"></param>
        /// <param name="maxSweeps"></param>
        /// <returns></returns>
        public static MinibatchSource CreateMinibatchSource(string mapFilePath, string meanFilePath,
            int[] imageDims, int numClasses, uint maxSweeps)
        {
            List<CNTKDictionary> transforms = new List<CNTKDictionary>
            {
                CNTKLib.ReaderCrop("RandomSide",
                    new Tuple<int, int>(0, 0),
                    new Tuple<float, float>(0.8f, 1.0f),
                    new Tuple<float, float>(0.0f, 0.0f),
                    new Tuple<float, float>(1.0f, 1.0f),
                    "uniRatio"),CNTKLib.ReaderScale(imageDims[0], imageDims[1], imageDims[2]),
                CNTKLib.ReaderMean(meanFilePath)
            };

            var deserializerConfiguration = CNTKLib.ImageDeserializer(mapFilePath,
                "labels", (uint)numClasses,
                "features",
                transforms);

            MinibatchSourceConfig config =
                new MinibatchSourceConfig(new List<CNTKDictionary> { deserializerConfiguration })
                {
                    MaxSweeps = maxSweeps
                };

            return CNTKLib.CreateCompositeMinibatchSource(config);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Variable CreateInputVariable(NDShape shape, DataType type, string name)
        {
            return CNTKLib.InputVariable(shape, type, name);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static Variable CreateScalar(float value, DeviceDescriptor device)
        {
            return Constant.Scalar<float>(value, device);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        /// <param name="dataType"></param>
        /// <param name="cntkDictionary"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static Variable CreateParameter(NDShape shape, DataType dataType, CNTKDictionary cntkDictionary,
            DeviceDescriptor device)
        {
            return new Variable(new Parameter(shape, DataType.Float,cntkDictionary, device));
        }

        public static Variable CreateParameter(NDShape shape, float initValue, DeviceDescriptor device, string name="")
        {
            return new Variable(new Parameter(shape, initValue, device, name));
        }
        /// <summary>
        /// 学习率
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minibatchsize"></param>
        /// <returns></returns>
        public static TrainingParameterScheduleDouble CreateTrainingRate(double value,int minibatchsize)
        {
            return new TrainingParameterScheduleDouble(value, (uint)minibatchsize);
        }

        //public static NDShape CreateNdShape(int[] dims)
        //{
        //    return (NDShape)(dims);
        //}

        public static NDShape CreateNdShape(List<Object>dims)
        {
            int[]dimInts=new int[dims.Count];
            for (int i = 0; i < dims.Count; i++)
            {
                dimInts[i] = dims.IndexOf(i);
            }

            return (NDShape)(dimInts);
        }


    }
}
