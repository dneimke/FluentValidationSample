﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> PostName(ProvideName.Request request)
        {
            var result = await _mediator.Send(request);
            return RedirectToAction("Index", new { result.Name }) ;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
