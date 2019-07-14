using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using VueAspValidate.JS;

namespace VueAspValidate
{
    public static class MiddlewareExtension
    {
        public static IApplicationBuilder UseAspValidate(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AspValidateMiddleware>();
        }
    }

    public static class ServiceExtension
    {
        public static IServiceCollection AddAspValidate(this IServiceCollection services)
            => AddAspValidate(services, _ => { });

        public static IServiceCollection AddAspValidate(this IServiceCollection services, Action<IVueAspValidateConfigurer> opts)
        {
            var conf = new VueAspValidateConfigurer();
            opts(conf);

            services.AddSingleton(conf.Options);

            foreach (var item in typeof(IValidator).Assembly.GetTypes().Where(o => 
                typeof(IValidator).IsAssignableFrom(o)
                && o != typeof(IValidator)
                && o != typeof(Validator<>)
                && o != typeof(ExpressionValidator<,>)))
            {
                Predicate<PropertyInfo> pred;
                var attr = item.GetCustomAttribute<ValidateIfAttributeAttribute>();
                var @base = item.BaseType.GenericTypeArguments.Length > 0 ? item.BaseType.GetGenericTypeDefinition() : null;

                if (attr != null)
                {
                    pred = o => o.GetCustomAttribute(attr.AttributeType) != null;
                }
                else if (@base == typeof(Validator<>)
                      || @base == typeof(ExpressionValidator<,>))
                {
                    pred = o => o.GetCustomAttribute(item.BaseType.GenericTypeArguments[0]) != null;
                }
                else
                {
                    var predMethod = item.GetMethod("ShouldValidate", BindingFlags.NonPublic | BindingFlags.Static);
                    pred = o => (bool)predMethod.Invoke(null, new object[] { o });
                }

                conf.Options.Validators.Add(new ConditionalValidator(pred, Activator.CreateInstance(item) as IValidator));
            }

            services.AddSingleton<ModelValidator>();
            services.AddSingleton<JsBuilder>();

            return services;
        }
    }

    public static class MvcExtensions
    {
        public static MvcOptions AddAspValidate(this MvcOptions options)
        {
            options.Filters.Add<AspValidateActionFilter>();
            return options;
        }
    }
}
