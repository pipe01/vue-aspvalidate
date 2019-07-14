using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace VueAspValidate.JS
{
    internal partial class JsVisitor
    {
        public JsVisitor()
        {
            DotnetToJsMethod = new Dictionary<MethodInfo, JsMethodHandler>
            {
                [Info.OfMethod<string>(nameof(string.StartsWith), "String")] = "startsWith",
                [Info.OfMethod<string>(nameof(string.EndsWith), "String")] = "endsWith",
                [Info.OfMethod<string>(nameof(string.Contains), "String")] = "includes",

                [Info.OfMethod<string>(nameof(string.IndexOf), "String")] = "indexOf",
                [Info.OfMethod<string>(nameof(string.IndexOf), "Char")] = "indexOf",
                [Info.OfMethod<string>(nameof(string.IndexOf), "String, Int32")] = "indexOf",
                [Info.OfMethod<string>(nameof(string.IndexOf), "Char, Int32")] = "indexOf",

                [Info.OfMethod<string>(nameof(string.LastIndexOf), "String")] = "lastIndexOf",
                [Info.OfMethod<string>(nameof(string.LastIndexOf), "Char")] = "lastIndexOf",
                [Info.OfMethod<string>(nameof(string.LastIndexOf), "String, Int32")] = "lastIndexOf",
                [Info.OfMethod<string>(nameof(string.LastIndexOf), "Char, Int32")] = "lastIndexOf",

                [Info.OfPropertyGet<string>("Chars")] = new HandlerDelegate(HandleStringGetChars),

                [Info.OfMethod<Regex>(nameof(Regex.IsMatch), "String, String")] = new HandlerDelegate(HandleRegexStringString),
            };
        }

        private void HandleToString(MethodCallExpression node)
        {
            Builder.Append("String(");
            base.Visit(node.Object);
            Builder.Append(")");
        }

        private void HandleStringGetChars(MethodCallExpression node)
        {
            base.Visit(node.Object);
            Builder.Append("[");
            base.Visit(node.Arguments[0]);
            Builder.Append("]");
        }

        private void HandleRegexStringString(MethodCallExpression node)
        {
            Builder.Append("(");
            base.Visit(node.Arguments[0]);
            Builder.Append(".match(");
            base.Visit(node.Arguments[1]);
            Builder.Append(")!=null)");
        }
    }
}
