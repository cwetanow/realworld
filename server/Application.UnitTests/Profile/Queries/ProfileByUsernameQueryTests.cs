using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Profiles.Models;
using Application.Profiles.Queries;
using Application.UnitTests.Common;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Profile.Queries
{
	public class ProfileByUsernameQueryTests : BaseTestFixture
	{
		[Theory]
		[InlineData("username")]
		public void TestHandle_ProfileDoesntExist_ShouldThrowEntityNotFoundExceptionWithCorrectMessage(string username)
		{
			// Arrange
			var query = new ProfileByUsernameQuery { Username = username };

			var currentUserServiceMock = new Mock<ICurrentUserService>();

			var sut = new ProfileByUsernameQuery.Handler(context: Context, mapper: Mapper, currentUserServiceMock.Object);

			// Act
			var action = new Func<Task<ProfileDto>>(async () => await sut.Handle(query, CancellationToken.None));

			// Assert
			action.Should().Throw<EntityNotFoundException<UserProfile>>().And.Message.Should().Contain(username);
		}

		[Theory]
		[InlineData("username")]
		public async Task TestHandle_ProfileExists_ShouldReturnCorrectly(string username)
		{
			// Arrange
			var profile = new UserProfile(Guid.NewGuid().ToString(), "email", username);
			Context.UserProfiles.Add(profile);
			await Context.SaveChangesAsync();

			var query = new ProfileByUsernameQuery { Username = username };

			var currentUserServiceMock = new Mock<ICurrentUserService>();

			var sut = new ProfileByUsernameQuery.Handler(context: Context, mapper: Mapper, currentUserServiceMock.Object);

			var expectedResult = Mapper.Map<ProfileDto>(profile);

			// Act
			var actualResult = await sut.Handle(query, CancellationToken.None);

			// Assert
			actualResult.Should().BeEquivalentTo(expectedResult);
		}

		[Theory]
		[InlineData("username")]
		public async Task TestHandle_UserIsAuthenticatedAndIsNotFollowing_ShouldSetDtoFollowingToFalse(string username)
		{
			// Arrange
			var profile = new UserProfile(Guid.NewGuid().ToString(), "email", username);
			Context.UserProfiles.Add(profile);
			await Context.SaveChangesAsync();

			var query = new ProfileByUsernameQuery { Username = username };

			var currentUserServiceMock = new Mock<ICurrentUserService>();

			var sut = new ProfileByUsernameQuery.Handler(context: Context, mapper: Mapper, currentUserServiceMock.Object);

			// Act
			var actualResult = await sut.Handle(query, CancellationToken.None);

			// Assert
			actualResult.Following.Should().BeFalse();
		}

		[Theory]
		[InlineData("username")]
		public async Task TestHandle_UserIsAuthenticatedAndIsFollowing_ShouldSetDtoFollowingToFalse(string username)
		{
			// Arrange
			var currentUserEmail = "currentUserEmail";
			var currentUserProfile = new UserProfile(Guid.NewGuid().ToString(), currentUserEmail, "currentUsername");
			var profile = new UserProfile(Guid.NewGuid().ToString(), "email", username);

			Context.UserProfiles.Add(currentUserProfile);
			Context.UserProfiles.Add(profile);
			await Context.SaveChangesAsync();

			Context.UserFollowers.Add(new UserFollower(profile.Id, currentUserProfile.Id));
			await Context.SaveChangesAsync();

			var query = new ProfileByUsernameQuery { Username = username };

			var currentUserServiceMock = new Mock<ICurrentUserService>();
			currentUserServiceMock.Setup(s => s.IsAuthenticated).Returns(true);
			currentUserServiceMock.Setup(s => s.Email).Returns(currentUserEmail);

			var sut = new ProfileByUsernameQuery.Handler(context: Context, mapper: Mapper, currentUserServiceMock.Object);

			// Act
			var actualResult = await sut.Handle(query, CancellationToken.None);

			// Assert
			actualResult.Following.Should().BeTrue();
		}
	}
}
