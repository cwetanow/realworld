using System;
using System.Collections.Generic;

namespace Application.Common.Exceptions
{
	public class BadRequestException : Exception
	{
		public IEnumerable<string> Errors { get; }

		public BadRequestException(IEnumerable<string> errors = null)
		{
			Errors = errors ?? new List<string>();
		}
	}
}
