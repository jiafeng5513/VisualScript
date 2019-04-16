using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNTK;

namespace TensorCore
{
    public class BuildInLayers
    {
        public static Function ConvBatchNormalizationLayer(Variable input, int outFeatureMapCount, 
            int kernelWidth, int kernelHeight, int hStride, int vStride,
            double wScale, double bValue, double scValue, int bnTimeConst, bool spatial, DeviceDescriptor device)
        {
            int numInputChannels = input.Shape[input.Shape.Rank - 1];

            var convParams = new Parameter(new int[] { kernelWidth, kernelHeight, numInputChannels, outFeatureMapCount },
                DataType.Float, CNTKLib.GlorotUniformInitializer(wScale, -1, 2), device);
            var convFunction = CNTKLib.Convolution(convParams, input, new int[] { hStride, vStride, numInputChannels });

            var biasParams = new Parameter(new int[] { NDShape.InferredDimension }, (float)bValue, device, "");
            var scaleParams = new Parameter(new int[] { NDShape.InferredDimension }, (float)scValue, device, "");
            var runningMean = new Constant(new int[] { NDShape.InferredDimension }, 0.0f, device);
            var runningInvStd = new Constant(new int[] { NDShape.InferredDimension }, 0.0f, device);
            var runningCount = Constant.Scalar(0.0f, device);
            return CNTKLib.BatchNormalization(convFunction, scaleParams, biasParams, runningMean, runningInvStd, runningCount,
                spatial, (double)bnTimeConst, 0.0, 1e-5 /* epsilon */);
        }

        public static Function ProjectLayer(Variable wProj, Variable input, int hStride, int vStride, double bValue, double scValue, int bnTimeConst,
            DeviceDescriptor device)
        {
            int outFeatureMapCount = wProj.Shape[0];
            var b = new Parameter(new int[] { outFeatureMapCount }, (float)bValue, device, "");
            var sc = new Parameter(new int[] { outFeatureMapCount }, (float)scValue, device, "");
            var m = new Constant(new int[] { outFeatureMapCount }, 0.0f, device);
            var v = new Constant(new int[] { outFeatureMapCount }, 0.0f, device);

            var n = Constant.Scalar(0.0f, device);

            int numInputChannels = input.Shape[input.Shape.Rank - 1];

            var c = CNTKLib.Convolution(wProj, input, new int[] { hStride, vStride, numInputChannels }, new bool[] { true }, new bool[] { false });
            return CNTKLib.BatchNormalization(c, sc, b, m, v, n, true /*spatial*/, (double)bnTimeConst, 0, 1e-5, false);
        }

        public static Function ResNetNode(Variable input, int outFeatureMapCount, int kernelWidth, int kernelHeight,
            double wScale, double bValue,
            double scValue, int bnTimeConst, bool spatial, DeviceDescriptor device)
        {
            var c1 = ConvBatchNormalizationReLULayer(input, outFeatureMapCount, kernelWidth, kernelHeight, 1, 1, wScale, bValue, scValue, bnTimeConst, spatial, device);
            var c2 = ConvBatchNormalizationLayer(c1, outFeatureMapCount, kernelWidth, kernelHeight, 1, 1, wScale, bValue, scValue, bnTimeConst, spatial, device);
            var p = CNTKLib.Plus(c2, input);
            return CNTKLib.ReLU(p);
        }

        public static Function ResNetNode(Variable input, int outFeatureMapCount,bool spatial)
        {
            DeviceDescriptor device = DeviceDescriptor.GPUDevice(0);
            int kernelWidth = 3;
            int kernelHeight = 3;
            double wScale= 7.07;
            double bValue = 0;
            double scValue=1;
            int bnTimeConst=4096;

            var c1 = ConvBatchNormalizationReLULayer(input, outFeatureMapCount, kernelWidth, kernelHeight, 1, 1, wScale, bValue, scValue, bnTimeConst, spatial, device);
            var c2 = ConvBatchNormalizationLayer(c1, outFeatureMapCount, kernelWidth, kernelHeight, 1, 1, wScale, bValue, scValue, bnTimeConst, spatial, device);
            var p = CNTKLib.Plus(c2, input);
            return CNTKLib.ReLU(p);
        }

        public static Function ResNetNodeInc(Variable input, int outFeatureMapCount, int kernelWidth, int kernelHeight, double wScale, double bValue,
            double scValue, int bnTimeConst, bool spatial, Variable wProj, DeviceDescriptor device)
        {
            var c1 = ConvBatchNormalizationReLULayer(input, outFeatureMapCount, kernelWidth, kernelHeight, 2, 2, wScale, bValue, scValue, bnTimeConst, spatial, device);
            var c2 = ConvBatchNormalizationLayer(c1, outFeatureMapCount, kernelWidth, kernelHeight, 1, 1, wScale, bValue, scValue, bnTimeConst, spatial, device);

            var cProj = ProjectLayer(wProj, input, 2, 2, bValue, scValue, bnTimeConst, device);

            var p = CNTKLib.Plus(c2, cProj);
            return CNTKLib.ReLU(p);
        }

        public static Function ResNetNodeInc(Variable input, int outFeatureMapCount, bool spatial, Variable wProj)
        {
            DeviceDescriptor device = DeviceDescriptor.GPUDevice(0);
            int kernelWidth = 3;
            int kernelHeight = 3;
            double wScale = 7.07;
            double bValue = 0;
            double scValue = 1;
            int bnTimeConst = 4096;
            var c1 = ConvBatchNormalizationReLULayer(input, outFeatureMapCount, kernelWidth, kernelHeight, 2, 2, wScale, bValue, scValue, bnTimeConst, spatial, device);
            var c2 = ConvBatchNormalizationLayer(c1, outFeatureMapCount, kernelWidth, kernelHeight, 1, 1, wScale, bValue, scValue, bnTimeConst, spatial, device);

            var cProj = ProjectLayer(wProj, input, 2, 2, bValue, scValue, bnTimeConst, device);

            var p = CNTKLib.Plus(c2, cProj);
            return CNTKLib.ReLU(p);
        }

        private static Function ConvBatchNormalizationReLULayer(Variable input, int outFeatureMapCount, int kernelWidth, int kernelHeight, int hStride, int vStride,
            double wScale, double bValue, double scValue, int bnTimeConst, bool spatial, DeviceDescriptor device)
        {
            var convBNFunction = ConvBatchNormalizationLayer(input, outFeatureMapCount, kernelWidth, kernelHeight, hStride, vStride, wScale, bValue, scValue, bnTimeConst, spatial, device);
            return CNTKLib.ReLU(convBNFunction);
        }

    }
}
