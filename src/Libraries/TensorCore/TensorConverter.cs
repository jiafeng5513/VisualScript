using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using CNTK;

namespace TensorCore
{
    public class TensorConverter
    {
        public static Variable FunctionToVariable(Function func)
        {
            return new Variable(func);
        }

        public static Variable ParameterToVariable(Parameter p)
        {
            return new Variable(p);
        }
    }
}
