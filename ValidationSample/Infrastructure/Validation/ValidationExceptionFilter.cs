using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Net;

namespace ValidationSample.Infrastructure.Validation
{
    public class ValidationExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var validationException = context.Exception as ValidationException;

            if (validationException != null)
            {
                var problemDetails = GetValidationProblemDetails(validationException);
                context.ExceptionHandled = true;
                context.Result = new JsonResult(problemDetails);
            }
        }

        private ValidationProblemDetails GetValidationProblemDetails(ValidationException validationException)
        {
            if (validationException == null) return null;

            var errorCollection = validationException.Errors.ToErrorDictionary();

            var validationProblemDetails = new ValidationProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Detail = string.Join(Environment.NewLine, validationException.Errors.Select(f => f.ErrorMessage))
            };
            foreach (var error in errorCollection)
            {
                validationProblemDetails.Errors.Add(error.Key, error.Value.ToArray());
            }

            return validationProblemDetails;
        }
    }
}
