using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNTK;

namespace TensorCore
{
    public class TensorFunction
    {
        public static Function Negate(Variable operand, string name = "")
        {
            return CNTKLib.Negate(operand, name);
        }

        public static Function Sigmoid(Variable operand, string name = "")
        {
            return CNTKLib.Sigmoid(operand, name);
        }

        public static Function Tanh(Variable operand, string name)
        {
            return CNTKLib.Tanh(operand, name);
        }

        public static Function Asin(Variable operand, string name = "")
        {
            return CNTKLib.Asin(operand, name);
        }

        public static Function Sin(Variable operand, string name = "")
        {
            return CNTKLib.Sin(operand, name);
        }

        public static Function Acos(Variable operand, string name = "")
        {
            return CNTKLib.Acos(operand, name);
        }

        public static Function Cos(Variable operand, string name = "")
        {
            return CNTKLib.Cos(operand, name);
        }

        public static Function Cosh(Variable operand, string name = "")
        {
            return CNTKLib.Cosh(operand, name);
        }

        public static Function Sinh(Variable operand, string name = "")
        {
            return CNTKLib.Sinh(operand, name);
        }

        public static Function ReLU(Variable operand, string name = "")
        {
            return CNTKLib.ReLU(operand, name);
        }

        public static Function Exp(Variable operand, string name = "")
        {
            return CNTKLib.Exp(operand, name);
        }

        public static Function Log(Variable operand, string name = "")
        {
            return CNTKLib.Log(operand, name);
        }

        public static Function Square(Variable operand, string name = "")
        {
            return CNTKLib.Square(operand, name);
        }

        public static Function Sqrt(Variable operand, string name = "")
        {
            return CNTKLib.Sqrt(operand, name);
        }

        public static Function Softmax(Variable operand, Axis axis, string name = "")
        {
            return CNTKLib.Softmax(operand, axis, name);
        }

        public static Function Hardmax(Variable operand, string name = "")
        {
            return CNTKLib.Hardmax(operand, name);
        }

        public static Function Transpose(Variable operand, string name = "")
        {
            return CNTKLib.Transpose(operand, name);
        }

        public static Function Dropout(Variable operand, double dropoutRate, uint seed, string name = "")
        {
            return CNTKLib.Dropout(operand, dropoutRate, seed, name);
        }

        public static Function Reshape(Variable operand, NDShape replacementShape, Axis beginAxis, Axis endAxis,
            string name = "")
        {
            return CNTKLib.Reshape(operand, replacementShape, beginAxis, endAxis, name);
        }

        public static Function Times(Variable leftOperand, Variable rightOperand, string name = "")
        {
            return CNTKLib.Times(leftOperand, rightOperand, name);
        }

        public static Function TransposeTimes(Variable leftOperand, Variable rightOperand, uint outputRank,
            string name = "")
        {
            return CNTKLib.TransposeTimes(leftOperand, rightOperand, outputRank, name);
        }

        public static Function Plus(Variable leftOperand, Variable rightOperand, string name = "")
        {
            return CNTKLib.Plus(leftOperand, rightOperand, name);
        }

        public static Function Minus(Variable leftOperand, Variable rightOperand, string name = "")
        {
            return CNTKLib.Minus(leftOperand, rightOperand, name);
        }

        public static Function LogAddExp(Variable leftOperand, Variable rightOperand, string name = "")
        {
            return CNTKLib.LogAddExp(leftOperand, rightOperand, name);
        }

        public static Function Pow(Variable leftOperand, Variable rightOperand, string name = "")
        {
            return CNTKLib.Pow(leftOperand, rightOperand, name);
        }

        public static Function ElementTimes(Variable leftOperand, Variable rightOperand, string name = "")
        {
            return CNTKLib.ElementTimes(leftOperand, rightOperand, name);
        }

        public static Function ElementDivide(Variable leftOperand, Variable rightOperand, string name = "")
        {
            return CNTKLib.ElementDivide(leftOperand, rightOperand, name);
        }

        public static Function CosineDistance(Variable leftOperand, Variable rightOperand, string name = "")
        {
            return CNTKLib.CosineDistance(leftOperand, rightOperand, name);
        }

        public static Function CosineDistanceWithNegativeSamples(Variable leftOperand, Variable rightOperand,
            uint shiftWindow, uint numberOfNegativeSamples, string name = "")
        {
            return CNTKLib.CosineDistanceWithNegativeSamples(leftOperand, rightOperand, shiftWindow, numberOfNegativeSamples, name);
        }

        public static Function BinaryCrossEntropy(Variable prediction, Variable targets, string name = "")
        {
            return CNTKLib.BinaryCrossEntropy(prediction, targets, name);
        }

        public static Function WeightedBinaryCrossEntropy(Variable prediction, Variable targets, Variable weights,
            string name = "")
        {
            return CNTKLib.WeightedBinaryCrossEntropy(prediction, targets, weights, name);
        }

        public static Function CrossEntropyWithSoftmax(Variable prediction, Variable labels, Axis axis,
            string name = "")
        {
            return CNTKLib.CrossEntropyWithSoftmax(prediction, labels, axis, name);
        }

        public static Function ClassificationError(Variable prediction, Variable labels, uint topN, Axis axis,
            string name = "")
        {
            return CNTKLib.ClassificationError(prediction, labels, topN, axis, name);
        }

        public static Function PastValue(Variable operand, Variable initialState, uint offset = 1, string name = "")
        {
            return CNTKLib.PastValue(operand, initialState, offset, name);
        }

        public static Function FutureValue(Variable operand, Variable initialState, uint offset = 1, string name = "")
        {
            return CNTKLib.PastValue(operand, initialState, offset, name);
        }
        //多个重载
        public static Function Convolution(Variable convolutionMap, Variable operand, NDShape strides ,
            BoolVector sharing , BoolVector autoPadding,NDShape dilation )
        {
            return CNTKLib.Convolution(convolutionMap, operand,strides,
            sharing, autoPadding, dilation);
        }

        public static Function ROIPooling(Variable operand, Variable rois, PoolingType poolingType,
            NDShape roiOutputShape, double spatialScale, string name = "")
        {
            return CNTKLib.ROIPooling(operand, rois, poolingType,roiOutputShape, spatialScale,name);
        }

        //多个重载
        public static Function Pooling(Variable operand, PoolingType poolingType, NDShape poolingWindowShape, NDShape strides,
            BoolVector autoPadding,bool ceilOutDim = false,bool includePad = false,string name = "")
        {
            return CNTKLib.Pooling(operand,poolingType, poolingWindowShape, strides, autoPadding, ceilOutDim , includePad, name);
        }

        //多个重载
        public static Function BatchNormalization(Variable operand, Variable scale, Variable bias, Variable runningMean,
            Variable runningInvStd, Variable runningCount, bool spatial, double normalizationTimeConstant = 0,
            double blendTimeConstant = 0, double epsilon = 0.00001, bool useCuDNNEngine = true, string name = "")
        {
            return CNTKLib.BatchNormalization(operand,scale, bias, runningMean,runningInvStd, runningCount, 
                spatial, normalizationTimeConstant,blendTimeConstant, epsilon, useCuDNNEngine);
        }

        public static Function Splice(VariableVector operands, Axis axis, string name = "")
        {
            return CNTKLib.Splice(operands, axis, name);
        }

        public static Function Combine(VariableVector operands, string name = "")
        {
            return CNTKLib.Combine(operands, name);
        }

        public static Function LeakyReLU(Variable operand, double alpha, string name = "")
        {
            return CNTKLib.LeakyReLU(operand, alpha, name);
        }
    }
}
