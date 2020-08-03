using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Articles.Models;
using Application.Articles.Queries;
using Application.Common.Interfaces;
using Application.UnitTests.Common;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Articles.Queries
{
	public class ListArticlesQueryTests : BaseTestFixture
	{
		[Fact]
		public async Task TestHandle_ShouldReturnCorrectly()
		{
			// Arrange
			var author = new UserProfile(Guid.NewGuid().ToString(), "email", "username");

			var articles = new List<Article>
			{
				new Article("first title","first description","first body",new DateTime(1,2,3),author),
				new Article("second title","second description","second body",new DateTime(2,2,3),author),
			};

			Context.Articles.AddRange(articles);
			await Context.SaveChangesAsync();

			var expected = new ArticleListDto {
				Articles = articles.OrderByDescending(a => a.UpdatedAt).Select(a => Mapper.Map<ArticleDto>(a))
			};

			var query = new ListArticlesQuery();

			var currentUserMock = new Mock<ICurrentUserService>();

			var sut = new ListArticlesQuery.Handler(Context, Mapper, currentUserMock.Object);

			// Act
			var result = await sut.Handle(query, CancellationToken.None);

			// Assert
			result.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public async Task TestHandle_WithTag_ShouldFilterCorrectly()
		{
			// Arrange
			var author = new UserProfile(Guid.NewGuid().ToString(), "email", "username");
			var tag = "tag";

			var articles = new List<Article>
			{
				new Article("first title","first description","first body",new DateTime(1,2,3),author),
				new Article("second title","second description","second body",new DateTime(2,2,3),author, new List<Tag>{new Tag(tag)}),
			};

			Context.Articles.AddRange(articles);
			await Context.SaveChangesAsync();

			var expected = new ArticleListDto {
				Articles = new List<ArticleDto> { Mapper.Map<ArticleDto>(articles[1]) }
			};

			var query = new ListArticlesQuery { Tag = tag };

			var currentUserMock = new Mock<ICurrentUserService>();

			var sut = new ListArticlesQuery.Handler(Context, Mapper, currentUserMock.Object);

			// Act
			var result = await sut.Handle(query, CancellationToken.None);

			// Assert
			result.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public async Task TestHandle_WithAuthor_ShouldFilterCorrectly()
		{
			// Arrange
			var author = new UserProfile(Guid.NewGuid().ToString(), "email", "username");
			var otherAuthor = new UserProfile(Guid.NewGuid().ToString(), "otherAuthorEmail", "otherAuthorUsername");

			var articles = new List<Article>
			{
				new Article("first title","first description","first body",new DateTime(1,2,3),author),
				new Article("second title","second description","second body",new DateTime(2,2,3),otherAuthor),
			};

			Context.Articles.AddRange(articles);
			await Context.SaveChangesAsync();

			var expected = new ArticleListDto {
				Articles = new List<ArticleDto> { Mapper.Map<ArticleDto>(articles[0]) }
			};

			var query = new ListArticlesQuery { Author = author.Username };

			var currentUserMock = new Mock<ICurrentUserService>();

			var sut = new ListArticlesQuery.Handler(Context, Mapper, currentUserMock.Object);

			// Act
			var result = await sut.Handle(query, CancellationToken.None);

			// Assert
			result.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public async Task TestHandle_WithAuthenticatedUser_ShouldSetAuthorFollowedCorrectly()
		{
			// Arrange
			var user = new UserProfile(Guid.NewGuid().ToString(), "currentUser", "currentUser");

			var followedAuthor = new UserProfile(Guid.NewGuid().ToString(), "email", "username");
			var notFollowedAuthor = new UserProfile(Guid.NewGuid().ToString(), "otherAuthorEmail", "otherAuthorUsername");

			Context.UserFollowers.Add(new UserFollower(followedAuthor, user));

			var articles = new List<Article>
			{
				new Article("first title","first description","first body",new DateTime(1,2,3),followedAuthor),
				new Article("second title","second description","second body",new DateTime(2,2,3),notFollowedAuthor),
			};

			Context.Articles.AddRange(articles);
			await Context.SaveChangesAsync();

			var query = new ListArticlesQuery();

			var currentUser = Mock.Of<ICurrentUserService>(s => s.UserId == user.Id
			                                                    && s.IsAuthenticated == true);

			var sut = new ListArticlesQuery.Handler(Context, Mapper, currentUser);

			// Act
			var result = await sut.Handle(query, CancellationToken.None);

			var articleDtos = result.Articles.ToList();

			// Assert
			articleDtos[0].Author.Following.Should().BeFalse();
			articleDtos[1].Author.Following.Should().BeTrue();
		}

		[Fact]
		public async Task TestHandle_ShouldSkipAndTakeCorrectNumberOfItems()
		{
			// Arrange
			var author = new UserProfile(Guid.NewGuid().ToString(), "email", "username");
			var otherAuthor = new UserProfile(Guid.NewGuid().ToString(), "otherAuthorEmail", "otherAuthorUsername");

			var articles = new List<Article>
			{
				new Article("first title","first description","first body",new DateTime(1,2,3),author),
				new Article("second title","second description","second body",new DateTime(2,2,3),otherAuthor),
			};

			Context.Articles.AddRange(articles);
			await Context.SaveChangesAsync();

			var expected = new ArticleListDto {
				Articles = new List<ArticleDto> { Mapper.Map<ArticleDto>(articles[0]) }
			};

			var query = new ListArticlesQuery { Limit = 1, Offset = 1 };

			var currentUserMock = new Mock<ICurrentUserService>();

			var sut = new ListArticlesQuery.Handler(Context, Mapper, currentUserMock.Object);

			// Act
			var result = await sut.Handle(query, CancellationToken.None);

			// Assert
			result.Should().BeEquivalentTo(expected);
		}
	}
}
