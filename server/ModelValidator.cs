using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VueAspValidate
{
    internal class ModelValidator
    {
        private readonly ValidatorCollection Validators;

        public ModelValidator(AspValidateOptions options)
        {
            this.Validators = options.Validators;
        }

        public ValidationResult Validate(object model)
        {
            var errors = new Dictionary<string, IList<string>>();

            foreach (var prop in model.GetType().GetProperties())
            {
                foreach (var val in Validators.ForProperty(prop))
                {
                    var result = val.Check(prop.GetValue(model), new ValidatorContext(model, prop));

                    if (!result.Successful)
                    {
                        if (!errors.TryGetValue(prop.Name, out var propErrors))
                            errors[prop.Name] = propErrors = new List<string>();

                        propErrors.Add(result.Error.Replace("@$", prop.Name));

                        if (result.Stop)
                            goto exit;
                    }
                }
            }

        exit:
            return new ValidationResult(errors.Count == 0, errors.ToDictionary(o => o.Key, o => o.Value.ToArray()));
        }
    }
}
