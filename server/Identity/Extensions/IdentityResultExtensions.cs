using System.Linq;
using Application.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Extensions
{
	public static class IdentityResultExtensions
	{
		public static Result ToApplicationResult(this IdentityResult result) => result.Succeeded
			? Result.CreateSuccess()
			: Result.CreateFailure(result.Errors.Select(e => e.Description).ToArray());
	}
}
