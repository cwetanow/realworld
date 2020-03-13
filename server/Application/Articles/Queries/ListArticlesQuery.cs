using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Articles.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common.Helpers;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Articles.Queries
{
	public class ListArticlesQuery : IRequest<ArticleListDto>
	{
		public string Tag { get; set; }
		public string Author { get; set; }
		public string Favorited { get; set; }

		public int Limit { get; set; } = 20;
		public int Offset { get; set; } = 0;

		public class Handler : IRequestHandler<ListArticlesQuery, ArticleListDto>
		{
			private readonly DbContext context;
			private readonly IMapper mapper;

			public Handler(DbContext context, IMapper mapper)
			{
				this.context = context;
				this.mapper = mapper;
			}

			public async Task<ArticleListDto> Handle(ListArticlesQuery request, CancellationToken cancellationToken)
			{
				var whereExpression = PredicateBuilder.True<Article>();

				if (!string.IsNullOrEmpty(request.Author))
				{
					whereExpression = whereExpression
						.And(a => a.Author.Username == request.Author);
				}

				if (!string.IsNullOrEmpty(request.Tag))
				{
					whereExpression = whereExpression
						.And(a => a.Tags.Any(t => t.Tag.Value == request.Tag));
				}

				//if (!string.IsNullOrEmpty(request.Favorited))
				//{
				//	whereExpression = whereExpression
				//		.And(a => a.Author.Username == request.Author);
				//}

				var articles = await context.Set<Article>()
					.Where(whereExpression)
					.ProjectTo<ArticleDto>(mapper.ConfigurationProvider)
					.OrderByDescending(a => a.UpdatedAt)
					.Skip(request.Offset)
					.Take(request.Limit)
					.ToListAsync(cancellationToken);

				return mapper.Map<ArticleListDto>(articles);
			}
		}
	}
}
