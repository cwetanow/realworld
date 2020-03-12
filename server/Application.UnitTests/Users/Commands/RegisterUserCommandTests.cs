using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.UnitTests.Common;
using Application.Users.Commands.RegisterUser;
using Application.Users.Models;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Application.UnitTests.Users.Commands
{
	public class RegisterUserCommandTests : BaseTestFixture
	{
		[Theory]
		[InlineData("email", "username", "password")]
		public async Task TestHandle_ShouldCallUserServiceCreateUserCorrectly(
			string email, string username, string password)
		{
			// Arrange
			var command = new RegisterUserCommand {
				Email = email,
				Username = username,
				Password = password
			};

			var mockedUserService = new Mock<IUserService>();
			mockedUserService.Setup(s => s.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync((Result.CreateSuccess(), null));

			var sut = new RegisterUserCommand.Handler(mockedUserService.Object, Mapper, Context);

			// Act
			await sut.Handle(command, CancellationToken.None);

			// Assert
			mockedUserService.Verify(s => s.CreateUser(username, email, password), Times.Once);
		}

		[Theory]
		[InlineData("email", "username", "password")]
		public void TestHandle_UserServiceCreateUserIsNotSuccess_ShouldThrowBadRequestExceptionWithCorrectErrors(
			string email, string username, string password)
		{
			// Arrange
			var errors = new List<string> { "some error" };

			var result = Result.CreateFailure(errors.ToArray());

			var command = new RegisterUserCommand {
				Email = email,
				Username = username,
				Password = password
			};

			var mockedUserService = new Mock<IUserService>();
			mockedUserService.Setup(s => s.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync((result, null));

			var sut = new RegisterUserCommand.Handler(mockedUserService.Object, Mapper, Context);

			// Act
			var action = new Func<Task<UserWithProfileDto>>(async () => await sut.Handle(command, CancellationToken.None));

			// Assert
			action.Should().Throw<BadRequestException>().And.Errors.Should().BeEquivalentTo(errors);
		}

		[Theory]
		[InlineData("email", "username", "password", "userId")]
		public async Task TestHandle_UserServiceCreateUserIsSuccess_ShouldSaveUserProfileToDatabase(
			string email, string username, string password, string userId)
		{
			// Arrange
			var command = new RegisterUserCommand {
				Email = email,
				Username = username,
				Password = password
			};

			var mockedUserService = new Mock<IUserService>();
			mockedUserService.Setup(s => s.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync((Result.CreateSuccess(), userId));

			var sut = new RegisterUserCommand.Handler(mockedUserService.Object, Mapper, Context);

			// Act
			await sut.Handle(command, CancellationToken.None);

			var profile = await Context.UserProfiles.SingleOrDefaultAsync();

			// Assert
			profile.Should().NotBeNull();
			profile.Username.Should().Be(username);
			profile.Email.Should().Be(email);
			profile.UserId.Should().Be(userId);
		}

		[Theory]
		[InlineData("email", "username", "password", "userId")]
		public async Task TestHandle_UserServiceCreateUserIsSuccess_ShouldReturnCorrectly(string email, string username, string password, string userId)
		{
			// Arrange
			var command = new RegisterUserCommand {
				Email = email,
				Username = username,
				Password = password
			};

			var mockedUserService = new Mock<IUserService>();
			mockedUserService.Setup(s => s.CreateUser(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync((Result.CreateSuccess(), userId));

			var sut = new RegisterUserCommand.Handler(mockedUserService.Object, Mapper, Context);

			// Act
			var result = await sut.Handle(command, CancellationToken.None);

			// Assert
			result.Username.Should().Be(username);
			result.Email.Should().Be(email);
		}
	}
}
