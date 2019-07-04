using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahoo.Yui.Compressor;

namespace VueAspValidate
{
    internal class JsBuilder
    {
        private readonly ValidatorCollection Validators;

        public JsBuilder(AspValidateOptions options)
        {
            this.Validators = options.Validators;
        }

        public string BuildForModelType(Type modelType)
        {
            var js = new StringBuilder();
            js.Append("({");

            foreach (var prop in modelType.GetProperties())
            {
                var context = new ValidatorContext(null, prop);

                js.Append(prop.Name + ":[");

                foreach (var val in Validators.ForProperty(prop))
                {
                    js.Append("(function(value){").Append(val.BuildJS(context)).Append("}),");
                }

                js.Append("],");
            }

            js.Append("})");

            try
            {
                return new JavaScriptCompressor().Compress(js.ToString());
            }
            catch (Exception ex)
            {

                throw;
            }
        }

    }
}
