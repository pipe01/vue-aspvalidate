using System;
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

                [Info.OfMethod<Regex>(nameof(Regex.IsMatch), "String, String")] = new HandlerDelegate(HandleRegexIsMatchStringString),
                [Info.OfMethod<Regex>(nameof(Regex.IsMatch), "String, String, RegexOptions")] = new HandlerDelegate(HandleRegexIsMatchStringStringRegexOptions),
                [Info.OfMethod<Regex>(nameof(Regex.Replace), "String, String, String")] = new HandlerDelegate(HandleRegexReplaceStringStringString),
            };
        }

        private void HandleRegexReplaceStringStringString(MethodCallExpression node)
        {
            base.Visit(node.Arguments[0]);
            Builder.Append(".replace(");

            if (node.Arguments[1] is ConstantExpression constExpr)
            {
                Builder.Append("/")
                       .Append(constExpr.Value)
                       .Append("/");
            }
            else
            {
                base.Visit(node.Arguments[1]);
            }

            Builder.Append(",");
            base.Visit(node.Arguments[2]);
            Builder.Append(")");
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

        private void HandleRegexIsMatchStringString(MethodCallExpression node)
        {
            Builder.Append("new RegExp(");
            base.Visit(node.Arguments[1]);
            Builder.Append(").test(");
            base.Visit(node.Arguments[0]);
            Builder.Append(")");
        }

        private void HandleRegexIsMatchStringStringRegexOptions(MethodCallExpression node)
        {
            if (!(node.Arguments[2] is ConstantExpression flagsExpr) || !(flagsExpr.Value is RegexOptions flags))
                throw new NotSupportedException("Regex flags must be a constant");

            string jsFlags = "g";

            if ((flags & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase)
                jsFlags += "i";
            if ((flags & RegexOptions.Multiline) == RegexOptions.Multiline)
                jsFlags += "m";

            Builder.Append("new RegExp(");
            base.Visit(node.Arguments[1]);
            Builder.Append(",\"")
                   .Append(jsFlags)
                   .Append("\").test(");
            base.Visit(node.Arguments[0]);
            Builder.Append(")");
        }

    }
}
