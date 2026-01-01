namespace UniCliqueBackend.Application.Options
{
    public class EmailPolicyOptions
    {
        public bool RestrictToAllowedList { get; set; } = true;
        public List<string> AllowedDomains { get; set; } = new List<string>
        {
            "gmail.com",
            "outlook.com",
            "hotmail.com"
        };
        public List<string> AllowedSuffixes { get; set; } = new List<string>
        {
            ".edu.tr",
            ".edu.com.tr"
        };
    }
}
