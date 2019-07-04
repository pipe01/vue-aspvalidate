using System;
using System.Reflection;

namespace VueAspValidate
{
    internal class ConditionalValidator
    {
        public Predicate<PropertyInfo> Predicate { get; }
        public IValidator Validator { get; }

        public ConditionalValidator(Predicate<PropertyInfo> predicate, IValidator validator)
        {
            this.Predicate = predicate;
            this.Validator = validator;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class ValidateIfAttributeAttribute : Attribute
    {
        public Type AttributeType { get; }

        public ValidateIfAttributeAttribute(Type attributeType)
        {
            this.AttributeType = attributeType;
        }
    }
}
