using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Articles.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
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
			private readonly ICurrentUserService currentUser;

			public Handler(DbContext context, IMapper mapper, ICurrentUserService currentUser)
			{
				this.context = context;
				this.mapper = mapper;
				this.currentUser = currentUser;
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

				if (currentUser.IsAuthenticated)
				{
					article.Author.Following = await context.Set<UserFollower>()
						.AnyAsync(f => f.User.Username == article.Author.Username && f.Follower.Email == currentUser.Email, cancellationToken);
				}

				return article;
			}
		}
	}
}
