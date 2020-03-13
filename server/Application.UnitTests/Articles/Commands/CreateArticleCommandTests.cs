using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Articles.Commands;
using Application.Common.Interfaces;
using Application.UnitTests.Common;
using Domain.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Application.UnitTests.Articles.Commands
{
	public class CreateArticleCommandTests : BaseTestFixture
	{
		[Theory]
		[InlineData("title", "description", "body")]
		public async Task TestHandle_WithoutTags_ShouldCreateEntityWithCorrectProperties(string title, string description, string body)
		{
			// Arrange
			var userEmail = "email";
			var author = new UserProfile(Guid.NewGuid().ToString(), userEmail, "username");

			Context.UserProfiles.Add(author);
			await Context.SaveChangesAsync();

			var command = new CreateArticleCommand {
				Title = title,
				Description = description,
				Body = body
			};

			var currentUserMock = new Mock<ICurrentUserService>();
			currentUserMock.Setup(s => s.Email).Returns(userEmail);

			var sut = new CreateArticleCommand.Handler(currentUserMock.Object, Context);

			// Act
			await sut.Handle(command, CancellationToken.None);

			var article = await Context.Articles
				.Include(a => a.Tags)
				.Include(a => a.Author)
				.SingleOrDefaultAsync();

			// Assert
			article.Should().NotBeNull();
			article.Body.Should().Be(body);
			article.Description.Should().Be(description);
			article.Title.Should().Be(title);
			article.Tags.Should().BeEmpty();
			article.Author.Should().BeEquivalentTo(author);
		}

		[Theory]
		[InlineData("title", "description", "body")]
		public async Task TestHandle_WithTags_ShouldCreateCorrectTags(string title, string description, string body)
		{
			// Arrange
			var existingTag = new Tag("existingTag");
			var newTagValue = "newTagValue";

			var userEmail = "email";
			var author = new UserProfile(Guid.NewGuid().ToString(), userEmail, "username");

			Context.UserProfiles.Add(author);
			Context.Tags.Add(existingTag);
			await Context.SaveChangesAsync();

			var command = new CreateArticleCommand {
				Title = title,
				Description = description,
				Body = body,
				TagList = new List<string> { newTagValue, existingTag.Value }
			};

			var expectedTags = new List<Tag>
			{
				new Tag(newTagValue),
				existingTag
			};

			var currentUserMock = new Mock<ICurrentUserService>();
			currentUserMock.Setup(s => s.Email).Returns(userEmail);

			var sut = new CreateArticleCommand.Handler(currentUserMock.Object, Context);

			// Act
			await sut.Handle(command, CancellationToken.None);

			var article = await Context.Articles
				.Include(a => a.Tags)
				.SingleOrDefaultAsync();

			var tags = article.Tags.Select(t => t.Tag);

			// Assert
			article.Should().NotBeNull();
			tags.Should().BeEquivalentTo(expectedTags, opts => opts.Excluding(t => t.Id));
		}
	}
}
