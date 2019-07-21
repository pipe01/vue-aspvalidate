using System;
using System.ComponentModel.DataAnnotations;

namespace VueAspValidate.Validators
{
    [ValidateIfAttribute(typeof(StringLengthAttribute))]
    public class StringLength : IValidator
    {
        public string BuildJS(ValidatorContext context)
        {
            var attr = context.Attribute<StringLengthAttribute>();

            return $"value => value.length >= {attr.MinimumLength} && value.length <= {attr.MaximumLength} ? true : '{attr.ErrorMessage ?? $"Must be between {attr.MinimumLength} and {attr.MaximumLength} characters long"}'";
        }

        public ValidatorResult Check(object value, ValidatorContext context)
        {
            var attr = context.Attribute<StringLengthAttribute>();
            var str = (string)value;

            return str.Length >= attr.MinimumLength && str.Length <= attr.MaximumLength
                ? ValidatorResult.Success
                : attr.ErrorMessage ?? $"@$ must be between {attr.MinimumLength} and {attr.MaximumLength} characters long";
        }
    }
}
