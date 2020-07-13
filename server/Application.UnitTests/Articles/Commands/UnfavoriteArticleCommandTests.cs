using System;
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
	public class UnfavoriteArticleCommandTests : BaseTestFixture
	{
		[Fact]
		public async Task GivenArticleIsNotFavourited_ShouldThrowEntityNotFoundException()
		{
			// Arrange
			var currentUserMock = new Mock<ICurrentUserService>();

			var command = new UnfavoriteArticleCommand() { Slug = "some slug" };

			var sut = new UnfavoriteArticleCommand.Handler(Context, currentUserMock.Object);

			// Act
			var action = new Func<Task<Unit>>(() => sut.Handle(command, CancellationToken.None));

			// Assert
			await action.Should().ThrowAsync<EntityNotFoundException<FavouritedArticle>>();
		}

		[Fact]
		public async Task GivenArticleIsFavourited_ShouldUnfavouriteArticle()
		{
			// Arrange
			var user = new UserProfile(Guid.NewGuid().ToString(), "email@email.com", "username");

			var article = new Article("test title", "test", "test", new DateTime(1111, 1, 1), user);

			var favouritedArticle = new FavouritedArticle(article, user);

			Context.Add(favouritedArticle);
			await Context.SaveChangesAsync();

			var currentUserMock = new Mock<ICurrentUserService>();
			currentUserMock.Setup(c => c.Email).Returns(user.Email);

			var command = new UnfavoriteArticleCommand { Slug = article.Slug };

			var sut = new UnfavoriteArticleCommand.Handler(Context, currentUserMock.Object);

			// Act
			await sut.Handle(command, CancellationToken.None);

			var existingFavouritedArticle = await Context.FavouritedArticles
				.SingleOrDefaultAsync();

			// Assert
			existingFavouritedArticle.Should().BeNull();
		}
	}
}
