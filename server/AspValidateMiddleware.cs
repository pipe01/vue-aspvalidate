using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VueAspValidate.JS;

namespace VueAspValidate
{
    internal class AspValidateMiddleware
    {
        private static readonly IDictionary<string, string> FormCache = new Dictionary<string, string>();

        private readonly RequestDelegate Next;
        private readonly JsBuilder JsBuilder;
        private readonly AspValidateOptions Options;

        public AspValidateMiddleware(RequestDelegate next, JsBuilder jsBuilder, AspValidateOptions options)
        {
            this.Next = next;
            this.JsBuilder = jsBuilder;
            this.Options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/vaspval"))
            {
                string formId = context.Request.Path.Value.Split('/')[2];

                if (formId.EndsWith(".js"))
                    formId = formId.Substring(0, formId.Length - 3);

                if (!Options.Forms.TryGetValue(formId, out var form))
                {
                    return;
                }

                if (!FormCache.TryGetValue(formId, out var formScript))
                {
                    FormCache[formId] = formScript = JsBuilder.BuildForModelType(form.ModelType);
                }

                context.Response.ContentType = "application/javascript";
                await context.Response.WriteAsync(formScript);
                return;
            }

            await Next(context);
        }
    }
}
