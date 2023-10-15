namespace SocialNetwork;

public class Configuration
{
    public static string JwtKey { get; set; } = "Zm4VkYCY3ZDg4kMTk3YjkyO8dkNDKyYjcwOGSL";

    public static SmtpConfiguration Smtp = new();

    public class SmtpConfiguration
    {
        public string Host { get; set; }

        public int Port { get; set; } = 25;

        public string Username { get; set; }

        public string Password { get; set; }
    }
}