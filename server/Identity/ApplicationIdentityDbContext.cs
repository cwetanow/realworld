using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity
{
	internal class ApplicationIdentityDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options)
			: base(options)
		{

		}
	}
}
