using System.Linq;
using System.Text.RegularExpressions;
using VueAspValidate.Attributes;

namespace VueAspValidate.Validators
{
    public class MatchesAll : Validator<MatchesAllAttribute>
    {
        protected override string BuildJS(ValidatorContext context, MatchesAllAttribute attribute)
        {
            return $"return [{string.Join(",", attribute.Patterns.Select(o => $"/{o}/"))}].every(o => o.exec(value) != null) ? true : ";
        }

        protected override ValidatorResult Check(object value, ValidatorContext context, MatchesAllAttribute attribute)
        {
            return attribute.Patterns.All(o => Regex.IsMatch((string)value, o));
        }
    }
}
