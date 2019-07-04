using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace VueAspValidate
{
    public interface IVueAspValidateConfigurer
    {
        IVueAspValidateConfigurer AddValidator<TValidator>(Predicate<PropertyInfo> propertyPredicate) where TValidator : IValidator, new();
        IVueAspValidateConfigurer AddValidator<TValidator, TAttribute>() where TValidator : IValidator, new() where TAttribute : Attribute;

        IVueAspValidateConfigurer AddForm<TModel>(string formId);
    }

    internal class VueAspValidateConfigurer : IVueAspValidateConfigurer
    {
        public AspValidateOptions Options => new AspValidateOptions(Validators, Forms);

        private readonly IDictionary<string, ModelForm> Forms = new Dictionary<string, ModelForm>();
        private readonly ValidatorCollection Validators = new ValidatorCollection();

        public IVueAspValidateConfigurer AddForm<TModel>(string formId)
        {
            Forms.Add(formId, new ModelForm(formId, typeof(TModel)));
            return this;
        }

        public IVueAspValidateConfigurer AddValidator<TValidator>(Predicate<PropertyInfo> propertyPredicate) where TValidator : IValidator, new()
        {
            Validators.Add(new ConditionalValidator(propertyPredicate, new TValidator()));
            return this;
        }

        public IVueAspValidateConfigurer AddValidator<TValidator, TAttribute>() where TValidator : IValidator, new() where TAttribute : Attribute
            => AddValidator<TValidator>(o => o.GetCustomAttribute<TAttribute>() != null);
    }
}
