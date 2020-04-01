using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Mappings
{
    public class UserFollowerMapping : IEntityTypeConfiguration<UserFollower>
    {
        public void Configure(EntityTypeBuilder<UserFollower> builder)
        {
            builder
                .HasKey(e => new {e.UserId, e.FollowerId});

            builder
                .HasOne(e => e.User)
                .WithMany(u => u.Followers)
                .HasForeignKey(e => e.UserId);

            builder
                .HasOne(e => e.Follower)
                .WithMany(u => u.FollowedUsers)
                .HasForeignKey(e => e.FollowerId);
        }
    }
}