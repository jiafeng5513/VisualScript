using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CNTK;

namespace TensorCore
{
    /*
     * CnnTrainer for Encapsulating Training procedures
     */
    public class CnnTrainer
    {
        private static int[] m_imageDim;
        private static int m_numClasses;
        private static uint m_TopN;
        private static string m_modelFile;
        private static uint m_minibatchSize;
        private static Function m_classifierOutput;
        private static DeviceDescriptor m_device;
        private static MinibatchSource m_minibatchSource;
        private static TrainingParameterScheduleDouble m_learningRatePerSample;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageDim"></param>
        /// <param name="numClasses"></param>
        /// <param name="TopN"></param>
        /// <param name="modelFile"></param>
        /// <param name="minibatchSize"></param>
        /// <param name="classifierOutput"></param>
        /// <param name="device"></param>
        /// <param name="minibatchSource"></param>
        /// <param name="learningRatePerSample"></param>
        public static bool CnnTrainerInti(int[] imageDim, int numClasses, int TopN, 
            string modelFile, int minibatchSize, CNTK.Function classifierOutput,
            CNTK.DeviceDescriptor device, CNTK.MinibatchSource minibatchSource, 
            CNTK.TrainingParameterScheduleDouble learningRatePerSample)
        {
            m_imageDim = imageDim;
            m_numClasses = numClasses;
            m_TopN = (uint)TopN;
            m_modelFile = modelFile;
            m_minibatchSize = (uint)minibatchSize;
            m_classifierOutput = classifierOutput;
            m_device = device;
            m_minibatchSource = minibatchSource;
            m_learningRatePerSample = learningRatePerSample;
            return true;
        }

        /// <summary>
        /// CNN 训练入口
        /// </summary>
        /// <param name="minibatchSource"></param>
        /// <param name="imageDim">输入维度</param>
        /// <param name="numClasses">分类数量</param>
        /// <param name="TopN"></param>
        /// <param name="classifierOutput">CNN分类器</param>
        /// <param name="modelFile">模型文件保存路径</param>
        /// <param name="minibatchSize"></param>
        /// <param name="learningRatePerSample"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static void RunTraining()
        {

            // prepare training data

            var imageStreamInfo = m_minibatchSource.StreamInfo("features");
            var labelStreamInfo = m_minibatchSource.StreamInfo("labels");

            // build a model
            var imageInput = CNTKLib.InputVariable(m_imageDim, imageStreamInfo.m_elementType, "Images");
            var labelsVar = CNTKLib.InputVariable(new int[] { m_numClasses }, labelStreamInfo.m_elementType, "Labels");

            // prepare for training
            var trainingLoss = CNTKLib.CrossEntropyWithSoftmax(m_classifierOutput, labelsVar, "lossFunction");
            var prediction = CNTKLib.ClassificationError(m_classifierOutput, labelsVar, m_TopN, "predictionError");


            var trainer = Trainer.CreateTrainer(m_classifierOutput, trainingLoss, prediction,
                new List<Learner> {Learner.SGDLearner(m_classifierOutput.Parameters(), m_learningRatePerSample) });


            int outputFrequencyInMinibatches = 20, miniBatchCount = 0;

            // Feed data to the trainer for number of epochs. 
            while (true)
            {
                var minibatchData = m_minibatchSource.GetNextMinibatch(m_minibatchSize, m_device);

                // Stop training once max epochs is reached.
                if (minibatchData.empty())
                {
                    break;
                }

                trainer.TrainMinibatch(new Dictionary<Variable, MinibatchData>()
                        {{imageInput, minibatchData[imageStreamInfo]}, {labelsVar, minibatchData[labelStreamInfo]}},
                    m_device);

                if ((miniBatchCount % outputFrequencyInMinibatches) == 0 && trainer.PreviousMinibatchSampleCount() != 0)
                {
                    float trainLossValue = (float) trainer.PreviousMinibatchLossAverage();
                    float evaluationValue = (float) trainer.PreviousMinibatchEvaluationAverage();
                    //Console
                    Console.WriteLine(
                        $"Minibatch: {miniBatchCount} CrossEntropyLoss = {trainLossValue}, EvaluationCriterion = {evaluationValue}");
                    //诊断控制台
                    System.Diagnostics.Trace.WriteLine(
                        $"Minibatch: {miniBatchCount} CrossEntropyLoss = {trainLossValue}, EvaluationCriterion = {evaluationValue}");
                }

                miniBatchCount++;
            }

            // save the model
            var imageClassifier = Function.Combine(new List<Variable>() {trainingLoss, prediction, m_classifierOutput },
                "ImageClassifier");
            imageClassifier.Save(m_modelFile);


           
        }
    }
}
