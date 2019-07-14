using System.Linq.Expressions;

namespace VueAspValidate.JS
{
    public static class ExpressionExtensions
    {
        public static string ToJs(this Expression expr)
        {
            var visitor = new JsVisitor();
            visitor.Visit(expr);
            return visitor.JsCode;
        }
    }
}
