using Newtonsoft.Json;
using System;
using System.Linq;
using VueAspValidate.Attributes;

namespace VueAspValidate.Validators
{
    [ValidateIfAttribute(typeof(ContainsAllAttribute))]
    public class ContainsAll : IValidator
    {
        public string BuildJS(ValidatorContext context)
        {
            var attr = context.Attribute<ContainsAllAttribute>();
            var arrJson = JsonConvert.SerializeObject(attr.Characters);

            return $"return {arrJson}.every(o => value.indexOf(o) != -1)";
        }

        public ValidatorResult Check(object value, ValidatorContext context)
        {
            var attr = context.Attribute<ContainsAllAttribute>();
            var str = (string)value;

            return attr.Characters.All(str.Contains);
        }
    }
}
