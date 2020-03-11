using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Users.Models
{
	public class UserWithProfileDto : IMapFrom<UserProfile>
	{
		public string Email { get; set; }
		public string Username { get; set; }
		public string Token { get; set; }
		public string Bio { get; set; }
		public string Image { get; set; }
	}
}
