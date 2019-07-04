using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace VueAspValidate
{
    internal class AspValidateActionFilter : IAsyncActionFilter
    {
        private readonly ModelValidator Validator;

        public AspValidateActionFilter(ModelValidator validator)
        {
            this.Validator = validator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!(context.ActionDescriptor is ControllerActionDescriptor action))
                goto next;

            var modelParameter = action.Parameters.OfType<ControllerParameterDescriptor>().FirstOrDefault(o =>
                    AspValidate.ShouldValidateParameter?.Invoke(o.ParameterInfo)
                    ?? o.ParameterInfo.GetCustomAttribute<ValidateModelAttribute>() != null);

            if (modelParameter == null)
                goto next;

            var model = context.ActionArguments[modelParameter.Name];

            var result = Validator.Validate(model);

            if (!result.Success)
            {
                var json = new JObject();
                var errors = new JObject();
                json["errors"] = errors;

                foreach (var item in result.Errors)
                {
                    errors[item.Key] = item.Value == null ? new JArray() : new JArray(item.Value);
                }

                context.Result = new BadRequestObjectResult(json);
                return;
            }

        next:
            await next();
        }
    }
}
