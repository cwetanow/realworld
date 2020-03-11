using Domain.Common;

namespace Domain.Entities
{
	public class UserProfile : Entity
	{
		public UserProfile(string userId, string email, string username)
		{
			UserId = userId;
			Email = email;
			Username = username;
		}

		public string UserId { get; set; }

		public string Email { get; set; }
		public string Username { get; set; }

		public string Bio { get; }
		public string Image { get; }
	}
}
