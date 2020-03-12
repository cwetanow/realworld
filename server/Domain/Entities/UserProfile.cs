using Domain.Common;

namespace Domain.Entities
{
	public class UserProfile : Entity
	{
		private UserProfile() { }

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
	}
}
