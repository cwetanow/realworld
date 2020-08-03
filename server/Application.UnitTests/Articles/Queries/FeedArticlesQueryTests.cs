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
	public class FeedArticlesQueryTests : BaseTestFixture
	{
		[Fact]
		public async Task TestHandle_ShouldReturnCorrectly()
		{
			// Arrange
			var user = new UserProfile(Guid.NewGuid().ToString(), "currentUser", "currentUser");
			var author = new UserProfile(Guid.NewGuid().ToString(), "author", "author");

			var articles = new List<Article>
			{
				new Article("first title", "first description", "first body", new DateTime(1, 2, 3), author),
				new Article("second title", "second description", "second body", new DateTime(2, 2, 3), author),
				new Article("third title", "third description", "third body", new DateTime(3, 3, 3), user),
			};

			Context.Articles.AddRange(articles);
			Context.UserFollowers.Add(new UserFollower(author, user));
			await Context.SaveChangesAsync();

			var expected = new ArticleListDto {
				Articles = articles
					.Take(2)
					.OrderByDescending(a => a.UpdatedAt)
					.Select(a => {
						var dto = Mapper.Map<ArticleDto>(a);
						dto.Author.Following = true;
						return dto;
					})
			};

			var query = new FeedArticlesQuery();

			var currentUser = Mock.Of<ICurrentUserService>(s => s.UserId == user.Id);

			var sut = new FeedArticlesQuery.Handler(Context, Mapper, currentUser);

			// Act
			var result = await sut.Handle(query, CancellationToken.None);

			// Assert
			result.Should().BeEquivalentTo(expected);
		}

		[Fact]
		public async Task TestHandle_ShouldSkipAndTakeCorrectNumberOfItems()
		{
			// Arrange
			var user = new UserProfile(Guid.NewGuid().ToString(), "currentUser", "currentUser");
			var author = new UserProfile(Guid.NewGuid().ToString(), "author", "author");

			var articles = new List<Article>
			{
				new Article("first title", "first description", "first body", new DateTime(1, 2, 3), author),
				new Article("second title", "second description", "second body", new DateTime(2, 2, 3), author),
				new Article("third title", "third description", "third body", new DateTime(3, 3, 3), user),
			};

			Context.Articles.AddRange(articles);
			Context.UserFollowers.Add(new UserFollower(author, user));
			await Context.SaveChangesAsync();

			var expected = new ArticleListDto {
				Articles = articles
					.Take(2)
					.OrderByDescending(a => a.UpdatedAt)
					.Skip(1)
					.Take(1)
					.Select(a => {
						var dto = Mapper.Map<ArticleDto>(a);
						dto.Author.Following = true;
						return dto;
					})
			};

			var query = new FeedArticlesQuery {
				Limit = 1,
				Offset = 1
			};

			var currentUser = Mock.Of<ICurrentUserService>(s => s.UserId == user.Id);

			var sut = new FeedArticlesQuery.Handler(Context, Mapper, currentUser);

			// Act
			var result = await sut.Handle(query, CancellationToken.None);

			// Assert
			result.Should().BeEquivalentTo(expected);
		}
	}
}