using System;

namespace VueAspValidate
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class ModelIgnoreAttribute : Attribute
    {
    }
}
