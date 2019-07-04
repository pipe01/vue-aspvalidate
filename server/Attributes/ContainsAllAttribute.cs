using System;

namespace VueAspValidate.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ContainsAllAttribute : Attribute
    {
        public string Characters { get; }

        public ContainsAllAttribute(string characters)
        {
            this.Characters = characters;
        }
    }
}
