using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using System.Threading.Tasks;
using ValidationSample.Features.Home;
using ValidationSample.Models;

namespace ValidationSample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(WelcomeMessage.Request request)
        {
            var result = await _mediator.Send(request);
            return View(result);
        }


        [HttpPost]
        public async Task<IActionResult> Index(ProvideName.ProvideNameRequest request)
        {
            if(!ModelState.IsValid)
            {
                var tmp = await _mediator.Send(new WelcomeMessage.Request());
                return View(tmp);
            }

            var result = await _mediator.Send(request);
            return RedirectToAction("Index", new WelcomeMessage.Request { Name = result.Name }) ;
        }
    }
}
