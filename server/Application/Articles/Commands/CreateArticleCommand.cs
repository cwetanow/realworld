using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Articles.Commands
{
	public class CreateArticleCommand : IRequest<int>
	{
		public string Title { get; set; }
		public string Description { get; set; }
		public string Body { get; set; }

		public IEnumerable<string> TagList { get; set; } = new List<string>();

		public class Handler : IRequestHandler<CreateArticleCommand, int>
		{
			private readonly ICurrentUserService currentUser;
			private readonly DbContext context;

			public Handler(ICurrentUserService currentUser, DbContext context)
			{
				this.currentUser = currentUser;
				this.context = context;
			}

			public async Task<int> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
			{
				var author = await context.Set<UserProfile>()
					.SingleAsync(p => p.Email == currentUser.Email, cancellationToken);

				var tags = new List<Tag>();

				if (request.TagList.Any())
				{
					var existingTags = await context.Set<Tag>()
						.Where(t => request.TagList.Any(tag => t.Value == tag))
						.ToDictionaryAsync(t => t.Value, t => t, cancellationToken);

					tags = request.TagList
						.Select(t => existingTags.ContainsKey(t) ? existingTags[t] : new Tag(t))
						.ToList();
				}

				var article = new Article(request.Title, request.Description, request.Body, DateTime.UtcNow, author, tags);

				context.Set<Article>().Add(article);
				await context.SaveChangesAsync(cancellationToken);

				return article.Id;
			}
		}

		public class Validator : AbstractValidator<CreateArticleCommand>
		{
			public Validator()
			{
				RuleFor(c => c.Title).NotEmpty();
				RuleFor(c => c.Description).NotEmpty();
				RuleFor(c => c.Body).NotEmpty();
			}
		}
	}
}
