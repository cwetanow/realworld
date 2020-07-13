using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace API.Infrastructure.Services
{
	public class HttpContextCurrentUserService : ICurrentUserService
	{
		public HttpContextCurrentUserService(IHttpContextAccessor httpContextAccessor)
		{
			this.IsAuthenticated = httpContextAccessor.HttpContext.User?.Identity != null;
			this.Email = httpContextAccessor.HttpContext.User?.Claims?.SingleOrDefault(c => c.Type.Equals(JwtRegisteredClaimNames.Sub))?.Value;

			if (int.TryParse(httpContextAccessor.HttpContext.User?.Claims?.SingleOrDefault(c => c.Type.Equals(JwtRegisteredClaimNames.Jti))?.Value, out var userId))
			{
				this.UserId = userId;
			}
		}

		public string Email { get; }
		public int UserId { get; }

		public bool IsAuthenticated { get; }
	}
}
