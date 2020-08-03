using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Articles.Models;
using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Articles.Queries
{
	public class FeedArticlesQuery : IRequest<ArticleListDto>
	{
		public int Limit { get; set; } = 20;
		public int Offset { get; set; } = 0;

		public class Handler : IRequestHandler<FeedArticlesQuery, ArticleListDto>
		{
			private readonly DbContext context;
			private readonly IMapper mapper;
			private readonly ICurrentUserService currentUser;

			public Handler(DbContext context, IMapper mapper, ICurrentUserService currentUser)
			{
				this.context = context;
				this.mapper = mapper;
				this.currentUser = currentUser;
			}

			public async Task<ArticleListDto> Handle(FeedArticlesQuery request, CancellationToken cancellationToken)
			{
				var articles = await context.Set<Article>()
					.Where(a => a.Author.Followers.Any(f => f.FollowerId == currentUser.UserId))
					.OrderByDescending(a => a.UpdatedAt)
					.Skip(request.Offset)
					.Take(request.Limit)
					.ProjectTo<ArticleDto>(mapper.ConfigurationProvider)
					.ToListAsync(cancellationToken);

				foreach (var article in articles)
				{
					article.Author.Following = true;
				}

				return mapper.Map<ArticleListDto>(articles);
			}
		}
	}
}