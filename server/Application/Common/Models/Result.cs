using System.Collections.Generic;

namespace Application.Common.Models
{
	public class Result
	{
		private Result(bool success, IEnumerable<string> errors = null)
		{
			Success = success;
			Errors = errors ?? new List<string>();
		}

		public bool Success { get; }

		public IEnumerable<string> Errors { get; }

		public static Result CreateSuccess() => new Result(true);

		public static Result CreateFailure(IEnumerable<string> errors = null) => new Result(false, errors);
	}
}
