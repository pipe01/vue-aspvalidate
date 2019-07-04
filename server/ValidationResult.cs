using Newtonsoft.Json;
using System.Collections.Generic;

namespace VueAspValidate
{
    public sealed class ValidationResult
    {
        [JsonIgnore]
        public bool Success { get; }

        public IDictionary<string, string[]> Errors { get; }

        internal ValidationResult(bool success, IDictionary<string, string[]> errors)
        {
            this.Success = success;
            this.Errors = errors;
        }
    }
}
