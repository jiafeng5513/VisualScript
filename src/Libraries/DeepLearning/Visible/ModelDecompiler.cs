using ProtobufTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepLearning.Visible
{
    /// <summary>
    /// 逆向训练好的模型
    /// </summary>
    public class ModelDecompiler
    {
        public static string ModelFile = string.Empty;
        public static string LabelFile = string.Empty;
        public static string inputFile = string.Empty;

        public static void ParamGet(string ModelFile = " ", string LabelFile = " ", string inputFile = " ")
        {
            ModelDecompiler.ModelFile = ModelFile;
            ModelDecompiler.LabelFile = LabelFile;
            ModelDecompiler.inputFile = inputFile;

            using (StreamWriter sw = new StreamWriter("F:\\ModelDecompilerParams.txt"))
            {

                sw.WriteLine(ModelFile);
                sw.WriteLine(LabelFile);
                sw.WriteLine(inputFile);

            }
        }
    }
}
