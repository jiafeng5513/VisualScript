using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNTK;

namespace TensorCore
{
    /*
     * 枚举.常量和属性的封装
     */
    public class Constants
    {
#region DataType
        /// <summary>
        /// DataType.Double
        /// </summary>
        /// <returns></returns>
        public static DataType DataType_Double()
        {
            return DataType.Double;
        }
        /// <summary>
        /// DataType.Float
        /// </summary>
        /// <returns></returns>
        public static DataType DataType_Float()
        {
            return DataType.Float;
        }
        /// <summary>
        /// DataType.Float16
        /// </summary>
        /// <returns></returns>
        public static DataType DataType_Float16()
        {
            return DataType.Float16;
        }
        /// <summary>
        /// DataType.Int16
        /// </summary>
        /// <returns></returns>
        public static DataType DataType_Int16()
        {
            return DataType.Int16;
        }
        /// <summary>
        /// DataType.Int8
        /// </summary>
        /// <returns></returns>
        public static DataType DataType_Int8()
        {
            return DataType.Int8;
        }
        /// <summary>
        /// DataType.UChar
        /// </summary>
        /// <returns></returns>
        public static DataType DataType_UChar()
        {
            return DataType.UChar;
        }
        /// <summary>
        /// DataType.Unknown
        /// </summary>
        /// <returns></returns>
        public static DataType DataType_Unknown()
        {
            return DataType.Unknown;
        }
        #endregion

#region Device
        /// <summary>
        /// DeviceDescriptor.CPUDevice
        /// </summary>
        /// <returns></returns>
        public static DeviceDescriptor Device_CPU()
        {
            return DeviceDescriptor.CPUDevice;
        }
        /// <summary>
        /// DeviceDescriptor.GPUDevice(GpuID)
        /// </summary>
        /// <param name="GpuID"></param>
        /// <returns></returns>
        public static DeviceDescriptor Device_GPU(int GpuID)
        {
            return DeviceDescriptor.GPUDevice(GpuID);
        }
        /// <summary>
        /// DeviceDescriptor.AllDevices()
        /// </summary>
        /// <returns></returns>
        public static IList<DeviceDescriptor> Device_All()
        {
            return DeviceDescriptor.AllDevices();
        }

        #endregion

#region consts in CNTKLib
        /// <summary>
        /// CNTKLib.DefaultParamInitScale
        /// </summary>
        /// <returns></returns>
        public static int DefaultParamInitScale()
        {
            return CNTKLib.DefaultParamInitScale;
        }
        /// <summary>
        /// CNTKLib.SentinelValueForInferParamInitRank
        /// </summary>
        /// <returns></returns>
        public static int SentinelValueForInferParamInitRank()
        {
            return CNTKLib.SentinelValueForInferParamInitRank;
        }
        #endregion

#region consts in MiniBatchSource

        public static UInt64 InfinitelyRepeat()
        {
            return MinibatchSource.InfinitelyRepeat;
        }


        #endregion
    }


}
