namespace CesiZen_API.Helper
{
    public static class Constants
    {
        public const Boolean STATUS_ACTIVE = true;
        public const Boolean  STATUS_INACTIVE = false;

        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_USER = "User";

        public static readonly Dictionary<int, string> ACTIVITY_TYPES = new()
           {
               { 1, "Yoga" },
               { 2, "ASMR" },
               { 3, "Sonore" },
               { 4, "Étirement" },
               {5, "Méditation" }
           };
    }
}
