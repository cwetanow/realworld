using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Tags.Models;
using Application.Tags.Queries;
using Application.UnitTests.Common;
using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Application.UnitTests.Tags.Queries
{
    public class TagListQueryTests : BaseTestFixture
    {
        [Fact]
        public async Task TestHandle_ShouldReturnCorrectly()
        {
            // Arrange
            var tags = new List<Tag>
            {
                new Tag(Guid.NewGuid().ToString()),
                new Tag(Guid.NewGuid().ToString()),
                new Tag(Guid.NewGuid().ToString())
            };

            Context.Tags.AddRange(tags);
            await Context.SaveChangesAsync();

            var expected = new TagListDto
            {
                Tags = tags.Select(t => t.Value)
            };

            var query = new TagListQuery();

            var sut = new TagListQuery.Handler(Context, Mapper);

            // Act
            var result = await sut.Handle(query, CancellationToken.None);

            // Assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}