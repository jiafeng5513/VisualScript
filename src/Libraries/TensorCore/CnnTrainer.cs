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
        private int[] m_imageDim;
        private int m_numClasses;
        private uint m_TopN;
        private string m_modelFile;
        private uint m_minibatchSize;
        private Function m_classifierOutput;
        private DeviceDescriptor m_device;
        private MinibatchSource m_minibatchSource;
        private TrainingParameterScheduleDouble m_learningRatePerSample;

        private static CnnTrainer uniqueInstance;

        private CnnTrainer()
        {
        }

        public static CnnTrainer getInstance()
        {
            if (uniqueInstance == null)
            {
                uniqueInstance = new CnnTrainer();
            }
            return uniqueInstance;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m_imageDim"></param>
        /// <param name="m_numClasses"></param>
        /// <param name="m_TopN"></param>
        /// <param name="m_modelFile"></param>
        /// <param name="m_minibatchSize"></param>
        /// <param name="m_classifierOutput"></param>
        /// <param name="m_device"></param>
        /// <param name="minibatchSource"></param>
        /// <param name="m_learningRatePerSample"></param>
        public bool CnnTrainerInti(int[] m_imageDim, int m_numClasses, int m_TopN, 
            string m_modelFile, int m_minibatchSize, Function m_classifierOutput, 
            DeviceDescriptor m_device, MinibatchSource minibatchSource, 
            TrainingParameterScheduleDouble m_learningRatePerSample)
        {
            this.m_imageDim = m_imageDim;
            this.m_numClasses = m_numClasses;
            this.m_TopN = (uint)m_TopN;
            this.m_modelFile = m_modelFile;
            this.m_minibatchSize = (uint)m_minibatchSize;
            this.m_classifierOutput = m_classifierOutput;
            this.m_device = m_device;
            this.m_minibatchSource = minibatchSource;
            this.m_learningRatePerSample = m_learningRatePerSample;
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
        public bool RunTraining()
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


            return true;
        }
    }
}
