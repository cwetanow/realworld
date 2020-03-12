using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Mappings
{
	public class UserProfileMapping : IEntityTypeConfiguration<UserProfile>
	{
		public void Configure(EntityTypeBuilder<UserProfile> builder)
		{
			builder.HasIndex(p => p.Email).IsUnique();
			builder.HasIndex(p => p.Username).IsUnique();

			builder.Property(e => e.Email).IsRequired();
			builder.Property(e => e.Username).IsRequired();
			builder.Property(e => e.UserId).IsRequired();
		}
	}
}
