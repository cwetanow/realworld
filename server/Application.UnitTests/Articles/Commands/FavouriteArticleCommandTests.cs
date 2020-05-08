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

            var command = new FavouriteArticleCommand()
            {
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
            var currentUser = new UserProfile(Guid.NewGuid().ToString(), email, email);
            var article = new Article(slug, null, null, new DateTime(), currentUser);

            Context.Articles.Add(article);
            Context.UserProfiles.Add(currentUser);
            Context.FavouritedArticles.Add(new FavouritedArticle(article, currentUser));
            await Context.SaveChangesAsync();

            var currentUserMock = new Mock<ICurrentUserService>();
            currentUserMock.Setup(s => s.Email).Returns(email);

            var command = new FavouriteArticleCommand()
            {
                Slug = slug
            };

            var sut = new FavouriteArticleCommand.Handler(Context, currentUserMock.Object);

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
            var currentUser = new UserProfile(Guid.NewGuid().ToString(), email, email);
            var article = new Article(slug, null, null, new DateTime(), currentUser);

            Context.Articles.Add(article);
            Context.UserProfiles.Add(currentUser);
            await Context.SaveChangesAsync();

            var currentUserMock = new Mock<ICurrentUserService>();
            currentUserMock.Setup(s => s.Email).Returns(email);

            var command = new FavouriteArticleCommand()
            {
                Slug = slug
            };

            var sut = new FavouriteArticleCommand.Handler(Context, currentUserMock.Object);

            // Act
            await sut.Handle(command, CancellationToken.None);

            var favouritedArticle = await Context.FavouritedArticles.SingleOrDefaultAsync();
            
            // Assert
            favouritedArticle.Should().NotBeNull();
            favouritedArticle.UserId.Should().Be(currentUser.Id);
            favouritedArticle.ArticleId.Should().Be(article.Id);
        }
    }
}