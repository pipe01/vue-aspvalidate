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
            return base.VisitConditional(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            Builder.Append("function(")
                   .Append(string.Join(",", node.Parameters.Select(o => o.Name)))
                   .Append("){");

            if (node.ReturnType != typeof(void))
                Builder.Append("return ");

            base.Visit(node.Body);

            Builder.Append(";}");

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

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return base.VisitBlock(node);
        }

        protected override CatchBlock VisitCatchBlock(CatchBlock node)
        {
            return base.VisitCatchBlock(node);
        }

        protected override Expression VisitDebugInfo(DebugInfoExpression node)
        {
            return base.VisitDebugInfo(node);
        }

        protected override Expression VisitDefault(DefaultExpression node)
        {
            return base.VisitDefault(node);
        }

        protected override Expression VisitDynamic(DynamicExpression node)
        {
            return base.VisitDynamic(node);
        }

        protected override ElementInit VisitElementInit(ElementInit node)
        {
            return base.VisitElementInit(node);
        }

        protected override Expression VisitExtension(Expression node)
        {
            return base.VisitExtension(node);
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            return base.VisitGoto(node);
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            return base.VisitIndex(node);
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            return base.VisitInvocation(node);
        }

        protected override Expression VisitLabel(LabelExpression node)
        {
            return base.VisitLabel(node);
        }

        protected override LabelTarget VisitLabelTarget(LabelTarget node)
        {
            return base.VisitLabelTarget(node);
        }

        protected override Expression VisitListInit(ListInitExpression node)
        {
            return base.VisitListInit(node);
        }

        protected override Expression VisitLoop(LoopExpression node)
        {
            return base.VisitLoop(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            base.Visit(node.Expression);
            Builder.Append(".")
                   .Append(node.Member.Name);

            return node;
        }

        protected override MemberAssignment VisitMemberAssignment(MemberAssignment node)
        {
            return base.VisitMemberAssignment(node);
        }

        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {
            return base.VisitMemberBinding(node);
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            return base.VisitMemberInit(node);
        }

        protected override MemberListBinding VisitMemberListBinding(MemberListBinding node)
        {
            return base.VisitMemberListBinding(node);
        }

        protected override MemberMemberBinding VisitMemberMemberBinding(MemberMemberBinding node)
        {
            return base.VisitMemberMemberBinding(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            return base.VisitNew(node);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            return base.VisitNewArray(node);
        }

        protected override Expression VisitRuntimeVariables(RuntimeVariablesExpression node)
        {
            return base.VisitRuntimeVariables(node);
        }

        protected override Expression VisitSwitch(SwitchExpression node)
        {
            return base.VisitSwitch(node);
        }

        protected override SwitchCase VisitSwitchCase(SwitchCase node)
        {
            return base.VisitSwitchCase(node);
        }

        protected override Expression VisitTry(TryExpression node)
        {
            return base.VisitTry(node);
        }

        protected override Expression VisitTypeBinary(TypeBinaryExpression node)
        {
            return base.VisitTypeBinary(node);
        }
    }
}
