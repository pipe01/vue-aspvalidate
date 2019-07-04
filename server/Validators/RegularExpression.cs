using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VueAspValidate.Validators
{
    [ValidateIfAttribute(typeof(RegularExpressionAttribute))]
    public class RegularExpression : IValidator
    {
        public string BuildJS(ValidatorContext context)
        {
            var attr = context.Attribute<RegularExpressionAttribute>();

            return $"return /{attr.Pattern}/.exec(value) != null ? true : '{attr.ErrorMessage ?? "Invalid format"}'";
        }

        public ValidatorResult Check(object value, ValidatorContext context)
        {
            var attr = context.Attribute<RegularExpressionAttribute>();

            if (!Regex.IsMatch((string)value, attr.Pattern))
                return attr.ErrorMessage ?? "Invalid format";

            return true;
        }
    }
}
