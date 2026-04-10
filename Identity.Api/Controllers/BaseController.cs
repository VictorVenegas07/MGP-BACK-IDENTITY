using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Identity.Api.Common;

namespace Identity.Api.Controllers;

/// <summary>
/// Base Controller 
/// </summary>

[Route(BaseRoute.BaseRouteUrl)]
[ApiController]
public class BaseController : ControllerBase
{
    private IMediator _mediator = null!;
    /// <summary>
    ///  Mediator 
    /// </summary>
    public IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;
}
