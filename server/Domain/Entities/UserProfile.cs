using System.Collections.Generic;
using System.Linq;
using Domain.Common;

namespace Domain.Entities
{
	public class UserProfile : Entity
	{
		private UserProfile()
		{
		}

		public UserProfile(string userId, string email, string username)
		{
			UserId = userId;
			Email = email;
			Username = username;
		}

		public string UserId { get; }

		public string Email { get; }
		public string Username { get; }

		public void UpdateBio(string bio) => Bio = bio;
		public void UpdateImage(string newImage) => Image = newImage;

		public string Bio { get; private set; }
		public string Image { get; private set; }

		public ICollection<UserFollower> Followers { get; } = new List<UserFollower>();
		public ICollection<UserFollower> FollowedUsers { get; } = new List<UserFollower>();

		public ICollection<FavouritedArticle> FavouriteArticles { get; } = new List<FavouritedArticle>();

		public bool HasFavouritedArticle(Article article) => FavouriteArticles.Any(fa => fa.ArticleId == article.Id);

		public void FavouriteArticle(Article article) => FavouriteArticles.Add(new FavouritedArticle(article, this));

		public bool HasFollowed(UserProfile otherUser) => FollowedUsers.Any(f => f.UserId == otherUser.Id);

		public void Follow(UserProfile otherUser) => FollowedUsers.Add(new UserFollower(otherUser, this));

		public void Unfollow(UserProfile otherUser)
		{
			var followedUser = FollowedUsers.Single(f => f.UserId == otherUser.Id);
			FollowedUsers.Remove(followedUser);
		}
	}
}