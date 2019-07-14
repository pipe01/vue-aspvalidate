using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace VueAspValidate.JS
{
    internal partial class JsVisitor
    {
        public JsVisitor()
        {
            DotnetToJsMethod[Info.OfMethod<Regex>(nameof(Regex.IsMatch), "String, String")] = new HandlerDelegate(HandleRegexStringString);
            DotnetToJsMethod[Info.OfPropertyGet<string>("Chars")] = new HandlerDelegate(HandleStringGetChars);
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
