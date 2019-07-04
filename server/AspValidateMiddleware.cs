using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace VueAspValidate
{
    internal class AspValidateMiddleware
    {
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

                context.Response.ContentType = "application/javascript";
                await context.Response.WriteAsync(JsBuilder.BuildForModelType(form.ModelType));
                return;
            }

            await Next(context);
        }
    }
}
