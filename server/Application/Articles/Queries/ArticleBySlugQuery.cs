using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Articles.Models;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Articles.Queries
{
	public class ArticleBySlugQuery : IRequest<ArticleDto>
	{
		public string Slug { get; set; }

		public class Handler : IRequestHandler<ArticleBySlugQuery, ArticleDto>
		{
			private readonly DbContext context;
			private readonly IMapper mapper;
			private readonly ICurrentUserService currentUser;

			public Handler(DbContext context, IMapper mapper, ICurrentUserService currentUser)
			{
				this.mapper = mapper;
				this.context = context;
				this.currentUser = currentUser;
			}

			public async Task<ArticleDto> Handle(ArticleBySlugQuery request, CancellationToken cancellationToken)
			{
				var article = await context.Set<Article>()
					.AsNoTracking()
					.Where(a => a.Slug == request.Slug)
					.ProjectTo<ArticleDto>(mapper.ConfigurationProvider)
					.SingleOrDefaultAsync(cancellationToken);

				if (article == null)
				{
					throw new EntityNotFoundException<Article>(request.Slug);
				}

				if (currentUser.IsAuthenticated)
				{
					article.Author.Following = await context.Set<UserFollower>()
						.AnyAsync(f => f.User.Username == article.Author.Username && f.Follower.Email == currentUser.Email, cancellationToken);
				}

				return article;
			}
		}

		public class Validator : AbstractValidator<ArticleBySlugQuery>
		{
			public Validator()
			{
				RuleFor(q => q.Slug).NotEmpty();
			}
		}
	}
}
