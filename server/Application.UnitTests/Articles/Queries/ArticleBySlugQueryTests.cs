using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Articles.Models;
using Application.Articles.Queries;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.UnitTests.Common;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Articles.Queries
{
	public class ArticleBySlugQueryTests : BaseTestFixture
	{
		[Theory]
		[InlineData("slug")]
		public void TestHandle_ArticleDoesntExist_ShouldThrowEntityNotFoundException(string slug)
		{
			// Arrange
			var query = new ArticleBySlugQuery { Slug = slug };

			var currentUserMock = new Mock<ICurrentUserService>();

			var sut = new ArticleBySlugQuery.Handler(Context, Mapper, currentUserMock.Object);

			// Act
			var act = new Func<Task<ArticleDto>>(async () => await sut.Handle(query, CancellationToken.None));

			// Assert
			act.Should().Throw<EntityNotFoundException<Article>>().And.Message.Should().Contain(query.Slug);
		}

		[Theory]
		[InlineData("slug")]
		public async Task TestHandle_ShouldReturnCorrectly(string slug)
		{
			// Arrange
			var article = new Article(slug, "description", "body", new DateTime(1, 2, 3), new UserProfile(Guid.NewGuid().ToString(), "email", "username"));

			Context.Articles.Add(article);
			await Context.SaveChangesAsync();

			var expected = Mapper.Map<ArticleDto>(article);

			var query = new ArticleBySlugQuery { Slug = slug };

			var currentUserMock = new Mock<ICurrentUserService>();

			var sut = new ArticleBySlugQuery.Handler(Context, Mapper, currentUserMock.Object);

			// Act
			var actualResult = await sut.Handle(query, CancellationToken.None);

			// Assert
			actualResult.Should().BeEquivalentTo(expected);
		}

		[Theory]
		[InlineData("slug")]
		public async Task TestHandle_WithUserFollower_ShouldSetAuthorIsFollowingCorrectly(string slug)
		{
			// Arrange
			var user = new UserProfile(Guid.NewGuid().ToString(), "currentUserEmail", "currentUserUsername");
			var author = new UserProfile(Guid.NewGuid().ToString(), "authorEmail", "authorUsername");

			Context.UserFollowers.Add(new UserFollower(author, user));
			await Context.SaveChangesAsync();

			var article = new Article(slug, "description", "body", new DateTime(1, 2, 3), author);

			Context.Articles.Add(article);
			await Context.SaveChangesAsync();

			var expected = Mapper.Map<ArticleDto>(article);

			var query = new ArticleBySlugQuery { Slug = slug };

			var currentUser = Mock.Of<ICurrentUserService>(s => s.UserId == user.Id
			                                                    && s.IsAuthenticated == true);

			var sut = new ArticleBySlugQuery.Handler(Context, Mapper, currentUser);

			// Act
			var actualResult = await sut.Handle(query, CancellationToken.None);

			// Assert
			actualResult.Should().BeEquivalentTo(expected,
				opts => opts.Excluding(dto => dto.Author.Following));
			actualResult.Author.Following.Should().BeTrue();
		}

		[Theory]
		[InlineData("slug")]
		public async Task TestHandle_UserIsNotFollower_ShouldSetAuthorIsFollowingCorrectly(string slug)
		{
			// Arrange
			var currentUser = new UserProfile(Guid.NewGuid().ToString(), "currentUserEmail", "currentUserUsername");
			Context.UserProfiles.Add(currentUser);

			var author = new UserProfile(Guid.NewGuid().ToString(), "authorEmail", "authorUsername");
			var article = new Article(slug, "description", "body", new DateTime(1, 2, 3), author);

			Context.Articles.Add(article);
			await Context.SaveChangesAsync();

			var expected = Mapper.Map<ArticleDto>(article);

			var query = new ArticleBySlugQuery { Slug = slug };

			var currentUserMock = new Mock<ICurrentUserService>();
			currentUserMock.Setup(s => s.IsAuthenticated).Returns(true);
			currentUserMock.Setup(s => s.Email).Returns(currentUser.Email);

			var sut = new ArticleBySlugQuery.Handler(Context, Mapper, currentUserMock.Object);

			// Act
			var actualResult = await sut.Handle(query, CancellationToken.None);

			// Assert
			actualResult.Should().BeEquivalentTo(expected,
				opts => opts.Excluding(dto => dto.Author.Following));
			actualResult.Author.Following.Should().BeFalse();
		}
	}
}
