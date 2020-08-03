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
    public class UpdateArticleCommandTests : BaseTestFixture
    {
        [Theory]
        [InlineData("article-title")]
        public void TestHandle_ArticleNotFound_ShouldThrowEntityNotFoundException(string slug)
        {
            // Arrange
            var currentUserMock = new Mock<ICurrentUserService>();

            var command = new UpdateArticleCommand
            {
                Slug = slug
            };

            var sut = new UpdateArticleCommand.Handler(Context, currentUserMock.Object);

            // Act
            var act = new Func<Task<Unit>>(async () => await sut.Handle(command, CancellationToken.None));

            // Assert
            act.Should().Throw<EntityNotFoundException<Article>>().And.Message.Should().Contain(slug);
        }

        [Theory]
        [InlineData("article-title")]
        public async Task TestHandle_UserIsNotAuthor_ShouldThrowBadRequestException(string slug)
        {
            // Arrange
            var article = new Article(slug, null, null, new DateTime(),
                new UserProfile(string.Empty, string.Empty, string.Empty));

            Context.Articles.Add(article);
            await Context.SaveChangesAsync();

            var currentUserMock = new Mock<ICurrentUserService>();

            var command = new UpdateArticleCommand
            {
                Slug = slug
            };

            var sut = new UpdateArticleCommand.Handler(Context, currentUserMock.Object);

            // Act
            var act = new Func<Task<Unit>>(async () => await sut.Handle(command, CancellationToken.None));

            // Assert
            act.Should().Throw<BadRequestException>();
        }

        [Theory]
        [InlineData("article-title", "new-article-title", "new description", "new body")]
        public async Task TestHandle_ShouldUpdateArticleCorrectly(string slug, string newTitle, string newDescription,
            string newBody)
        {
            // Arrange
            var user = new UserProfile(Guid.NewGuid().ToString(), "email", "username");

            var article = new Article(slug, null, null, new DateTime(), user);

            Context.Articles.Add(article);
            await Context.SaveChangesAsync();

            var currentUser = Mock.Of<ICurrentUserService>(s => s.UserId == user.Id);

            var command = new UpdateArticleCommand
            {
                Slug = slug,
                Body = newBody,
                Title = newTitle,
                Description = newDescription
            };

            var sut = new UpdateArticleCommand.Handler(Context, currentUser);

            // Act
            await sut.Handle(command, CancellationToken.None);

            var updatedArticle = await Context.Articles.SingleAsync();

            // Assert
            updatedArticle.Title.Should().Be(newTitle);
            updatedArticle.Description.Should().Be(newDescription);
            updatedArticle.Body.Should().Be(newBody);
            updatedArticle.Slug.Should().Be(newTitle);
        }
    }
}