using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Mappings
{
	public class ArticleTagMapping : IEntityTypeConfiguration<ArticleTag>
	{
		public void Configure(EntityTypeBuilder<ArticleTag> builder)
		{
			builder.HasKey(e => new { e.ArticleId, e.TagId });

			builder
				.HasOne(e => e.Article)
				.WithMany()
				.HasForeignKey(e => e.ArticleId);

			builder
				.HasOne(e => e.Tag)
				.WithMany()
				.HasForeignKey(e => e.TagId);
		}
	}
}
