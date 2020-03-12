namespace Identity.Configuration
{
	public class AuthConfiguration
	{
		public string SecretKey { get; set; }

		public string Issuer { get; set; }

		public string Audience { get; set; }

		public int ValidHours { get; set; }
	}
}
