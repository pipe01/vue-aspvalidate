using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VueAspValidate.JS
{
    internal partial class JsVisitor : ExpressionVisitor
    {
        private delegate void HandlerDelegate(MethodCallExpression node);

        private readonly struct JsMethodHandler
        {
            public string ReplaceMethodName { get; }
            public HandlerDelegate Handler { get; }

            public JsMethodHandler(string replaceMethodName)
            {
                this.ReplaceMethodName = replaceMethodName;
                this.Handler = null;
            }

            public JsMethodHandler(HandlerDelegate handler)
            {
                this.ReplaceMethodName = null;
                this.Handler = handler;
            }

            public static implicit operator JsMethodHandler(string str) => new JsMethodHandler(str);
            public static implicit operator JsMethodHandler(HandlerDelegate func) => new JsMethodHandler(func);
        }

        private readonly IDictionary<MethodInfo, JsMethodHandler> DotnetToJsMethod = new Dictionary<MethodInfo, JsMethodHandler>
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
            [Info.OfMethod<string>(nameof(string.LastIndexOf), "Char, Int32")] = "lastIndexOf"
        };

        private readonly StringBuilder Builder = new StringBuilder();

        public string JsCode => Builder.ToString();

        public override Expression Visit(Expression node)
        {
            Builder.Clear();
            return base.Visit(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            base.Visit(node.Left);
            Builder.Append(GetOperator(node.NodeType));
            base.Visit(node.Right);

            return node;
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            base.Visit(node.Test);
            Builder.Append("?");
            base.Visit(node.IfTrue);
            Builder.Append(":");
            base.Visit(node.IfFalse);

            return node;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            if (node.Parameters.Count == 1)
            {
                Builder.Append(node.Parameters[0].Name);
            }
            else
            {
                Builder.Append("(")
                       .Append(string.Join(",", node.Parameters.Select(o => o.Name)))
                       .Append(")");
            }

            Builder.Append("=>");

            base.Visit(node.Body);

            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var customMethod = DotnetToJsMethod.TryGetValue(node.Method, out var val) ? val : (JsMethodHandler?)null;

            if (customMethod?.Handler != null)
            {
                customMethod.Value.Handler(node);
                return node;
            }

            if (node.Method.Name == "ToString")
            {
                HandleToString(node);
                return node;
            }

            base.Visit(node.Object);

            Builder.Append(".");

            var methodName = customMethod?.ReplaceMethodName ?? node.Method.Name;

            Builder.Append(methodName)
                   .Append("(");

            for (int i = 0; i < node.Arguments.Count; i++)
            {
                base.Visit(node.Arguments[i]);

                if (i != node.Arguments.Count - 1)
                    Builder.Append(",");
            }

            Builder.Append(")");

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            Builder.Append(JsonConvert.SerializeObject(node.Value));
            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            Builder.Append(node.Name);
            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.ArrayLength:
                    base.Visit(node.Operand);
                    Builder.Append(".Length");
                    break;
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    base.Visit(node.Operand);
                    break;
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                    Builder.Append("!");
                    base.Visit(node.Operand);
                    break;
                default:
                    base.Visit(node.Operand);
                    break;
            }

            return node;
        }

        private static string GetOperator(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.Add:
                    return "+";
                case ExpressionType.Multiply:
                    return "*";
                case ExpressionType.Subtract:
                    return "-";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Assign:
                    return "=";
                case ExpressionType.Equal:
                    return "==";
                case ExpressionType.NotEqual:
                    return "!=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.And:
                    return "&&";
                case ExpressionType.Or:
                    return "||";
            }
            throw new NotImplementedException("Operator not implemented");
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            Builder.Append("null");
            return node;
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            return base.VisitIndex(node);
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            return base.VisitInvocation(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            base.Visit(node.Expression);
            Builder.Append(".")
                   .Append(node.Member.Name);

            return node;
        }

        protected override Expression VisitNew(NewExpression node)
        {
            throw new NotSupportedException("You cannot create new instances inside a lambda");
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            Builder.Append("[");

            for (int i = 0; i < node.Expressions.Count; i++)
            {
                base.Visit(node.Expressions[i]);

                if (i != node.Expressions.Count - 1)
                    Builder.Append(",");
            }

            Builder.Append("]");

            return node;
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            Builder.Append("true");
            return node;
        }
    }
}
