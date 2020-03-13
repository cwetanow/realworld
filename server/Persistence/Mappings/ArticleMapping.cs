using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Mappings
{
	public class ArticleMapping : IEntityTypeConfiguration<Article>
	{
		public void Configure(EntityTypeBuilder<Article> builder)
		{
			builder.HasIndex(e => e.Slug);

			builder
				.HasOne(e => e.Author)
				.WithMany()
				.HasForeignKey(e => e.AuthorId);
		}
	}
}
