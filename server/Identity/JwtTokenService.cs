using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Identity
{
	public class JwtTokenService
	{
		private readonly AuthConfiguration configuration;

		public JwtTokenService(AuthConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public string CreateToken(string email, params KeyValuePair<string, string>[] additionalClaims)
		{
			var claims = new List<Claim> {
				new Claim(JwtRegisteredClaimNames.Sub, email),
				new Claim(JwtRegisteredClaimNames.Jti, email)
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
