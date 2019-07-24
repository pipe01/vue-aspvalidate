using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VueAspValidate.Validators;

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
                foreach (var val in Validators.ForProperty(prop).OrderBy(o => o, new ValidatorComparer(typeof(Required))))
                {
                    var result = val.Check(prop.GetValue(model), new ValidatorContext(model, prop));

                    if (!result.Successful)
                    {
                        if (!errors.TryGetValue(prop.Name, out var propErrors))
                            errors[prop.Name] = propErrors = new List<string>();

                        propErrors.Add(result.Error?.Replace("@$", prop.Name)
                            ?? (val is IValidatorWithErrorMessage err ? err.ErrorMessage : "Invalid field"));

                        if (result.Stop)
                            goto nextProp;
                    }
                }

                nextProp:
                { }
            }

            return new ValidationResult(errors.Count == 0, errors.ToDictionary(o => o.Key, o => o.Value.ToArray()));
        }

        private class ValidatorComparer : IComparer<IValidator>
        {
            private readonly Type[] PriorityTypes;

            public ValidatorComparer(params Type[] priorityTypes)
            {
                this.PriorityTypes = priorityTypes;
            }

            public int Compare(IValidator x, IValidator y)
            {
                if (PriorityTypes.Contains(x.GetType()))
                    return -1;
                else if (PriorityTypes.Contains(y.GetType()))
                    return 1;

                return 0;
            }
        }
    }
}
