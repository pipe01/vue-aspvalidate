using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

namespace VueAspValidate
{
    internal class ValidatorCollection : Collection<ConditionalValidator>
    {
        public IEnumerable<IValidator> ForProperty(PropertyInfo prop)
            => this.Where(o => o.Predicate(prop)).Select(o => o.Validator);
    }
}
