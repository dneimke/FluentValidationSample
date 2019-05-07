using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ValidationSample.Features.Home
{
    public static class ProvideName
    {
        public class Request : IRequest<ViewModel>
        {
            public string Name { get; set; }
        }

        public class ViewModel
        {
            public string Name { get; set; }
        }

        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(x => x.Name)
                    .Cascade(CascadeMode.StopOnFirstFailure)
                    .NotEmpty()
                    .Must((r, x) => !string.IsNullOrWhiteSpace(r.Name))
                    .WithMessage("'Name' should not be empty.")
                    .MinimumLength(5)
                    .MaximumLength(50);
            }
        }

        public class RequestHandler : IRequestHandler<Request, ViewModel>
        {
            public Task<ViewModel> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new ViewModel { Name = request.Name });
            }
        }
    }
}
