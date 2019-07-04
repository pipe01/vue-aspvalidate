using System;

namespace VueAspValidate.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class MatchesAllAttribute : Attribute
    {
        public string[] Patterns { get; }

        public MatchesAllAttribute(params string[] patterns)
        {
            this.Patterns = patterns;
        }
    }
}
