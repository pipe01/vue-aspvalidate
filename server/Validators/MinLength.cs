using System;
using System.ComponentModel.DataAnnotations;

namespace VueAspValidate.Validators
{
    [ValidateIfAttribute(typeof(MinLengthAttribute))]
    public class MinLength : IValidator
    {
        public string BuildJS(ValidatorContext context)
        {
            var attr = context.Attribute<MinLengthAttribute>();

            return $"return value.length >= {attr.Length} ? true : '{attr.ErrorMessage ?? $"Must be more than {attr.Length} characters long"}'";
        }

        public ValidatorResult Check(object value, ValidatorContext context)
        {
            var attr = context.Attribute<MinLengthAttribute>();

            return value is string str
                ? str.Length >= attr.Length ? ValidatorResult.Success : attr.ErrorMessage ?? $"@$ must be more than {attr.Length} characters long"
                : value is Array arr
                ? arr.Length >= attr.Length ? ValidatorResult.Success : attr.ErrorMessage ?? $"@$ must be more than {attr.Length} items long"
                : throw new ArgumentException("Invalid type", nameof(value));
        }
    }
}
