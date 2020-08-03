using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Articles.Commands;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UnitTests.Common;
using Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Application.UnitTests.Articles.Commands
{
	public class DeleteArticleCommandTests : BaseTestFixture
	{
		[Fact]
		public async Task GivenSlugOfNonExistingArticle_ThrowsEntityNotFoundException()
		{
			// Arrange
			var currentUser = Mock.Of<ICurrentUserService>();

			var command = new DeleteArticleCommand { Slug = "some slug" };

			var sut = new DeleteArticleCommand.Handler(Context, currentUser);

			// Act
			var action = new Func<Task<Unit>>(() => sut.Handle(command, CancellationToken.None));

			// Assert
			await action.Should().ThrowAsync<EntityNotFoundException<Article>>();
		}

		[Fact]
		public async Task GivenUserThatIsNotAuthor_ThrowsBadRequestException()
		{
			// Arrange
			var article = new Article("test title", "test", "test", new DateTime(1111, 1, 1),
				new UserProfile(Guid.NewGuid().ToString(), "email@email.com", "username"));

			Context.Articles.Add(article);
			await Context.SaveChangesAsync();

			var currentUser = Mock.Of<ICurrentUserService>();

			var command = new DeleteArticleCommand { Slug = article.Slug };

			var sut = new DeleteArticleCommand.Handler(Context, currentUser);

			// Act
			var action = new Func<Task<Unit>>(() => sut.Handle(command, CancellationToken.None));

			// Assert
			await action.Should().ThrowAsync<BadRequestException>();
		}

		[Fact]
		public async Task GivenSlugOfExistingArticle_DeletesArticleWithTags()
		{
			// Arrange
			var user = new UserProfile(Guid.NewGuid().ToString(), "email@email.com", "username");

			var article = new Article("test title", "test", "test", new DateTime(1111, 1, 1), user,
				new List<Tag> { new Tag("test tag") });

			Context.Articles.Add(article);
			await Context.SaveChangesAsync();

			var currentUser = Mock.Of<ICurrentUserService>(s => s.UserId == user.Id);

			var command = new DeleteArticleCommand { Slug = article.Slug };

			var sut = new DeleteArticleCommand.Handler(Context, currentUser);

			// Act
			await sut.Handle(command, CancellationToken.None);

			var existingArticle = await Context.Articles
				.SingleOrDefaultAsync(a => a.Id == article.Id);

			var tags = await Context.ArticleTags.Where(a => a.ArticleId == article.Id).ToListAsync();

			// Assert
			existingArticle.Should().BeNull();
			tags.Should().BeEmpty();
		}
	}
}
