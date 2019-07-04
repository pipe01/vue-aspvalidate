using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VueAspValidate
{
    public delegate bool ShouldValidateParameterDelegate(ParameterInfo parameter);

    public static class AspValidate
    {
        public static ShouldValidateParameterDelegate ShouldValidateParameter { get; set; }
    }
}
