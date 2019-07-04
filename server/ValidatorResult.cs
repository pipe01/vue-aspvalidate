namespace VueAspValidate
{
    public sealed class ValidatorResult
    {
        public static ValidatorResult Success { get; } = new ValidatorResult();

        public bool Successful { get; } = true;
        public string Error { get; }

        public bool Stop { get; }

        public ValidatorResult()
        {
        }

        public ValidatorResult(string error, bool stop)
        {
            this.Successful = false;
            this.Error = error;
            this.Stop = stop;
        }

        public static implicit operator ValidatorResult(string error)
            => new ValidatorResult(error, false);

        public static implicit operator ValidatorResult(bool success)
            => success ? new ValidatorResult() : new ValidatorResult(null, false);
    }
}
