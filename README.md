# FluentValidationSample
Demonstrating how to use FluentValidation with MediatR in an ASP.NET Core application.

## Create new ASP.NET Core Application

Open Visual Studio and create a new .NET Core MVC web application.

Remove non-critical concerns:

* Privacy view and controller action
* Environmental-specific CSS and JavaScript includes

## Add MediatR

Add MediatR requirements:

* Install-Package MediatR
* Install-Package MediatR.Extensions.Microsoft.DependencyInjection

And add MediatR services to the ASP.NET Core application by adding the following code to ConfigureServices in Startup.cs:

```csharp
services.AddMediatR(Assembly.GetAssembly(typeof(Program)));
```

## Add MediatR handler

Create a simple MediatR handler to display a welcome message on the Home page:

```csharp
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
```

## Call the MedaitR handler

Change the Home controller's Index action so that it can accept a WelcomeMessage.Request and return the response
as the Model for the View.

```csharp
[HttpGet]
public async Task<IActionResult> Index(WelcomeMessage.Request request)
{
    var result = await _mediator.Send(request);
    return View(result);
}
```

And update the Index view to display the welcome message.

```html
@model  ValidationSample.Features.Home.WelcomeMessage.ViewModel;
@{
    ViewData["Title"] = "Home Page";
}

<div class="text-center">
    <h1 class="display-4">@Model.Message</h1>
</div>
```

## Test MediatR configuration

You should now be able to run the web application using F5 and, by default you should be presented with 
the following page:

![Default Home Page](https://github.com/dneimke/FluentValidationSample/blob/master/images/home-1.png)

Now change the URL to include a name by adding ```?name=Fred Flinstone``` to the end of the URL path and 
press ENTER.  You should now be presented with the following page:

![Home Page with name](https://github.com/dneimke/FluentValidationSample/blob/master/images/home-2.png)


## Allow the user to supply a name

Add a form to the home page to allow the user to input a name to display

```html
<form method="post" asp-controller="Home" asp-action="PostName">
    <input type="text" name="Name" />
    <input type="submit" />
</form>
```

And add a controller action to handle the POST request

```csharp

```[HttpPost]
public async Task<IActionResult> PostName(ProvideName.Request request)
{
    var result = await _mediator.Send(request);
    return RedirectToAction("Index", new { result.Name }) ;
}
```

Next, create the ProvideName handler to process the request

```csharp
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

        public class RequestHandler : IRequestHandler<Request, ViewModel>
        {
            public Task<ViewModel> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult(new ViewModel { Name = request.Name });
            }
        }
    }
}
```

This is a fairly useless handler since all it does for now is to return the input that it is passed.  Next we will validate the 
Name value using the FluentValidation assembly.

## Add FluentValidation

* Install-Package FluentValidation

Unlike MediatR, FluentValidation doesn't have an out-of-the-box way to register validators.  We will use [Scrutor](https://github.com/khellang/Scrutor) 
to scan our assembly and register validators.  We will also use Scrutor's decorator capabilities to match validators with MediatR handlers.

* Install-Package Scrutor

With Scrutor installed, add the following code in RegisterServices to scan for and add any FluentValidator classes to our DI container

```csharp
services.Scan(s => s
    .FromAssemblyOf<Program>()
    .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
    .AsImplementedInterfaces()
    .WithTransientLifetime()
);
```

In addition to the Scan extension, Scrutor provides us with a Decorate method that can be used to decorate already registered services.
We will use this to wrap our MediatR handlers with any validators that have the same type signature.  First, create a Decorator class like so

```csharp
namespace ValidationSample.Infrastructure
{
    public class ValidationDecorator<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IRequestHandler<TRequest, TResponse> _inner;
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationDecorator(IRequestHandler<TRequest, TResponse> inner, IEnumerable<IValidator<TRequest>> validators)
        {
            _inner = inner;
            _validators = validators;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();
            if (failures.Any())
                throw new ValidationException(failures);

            return _inner.Handle(request, cancellationToken);
        }
    }
}
```

Finally, register the decorator class with Scrutor in Startup.cs

```csharp
services.Decorate(typeof(IRequestHandler<,>), typeof(ValidationDecorator<,>));
```

## Create a Validator 

Add the following validator class inside of the ProvideName static class

```csharp
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
```

## Test FluentValidation configuration

Run the web application using F5 and, press submit without entering any text.  You should be presented with 
the following error page:

![Home Page with validation error](https://github.com/dneimke/FluentValidationSample/blob/master/images/home-3.png)

## Handle Errors


