using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ValidationSample.Features.Home
{
    public static class WelcomeMessage
    {
        public class Request : IRequest<ViewModel>
        {
            public string Name { get; set; }
        }

        public class ViewModel
        {
            public string Message { get; set; }
        }

        public class RequestHandler : IRequestHandler<Request, ViewModel>
        {
            public Task<ViewModel> Handle(Request request, CancellationToken cancellationToken)
            {
                var message = string.IsNullOrEmpty(request.Name) ? "Who are you?" : $"Hello {request.Name}";
                return Task.FromResult(new ViewModel { Message = message });
            }
        }
    }
}
