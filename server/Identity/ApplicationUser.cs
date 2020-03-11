using Microsoft.AspNetCore.Identity;

namespace Identity
{
	public class ApplicationUser : IdentityUser
	{
		private ApplicationUser() { }

		public ApplicationUser(string username, string email) 
			: base(username)
		{
			this.Email = email;
		}
	}
}
