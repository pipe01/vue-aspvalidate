using System.Collections.Generic;

namespace VueAspValidate
{
    internal class AspValidateOptions
    {
        public ValidatorCollection Validators { get; set; }
        public IDictionary<string, ModelForm> Forms { get; set; }

        public AspValidateOptions(ValidatorCollection validators, IDictionary<string, ModelForm> forms)
        {
            this.Validators = validators;
            this.Forms = forms;
        }
    }
}
