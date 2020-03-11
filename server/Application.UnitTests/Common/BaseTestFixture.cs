using System;
using Application.Common.Mappings;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.UnitTests.Common
{
	public class BaseTestFixture : IDisposable
	{
		protected BaseTestFixture()
		{
			var configurationProvider = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
			Mapper = configurationProvider.CreateMapper();

			Context = CreateContext();
		}

		protected IMapper Mapper { get; }
		protected ConduitDbContext Context { get; }

		private ConduitDbContext CreateContext()
		{
			var options = new DbContextOptionsBuilder<ConduitDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			var context = new ConduitDbContext(options);
			context.Database.EnsureCreated();

			return context;
		}

		public void Dispose()
		{
			Context.Database.EnsureDeleted();
			Context.Dispose();
		}
	}
}
