using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Articles.Models;
using Application.Common.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Articles.Queries
{
	public class ArticleByIdQuery : IRequest<ArticleDto>
	{
		public int Id { get; set; }

		public class Handler : IRequestHandler<ArticleByIdQuery, ArticleDto>
		{
			private readonly DbContext context;
			private readonly IMapper mapper;

			public Handler(DbContext context, IMapper mapper)
			{
				this.context = context;
				this.mapper = mapper;
			}

			public async Task<ArticleDto> Handle(ArticleByIdQuery request, CancellationToken cancellationToken)
			{
				var article = await context.Set<Article>()
					.AsNoTracking()
					.Where(a => a.Id == request.Id)
					.ProjectTo<ArticleDto>(mapper.ConfigurationProvider)
					.SingleOrDefaultAsync(cancellationToken);

				if (article == null)
				{
					throw new EntityNotFoundException<Article>(request.Id);
				}

				return article;
			}
		}
	}
}
