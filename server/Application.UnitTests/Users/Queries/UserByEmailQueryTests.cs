using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.UnitTests.Common;
using Application.Users.Models;
using Application.Users.Queries;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Application.UnitTests.Users.Queries
{
	public class UserByEmailQueryTests : BaseTestFixture
	{
		[Theory]
		[InlineData("email")]
		public void TestHandle_EntityDoesNotExist_ShouldThrowEntityNotFoundExceptionWithCorrectErrors(string email)
		{
			// Arrange
			var command = new UserByEmailQuery {
				Email = email
			};

			var sut = new UserByEmailQuery.Handler(Context, Mapper);

			// Act
			var action = new Func<Task<UserWithProfileDto>>(async () => await sut.Handle(command, CancellationToken.None));

			// Assert
			action.Should().Throw<EntityNotFoundException<UserProfile>>().And.Message.Should().Contain(email);
		}

		[Theory]
		[InlineData("email")]
		public async Task TestHandle_EntityExists_ShouldReturnCorrectly(string email)
		{
			// Arrange
			var userProfile = new UserProfile(Guid.NewGuid().ToString(), email, "username");
			Context.UserProfiles.Add(userProfile);
			await Context.SaveChangesAsync();

			var command = new UserByEmailQuery {
				Email = email
			};

			var sut = new UserByEmailQuery.Handler(Context, Mapper);

			var expectedResult = Mapper.Map<UserWithProfileDto>(userProfile);

			// Act
			var actualResult = await sut.Handle(command, CancellationToken.None);

			// Assert
			actualResult.Should().BeEquivalentTo(expectedResult);
		}
	}
}
