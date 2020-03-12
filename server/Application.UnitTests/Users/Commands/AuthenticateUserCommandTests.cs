using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.UnitTests.Common;
using Application.Users.Commands.AuthenticateUser;
using Application.Users.Models;
using Domain.Entities;
using FluentAssertions;
using Moq;
using Xunit;

namespace Application.UnitTests.Users.Commands
{
	public class AuthenticateUserCommandTests : BaseTestFixture
	{
		[Theory]
		[InlineData("email", "password")]
		public async Task TestHandle_ShouldCallUserServiceAuthenticateCorrectly(string email, string password)
		{
			// Arrange
			var userProfile = new UserProfile(Guid.NewGuid().ToString(), email, "username");
			Context.UserProfiles.Add(userProfile);
			await Context.SaveChangesAsync();

			var command = new AuthenticateUserCommand {
				Email = email,
				Password = password
			};

			var mockedUserService = new Mock<IUserService>();
			mockedUserService.Setup(s => s.Authenticate(email, password))
				.ReturnsAsync(Result.CreateSuccess());

			var sut = new AuthenticateUserCommand.Handler(mockedUserService.Object, Mapper, Context);

			// Act
			await sut.Handle(command, CancellationToken.None);

			// Assert
			mockedUserService.Verify(s => s.Authenticate(email, password), Times.Once);
		}

		[Theory]
		[InlineData("email", "password")]
		public void TestHandle_UserServiceAuthenticateIsNotSuccess_ShouldThrowBadRequestException(string email, string password)
		{
			// Arrange
			var command = new AuthenticateUserCommand {
				Email = email,
				Password = password
			};

			var mockedUserService = new Mock<IUserService>();
			mockedUserService.Setup(s => s.Authenticate(email, password))
				.ReturnsAsync(Result.CreateFailure());

			var sut = new AuthenticateUserCommand.Handler(mockedUserService.Object, Mapper, Context);

			// Act
			var action = new Func<Task<UserWithProfileDto>>(async () => await sut.Handle(command, CancellationToken.None));

			// Assert
			action.Should().Throw<BadRequestException>();
		}

		[Theory]
		[InlineData("email", "password")]
		public void TestHandle_EntityDoesNotExist_ShouldThrowEntityNotFoundExceptionWithCorrectErrors(string email, string password)
		{
			// Arrange
			var command = new AuthenticateUserCommand {
				Email = email,
				Password = password
			};

			var mockedUserService = new Mock<IUserService>();
			mockedUserService.Setup(s => s.Authenticate(email, password))
				.ReturnsAsync(Result.CreateSuccess);

			var sut = new AuthenticateUserCommand.Handler(mockedUserService.Object, Mapper, Context);

			// Act
			var action = new Func<Task<UserWithProfileDto>>(async () => await sut.Handle(command, CancellationToken.None));

			// Assert
			action.Should().Throw<EntityNotFoundException<UserProfile>>().And.Message.Should().Contain(email);
		}

		[Theory]
		[InlineData("email", "password")]
		public async Task TestHandle_EntityExists_ShouldReturnCorrectly(string email, string password)
		{
			// Arrange
			var userProfile = new UserProfile(Guid.NewGuid().ToString(), email, "username");
			Context.UserProfiles.Add(userProfile);
			await Context.SaveChangesAsync();

			var command = new AuthenticateUserCommand {
				Email = email,
				Password = password
			};

			var mockedUserService = new Mock<IUserService>();
			mockedUserService.Setup(s => s.Authenticate(email, password))
				.ReturnsAsync(Result.CreateSuccess);

			var sut = new AuthenticateUserCommand.Handler(mockedUserService.Object, Mapper, Context);

			var expectedResult = Mapper.Map<UserWithProfileDto>(userProfile);

			// Act
			var actualResult = await sut.Handle(command, CancellationToken.None);

			// Assert
			actualResult.Should().BeEquivalentTo(expectedResult);
		}
	}
}
