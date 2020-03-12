using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Exceptions;
using Application.Profiles.Models;
using Application.Profiles.Queries;
using Application.UnitTests.Common;
using Domain.Entities;
using FluentAssertions;
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

			var sut = new ProfileByUsernameQuery.Handler(context: Context, mapper: Mapper);

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

			var sut = new ProfileByUsernameQuery.Handler(context: Context, mapper: Mapper);

			var expectedResult = Mapper.Map<ProfileDto>(profile);

			// Act
			var actualResult = await sut.Handle(query, CancellationToken.None);

			// Assert
			actualResult.Should().BeEquivalentTo(expectedResult);
		}
	}
}
