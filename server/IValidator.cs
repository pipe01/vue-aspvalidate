namespace VueAspValidate
{
    public interface IValidator
    {
        ValidatorResult Check(object value, ValidatorContext context);
        string BuildJS(ValidatorContext context);
    }
}
