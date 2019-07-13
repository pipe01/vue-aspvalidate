using System;

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
}
