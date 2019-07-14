using System.Text;

namespace VueAspValidate.JS
{
    internal static class StringBuilderJS
    {
        public static StringBuilder FunctionCall(this StringBuilder builder, string funcName, params object[] args)
        {
            return builder.Append($"{funcName}({string.Join(",", args)})");
        }
    }
}
