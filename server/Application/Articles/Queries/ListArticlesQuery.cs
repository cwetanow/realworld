using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Articles.Models;
using Application.Common.Interfaces;
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
			private readonly ICurrentUserService currentUser;

			public Handler(DbContext context, IMapper mapper, ICurrentUserService currentUser)
			{
				this.context = context;
				this.mapper = mapper;
				this.currentUser = currentUser;
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

				if (currentUser.IsAuthenticated)
				{
					var currentUserFollowedAuthors = context.Set<UserFollower>()
						.Where(f => f.Follower.Email == currentUser.Email)
						.Select(f => f.User.Username)
						.Distinct()
						.ToHashSet();

					foreach (var article in articles)
					{
						if (currentUserFollowedAuthors.Contains(article.Author.Username))
						{
							article.Author.Following = true;
						}
					}
				}

				return mapper.Map<ArticleListDto>(articles);
			}
		}
	}
}
