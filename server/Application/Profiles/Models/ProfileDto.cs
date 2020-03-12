using Application.Common.Mappings;
using Domain.Entities;

namespace Application.Profiles.Models
{
	public class ProfileDto : IMapFrom<UserProfile>
	{
		public string Email { get; set; }
		public string Username { get; set; }
		public string Bio { get; set; }
		public string Image { get; set; }
	}
}
