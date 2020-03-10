using Domain.Common;

namespace Domain.Entities
{
	public class UserProfile : Entity
	{
		public UserProfile(string userId)
		{
			UserId = userId;
		}

		public string UserId { get; set; }

		public string Bio { get; }
		public string Image { get; }
	}
}
