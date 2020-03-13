using System;
using System.Linq;
using System.Reflection;
using Domain.Common;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
	public class ConduitDbContext : DbContext
	{
		public ConduitDbContext(DbContextOptions<ConduitDbContext> options)
			: base(options)
		{
		}

		public DbSet<UserProfile> UserProfiles { get; set; }
		public DbSet<UserFollower> UserFollowers { get; set; }
		public DbSet<Article> Articles { get; set; }
		public DbSet<ArticleTag> ArticleTags { get; set; }
		public DbSet<Tag> Tags { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				// Configure primary key
				if (typeof(Entity).IsAssignableFrom(entityType.ClrType))
				{
					modelBuilder.Entity(entityType.ClrType)
						.HasKey(nameof(Entity.Id));
				}

				// Disable cascade delete
				modelBuilder.Model.GetEntityTypes()
					.SelectMany(t => t.GetForeignKeys())
					.Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade)
					.ToList()
					.ForEach(e => e.DeleteBehavior = DeleteBehavior.Restrict);

				// Configure all EntityTypeConfiguration
				var typesToRegisterConfigurations = Assembly.GetExecutingAssembly().GetTypes()
					.Where(t =>
						t.GetInterfaces().Any(gi => gi.IsGenericType &&
													gi.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
					.Select(Activator.CreateInstance)
					.ToList();

				foreach (var configurationInstance in typesToRegisterConfigurations)
				{
					modelBuilder.ApplyConfiguration((dynamic)configurationInstance);
				}
			}

			base.OnModelCreating(modelBuilder);
		}
	}
}
