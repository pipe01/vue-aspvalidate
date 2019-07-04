using System;

namespace VueAspValidate
{
    internal readonly struct ModelForm
    {
        public string ID { get; }
        public Type ModelType { get; }

        public ModelForm(string iD, Type modelType)
        {
            this.ID = iD;
            this.ModelType = modelType;
        }
    }
}
