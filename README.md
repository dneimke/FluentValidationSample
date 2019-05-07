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

## Run the page

You should now be able to run the web application using F5 and, by default you should be presented with 
the following page:

[[https://github.com/dneimke/FluentValidationSample/blob/master/images/home-1.png|alt=HomePage]]

Now change the URL to include a name by adding ```?name=Fred Flinstone``` to the end of the URL path and 
press ENTER.  You should now be presented with the following page:

[[https://github.com/dneimke/FluentValidationSample/blob/master/images/home-2.png|alt=HomePage]]

