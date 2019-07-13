using System;
using System.Reflection;

namespace VueAspValidate
{
    public sealed class ValidatorContext
    {
        public object Model { get; }
        public PropertyInfo Property { get; }

        internal ValidatorContext(object model, PropertyInfo property)
        {
            this.Model = model;
            this.Property = property;
        }

        public T Attribute<T>() where T : Attribute
            => Property.GetCustomAttribute<T>() ?? throw new InvalidOperationException($"This property does not have an appropiate attribute of type {typeof(T).Name}");
    }
}
