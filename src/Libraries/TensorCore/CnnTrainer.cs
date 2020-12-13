using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CNTK;
using Newtonsoft.Json;

namespace TensorCore
{
    /*
     * CnnTrainer for Encapsulating Training procedures
     */
    public class TrainerParamContainer
    {
        public int[] m_imageDim;
        public int m_numClasses;
        public uint m_TopN;
        public string m_modelFile;
        public uint m_minibatchSize;
        public Function m_classifierOutput;
        public DeviceDescriptor m_device;
        public MinibatchSource m_minibatchSource;
        public TrainingParameterScheduleDouble m_learningRatePerSample;

        public bool Check()
        {
            bool CanRun = true;
            if (m_imageDim == null)
            {
                Console.WriteLine("imageDim is null");
                CanRun = false;
                
            }

            //Console.WriteLine("numClasses=" + numClasses + "  TopN=" + TopN + " modelFile=" + modelFile + " minibatchSize=" + minibatchSize);
            if (m_classifierOutput == null)
            {
                Console.WriteLine("classifierOutput is null");
                CanRun = false;
            }

            if (m_device == null)
            {
                Console.WriteLine("device is null");
                CanRun = false;
            }

            if (m_minibatchSource == null)
            {
                Console.WriteLine("minibatchSource is null");
                CanRun = false;
            }

            if (m_learningRatePerSample == null)
            {
                Console.WriteLine("learningRatePerSample is null");
                CanRun = false;
            }

            return CanRun;

        }
    }
    public class CnnTrainer
    {
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
            TrainerParamContainer ParamDeliverer=new TrainerParamContainer();
            ParamDeliverer.m_imageDim = imageDim;
            ParamDeliverer.m_numClasses = numClasses;
            ParamDeliverer.m_TopN = (uint)TopN;
            ParamDeliverer.m_modelFile = modelFile;
            ParamDeliverer.m_minibatchSize = (uint)minibatchSize;
            ParamDeliverer.m_classifierOutput = classifierOutput;
            ParamDeliverer.m_device = device;
            ParamDeliverer.m_minibatchSource = minibatchSource;
            ParamDeliverer.m_learningRatePerSample = learningRatePerSample;

            //Thread thread = new Thread(new ParameterizedThreadStart(CnnTrainer.RunTraining));//创建线程
            //thread.Start(ParamDeliverer);
            //JsonConvert.SerializeObject(ParamDeliverer.m_imageDim);
            //JsonConvert.SerializeObject(ParamDeliverer.m_numClasses);
            //JsonConvert.SerializeObject(ParamDeliverer.m_TopN);
            //JsonConvert.SerializeObject(ParamDeliverer.m_modelFile);
            //JsonConvert.SerializeObject(ParamDeliverer.m_minibatchSize);
            ////JsonConvert.SerializeObject(ParamDeliverer.m_classifierOutput);
            ///*
            // * 分类器不能直接序列化,要先利用Function的Save进行一步序列化,
            // * 然后封包
            // * 传过来后,解包,拿出byte,Load进去
            // * 这个要先用控制台程序做实验
            // */
            //JsonConvert.SerializeObject(ParamDeliverer.m_minibatchSource);
            //JsonConvert.SerializeObject(ParamDeliverer.m_device);
            //JsonConvert.SerializeObject(ParamDeliverer.m_learningRatePerSample);

            //string json = JsonConvert.SerializeObject(ParamDeliverer);

            //string SavePath = System.Environment.CurrentDirectory;

            //System.IO.File.WriteAllText(SavePath+ "/TrainerParamContainer.json", json);
            //序列化出去
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
        public static void RunTraining(Object paramDeliverer)
        {
            
            var ParamDeliverer =(TrainerParamContainer) paramDeliverer;
            //ConsoleView.ConsoleView m = new ConsoleView.ConsoleView();
            //m.Show();
            //try
            //{
            //    string ParamFile = System.Environment.CurrentDirectory + "/TrainerParamContainer.json";
            //    string json = System.IO.File.ReadAllText(ParamFile);
            //    //ParamDeliverer= JsonConvert.DeserializeObject<TrainerParamContainer>(json);
            //}
            //catch (IOException e)
            //{
            //    Console.WriteLine(e);
            //    return;
            //}

            //if (ParamDeliverer.Check()==false)
            //{
            //    Console.WriteLine("Params Deliverer failed!");
            //    return;
            //}
            Console.WriteLine("*********Engine Check Pass*********");

            // prepare training data

            var imageStreamInfo = ParamDeliverer.m_minibatchSource.StreamInfo("features");
            var labelStreamInfo = ParamDeliverer.m_minibatchSource.StreamInfo("labels");

            // build a model
            var imageInput = CNTKLib.InputVariable(ParamDeliverer.m_imageDim, imageStreamInfo.m_elementType, "Images");
            var labelsVar = CNTKLib.InputVariable(new int[] { ParamDeliverer.m_numClasses }, labelStreamInfo.m_elementType, "Labels");

            // prepare for training
            var trainingLoss = CNTKLib.CrossEntropyWithSoftmax(ParamDeliverer.m_classifierOutput, labelsVar, "lossFunction");
            var prediction = CNTKLib.ClassificationError(ParamDeliverer.m_classifierOutput, labelsVar, ParamDeliverer.m_TopN, "predictionError");


            var trainer = Trainer.CreateTrainer(ParamDeliverer.m_classifierOutput, trainingLoss, prediction,
                new List<Learner> {Learner.SGDLearner(ParamDeliverer.m_classifierOutput.Parameters(), ParamDeliverer.m_learningRatePerSample) });


            int outputFrequencyInMinibatches = 20, miniBatchCount = 0;

            // Feed data to the trainer for number of epochs. 
            while (true)
            {
                var minibatchData = ParamDeliverer.m_minibatchSource.GetNextMinibatch(ParamDeliverer.m_minibatchSize, ParamDeliverer.m_device);

                // Stop training once max epochs is reached.
                if (minibatchData.empty())
                {
                    break;
                }

                trainer.TrainMinibatch(new Dictionary<Variable, MinibatchData>()
                        {{imageInput, minibatchData[imageStreamInfo]}, {labelsVar, minibatchData[labelStreamInfo]}},
                    ParamDeliverer.m_device);

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
            var imageClassifier = Function.Combine(new List<Variable>() {trainingLoss, prediction, ParamDeliverer.m_classifierOutput },
                "ImageClassifier");
            imageClassifier.Save(ParamDeliverer.m_modelFile);


           
        }
    }
}
