using FluentValidation;

namespace StudentAutomation.Core.Utilities.Validation
{
    public static class ValidationTool
    {
        public static async Task ValidateAsync(IValidator validator, object entity)
        {
            var context = new ValidationContext<object>(entity);
            var result = await validator.ValidateAsync(context);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
        public static void Validate(IValidator validator, object entity)
        {
            var context = new ValidationContext<object>(entity);
            var result = validator.Validate(context);

            if (!result.IsValid)
            {
                throw new ValidationException(result.Errors);
            }
        }
    }

}
