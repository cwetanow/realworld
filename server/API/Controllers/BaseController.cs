using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public abstract class BaseController : ControllerBase
	{
		protected readonly IMediator mediator;

		protected BaseController(IMediator mediator)
		{
			this.mediator = mediator;
		}
	}
}
