using System;
using System.Collections.Generic;

namespace Application.Common.Exceptions
{
	public class BadRequestException : Exception
	{
		public IEnumerable<string> Errors { get; }

		public BadRequestException(params string[] errors)
		{
			Errors = errors;
		}
	}
}
