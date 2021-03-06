using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using VueAspValidate.JS;

namespace VueAspValidate
{
    public interface IValidator
    {
        ValidatorResult Check(object value, ValidatorContext context);
        string BuildJS(ValidatorContext context);
    }

    public interface IValidatorWithErrorMessage : IValidator
    {
        string ErrorMessage { get; }
    }

    public abstract class Validator<TAttribute> : IValidator where TAttribute : Attribute
    {
        protected abstract string BuildJS(ValidatorContext context, TAttribute attribute);

        public string BuildJS(ValidatorContext context)
        {
            return BuildJS(context, context.Attribute<TAttribute>());
        }

        protected abstract ValidatorResult Check(object value, ValidatorContext context, TAttribute attribute);

        public ValidatorResult Check(object value, ValidatorContext context)
        {
            return Check(value, context, context.Attribute<TAttribute>());
        }
    }

    public abstract class ExpressionValidator<TAttribute, TValue> : IValidatorWithErrorMessage where TAttribute : Attribute
    {
        protected abstract Expression<Func<TValue, ValidatorResult>> GetValidationExpression(ValidatorContext context);

        public virtual string ErrorMessage => "Invalid format";

        string IValidator.BuildJS(ValidatorContext context)
        {
            return $"v => ({GetValidationExpression(context).ToJs()})(v) || {JsonConvert.SerializeObject(ErrorMessage)}";
        }

        ValidatorResult IValidator.Check(object value, ValidatorContext context)
        {
            if (value != null && !(value is TValue))
                throw new ArgumentException("Invalid value type", nameof(value));

            return GetValidationExpression(context).Compile()((TValue)value);
        }
    }
}
