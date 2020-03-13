using Domain.Common;

namespace Domain.Entities
{
	public class Tag : Entity
	{
		public Tag(string value)
		{
			Value = value;
		}

		public string Value { get; set; }
	}
}
