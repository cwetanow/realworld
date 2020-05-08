using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Mappings
{
    public class FavouritedArticleMapping : IEntityTypeConfiguration<FavouritedArticle>
    {
        public void Configure(EntityTypeBuilder<FavouritedArticle> builder)
        {
            builder
                .HasKey(e => new {e.UserId, e.ArticleId});

            builder
                .HasOne(e => e.User)
                .WithMany(u => u.FavouriteArticles)
                .HasForeignKey(e => e.UserId);

            builder
                .HasOne(e => e.Article)
                .WithMany()
                .HasForeignKey(e => e.ArticleId);
        }
    }
}