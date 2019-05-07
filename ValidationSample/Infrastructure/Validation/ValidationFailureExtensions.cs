using FluentValidation.Results;
using System.Collections.Generic;
using System.Linq;

namespace ValidationSample.Infrastructure.Validation
{
    public static class ValidationFailureExtensions
    {
        public static Dictionary<string, List<string>> ToErrorDictionary(this IEnumerable<ValidationFailure> validationFailures)
        {
            var errors = validationFailures.ToList();
            var errorCollection = new Dictionary<string, List<string>>();

            errors.ForEach(e =>
            {
                if (errorCollection.ContainsKey(e.PropertyName))
                {
                    errorCollection[e.PropertyName].Add(e.ErrorMessage);
                }
                else
                {
                    errorCollection[e.PropertyName] = new List<string> { e.ErrorMessage };
                }
            });

            return errorCollection;
        }
    }
}
