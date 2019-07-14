using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace VueAspValidate
{
    public interface IValidator
    {
        ValidatorResult Check(object value, ValidatorContext context);
        string BuildJS(ValidatorContext context);
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

    public abstract class ExpressionValidator<TAttribute, TValue> : IValidator where TAttribute : Attribute
    {
        protected abstract Expression<Func<TValue, bool>> GetValidationExpression(ValidatorContext context);

        string IValidator.BuildJS(ValidatorContext context)
        {
            throw new NotImplementedException();
        }

        ValidatorResult IValidator.Check(object value, ValidatorContext context)
        {
            if (!(value is TValue val))
                throw new ArgumentException("Invalid value type", nameof(value));

            return GetValidationExpression(context).Compile()(val);
        }
    }
}
