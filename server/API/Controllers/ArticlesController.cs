using System.Threading.Tasks;
using API.Requests;
using Application.Articles.Commands;
using Application.Articles.Models;
using Application.Articles.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class ArticlesController : BaseController
	{
		public ArticlesController(IMediator mediator) : base(mediator)
		{
		}

		[HttpPost]
		public async Task<ArticleDto> CreateArticle([FromBody] ArticleRequest<CreateArticleCommand> request)
		{
			var id = await mediator.Send(request.Article);

			return await mediator.Send(new ArticleByIdQuery { Id = id });
		}

		[HttpGet]
		[AllowAnonymous]
		public async Task<ArticleListDto> ListArticles([FromQuery] ListArticlesQuery query) => await mediator.Send(query);

		[HttpGet("{slug}")]
		[AllowAnonymous]
		public async Task<ArticleDto> GetArticle(string slug) => await mediator.Send(new ArticleBySlugQuery { Slug = slug });
	}
}
