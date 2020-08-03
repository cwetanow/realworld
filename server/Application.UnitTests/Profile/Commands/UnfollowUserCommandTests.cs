using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Profiles.Commands;
using Application.UnitTests.Common;
using Domain.Entities;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Application.UnitTests.Profile.Commands
{
	public class UnfollowUserCommandTests : BaseTestFixture
	{
		[Theory]
		[InlineData("username", "currentUserEmail")]
		public async Task TestHandle_OtherUserDoesntExist_ShouldThrowEntityNotFoundException(string username, string currentUserEmail)
		{
			// Arrange
			var currentUserProfile = new UserProfile(Guid.NewGuid().ToString(), currentUserEmail, "currentUsername");

			Context.UserProfiles.Add(currentUserProfile);
			await Context.SaveChangesAsync();

			var command = new UnfollowUserCommand { Username = username };

			var currentUser = Mock.Of<ICurrentUserService>(s => s.UserId == currentUserProfile.Id);

			var sut = new UnfollowUserCommand.Handler(currentUser, Context);

			// Act
			var act = new Func<Task<Unit>>(async () => await sut.Handle(command, CancellationToken.None));

			// Assert
			act.Should().Throw<EntityNotFoundException<UserProfile>>().And.Message.Should().Contain(username);
		}

		[Theory]
		[InlineData("username", "currentUserEmail")]
		public async Task TestHandle_UserIsNotFollower_ShouldThrowEntityNotFoundException(string username, string currentUserEmail)
		{
			// Arrange
			var otherUserProfile = new UserProfile(Guid.NewGuid().ToString(), "email", username);
			var currentUserProfile = new UserProfile(Guid.NewGuid().ToString(), currentUserEmail, "currentUsername");

			Context.UserProfiles.Add(otherUserProfile);
			Context.UserProfiles.Add(currentUserProfile);
			await Context.SaveChangesAsync();

			var command = new UnfollowUserCommand { Username = username };

			var currentUser = Mock.Of<ICurrentUserService>(s => s.UserId == currentUserProfile.Id);

			var sut = new UnfollowUserCommand.Handler(currentUser, Context);

			// Act
			var act = new Func<Task<Unit>>(async () => await sut.Handle(command, CancellationToken.None));

			// Assert
			act.Should().Throw<EntityNotFoundException<UserFollower>>()
				.And.Message.Should().Contain(otherUserProfile.Id.ToString())
				.And.Contain(currentUserProfile.Id.ToString());
		}

		[Theory]
		[InlineData("username", "currentUserEmail")]
		public async Task TestHandle_ShoudDeleteUserFollower(string username, string currentUserEmail)
		{
			// Arrange
			var otherUserProfile = new UserProfile(Guid.NewGuid().ToString(), "email", username);
			var currentUserProfile = new UserProfile(Guid.NewGuid().ToString(), currentUserEmail, "currentUsername");

			Context.UserProfiles.Add(otherUserProfile);
			Context.UserProfiles.Add(currentUserProfile);
			await Context.SaveChangesAsync();

			Context.UserFollowers.Add(new UserFollower(otherUserProfile.Id, currentUserProfile.Id));
			await Context.SaveChangesAsync();

			var command = new UnfollowUserCommand { Username = username };

			var currentUser = Mock.Of<ICurrentUserService>(s => s.UserId == currentUserProfile.Id);

			var sut = new UnfollowUserCommand.Handler(currentUser, Context);

			// Act
			await sut.Handle(command, CancellationToken.None);

			var following = await Context.UserFollowers
				.SingleOrDefaultAsync();

			// Assert
			following.Should().BeNull();
		}
	}
}
