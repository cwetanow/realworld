using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Models;
using Microsoft.IdentityModel.Tokens;

namespace Identity
{
	public class TokenService
	{
		private readonly AuthConfiguration configuration;

		public TokenService(AuthConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public string CreateToken(string username, string userId, params KeyValuePair<string, string>[] additionalClaims)
		{
			var claims = new List<Claim> {
				new Claim(JwtRegisteredClaimNames.Sub, username),
				new Claim(JwtRegisteredClaimNames.Jti, userId)
			};

			foreach (var additionalClaim in additionalClaims)
			{
				var claim = new Claim(additionalClaim.Key, additionalClaim.Value);
				claims.Add(claim);
			}

			return GenerateToken(claims);
		}

		public string GenerateToken(IEnumerable<Claim> claims)
		{
			var expires = DateTime.UtcNow.AddHours(configuration.ValidHours);

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.SecretKey));
			var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				configuration.Issuer,
				configuration.Audience,
				claims,
				expires: expires,
				signingCredentials: credentials
			);

			var tokenString = new JwtSecurityTokenHandler()
				.WriteToken(token);

			return tokenString;
		}
	}
}
