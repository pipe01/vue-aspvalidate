using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace VueAspValidate.Validators
{
    public class Required : ExpressionValidator<RequiredAttribute, object>
    {
        public override string ErrorMessage => "This field is required";

        protected override Expression<Func<object, ValidatorResult>> GetValidationExpression(ValidatorContext context)
            => value => value == null ? new ValidatorResult(ErrorMessage, true) : ValidatorResult.Success;
    }
}
