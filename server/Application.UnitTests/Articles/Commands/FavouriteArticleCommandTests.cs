using System;
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
	public class FavouriteArticleCommandTests : BaseTestFixture
	{
		[Theory]
		[InlineData("slug")]
		public void TestHandle_ArticleNotFound_ShouldThrowEntityNotFoundException(string slug)
		{
			// Arrange
			var currentUserMock = new Mock<ICurrentUserService>();

			var command = new FavouriteArticleCommand() {
				Slug = slug
			};

			var sut = new FavouriteArticleCommand.Handler(Context, currentUserMock.Object);

			// Act
			var act = new Func<Task<Unit>>(async () => await sut.Handle(command, CancellationToken.None));

			// Assert
			act.Should().Throw<EntityNotFoundException<Article>>().And.Message.Should().Contain(slug);
		}

		[Theory]
		[InlineData("slug", "email")]
		public async Task TestHandle_ArticleIsAlreadyFavourite_ShouldThrowBadRequestException(string slug, string email)
		{
			// Arrange
			var user = new UserProfile(Guid.NewGuid().ToString(), email, email);
			var article = new Article(slug, null, null, new DateTime(), user);

			user.FavouriteArticle(article);

			Context.UserProfiles.Add(user);
			await Context.SaveChangesAsync();

			var currentUser = Mock.Of<ICurrentUserService>(s => s.UserId == user.Id);

			var command = new FavouriteArticleCommand {
				Slug = slug
			};

			var sut = new FavouriteArticleCommand.Handler(Context, currentUser);

			// Act
			var act = new Func<Task<Unit>>(async () => await sut.Handle(command, CancellationToken.None));

			// Assert
			act.Should().Throw<BadRequestException>();
		}

		[Theory]
		[InlineData("slug", "email")]
		public async Task TestHandle_ArticleIsNotFavourite_ShouldCreateNew(string slug, string email)
		{
			// Arrange
			var user = new UserProfile(Guid.NewGuid().ToString(), email, email);
			var article = new Article(slug, null, null, new DateTime(), user);

			Context.Add(user);
			Context.Add(article);
			await Context.SaveChangesAsync();

			var currentUser = Mock.Of<ICurrentUserService>(s => s.UserId == user.Id);

			var command = new FavouriteArticleCommand() {
				Slug = slug
			};

			var sut = new FavouriteArticleCommand.Handler(Context, currentUser);

			// Act
			await sut.Handle(command, CancellationToken.None);

			var favouritedArticle = await Context.FavouritedArticles.SingleOrDefaultAsync();

			// Assert
			favouritedArticle.Should().NotBeNull();
			favouritedArticle.UserId.Should().Be(user.Id);
			favouritedArticle.ArticleId.Should().Be(article.Id);
		}
	}
}