using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VueAspValidate.Validators
{
    [ValidateIfAttribute(typeof(EmailAddressAttribute))]
    public class EmailAddress : IValidator
    {
        private const string Pattern = @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])";
        private static readonly Regex Regex = new Regex(Pattern);

        public string BuildJS(ValidatorContext context)
        {
            var attr = context.Attribute<EmailAddressAttribute>();

            return $"return /{Pattern}/.exec(value) != null ? true : '{attr.ErrorMessage ?? "Invalid email"}'";
        }

        public ValidatorResult Check(object value, ValidatorContext context)
        {
            return Regex.IsMatch((string)value);
        }
    }
}
