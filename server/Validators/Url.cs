using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VueAspValidate.Validators
{
    [ValidateIfAttribute(typeof(UrlAttribute))]
    public class Url : IValidator
    {
        private const string Pattern = @"^(?!mailto:)(?:(?:http|https|ftp)://)(?:\\S+(?::\\S*)?@)?(?:(?:(?:[1-9]\\d?|1\\d\\d|2[01]\\d|22[0-3])(?:\\.(?:1?\\d{1,2}|2[0-4]\\d|25[0-5])){2}(?:\\.(?:[0-9]\\d?|1\\d\\d|2[0-4]\\d|25[0-4]))|(?:(?:[a-z\\u00a1-\\uffff0-9]+-?)*[a-z\\u00a1-\\uffff0-9]+)(?:\\.(?:[a-z\\u00a1-\\uffff0-9]+-?)*[a-z\\u00a1-\\uffff0-9]+)*(?:\\.(?:[a-z\\u00a1-\\uffff]{2,})))|localhost)(?::\\d{2,5})?(?:(/|\\?|#)[^\\s]*)?$";
        private const string JsPattern = @"((([A-Za-z]{3,9}:(?:\/\/)?)(?:[\-;:&=\+\$,\w]+@)?[A-Za-z0-9\.\-]+|(?:www\.|[\-;:&=\+\$,\w]+@)[A-Za-z0-9\.\-]+)((?:\/[\+~%\/\.\w\-_]*)?\??(?:[\-\+=&;%@\.\w_]*)#?(?:[\.\!\/\\\w]*))?)";
        private static readonly Regex Regex = new Regex(Pattern);

        public string BuildJS(ValidatorContext context)
        {
            var attr = context.Attribute<UrlAttribute>();

            return $"value => /{JsPattern}/.exec(value) != null ? true : '{attr.ErrorMessage ?? "Invalid URL"}'";
        }

        public ValidatorResult Check(object value, ValidatorContext context)
        {
            return Regex.IsMatch((string)value);
        }
    }
}
