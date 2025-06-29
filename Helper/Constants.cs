namespace CesiZen_API.Helper
{
    public static class Constants
    {
        public const Boolean STATUS_ACTIVE = true;
        public const Boolean STATUS_INACTIVE = false;

        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_USER = "User";

        public static readonly Dictionary<int, string> ACTIVITY_TYPES = new()
        {
            { 1, "Séance guidée" },
            { 2, "Visualisation" },
            { 3, "Scénario immersif" },
            { 4, "Stretching audio" },
            { 5, "Mini-méditation" },
            { 6, "Histoire apaisante" },
            { 7, "Rituels du matin/soir" },
         };

        public static readonly Dictionary<int, string> FAKE_CATEGORY = new()
        {
           { 1, "Relaxation & Détente" },
           { 2, "Focus & Concentration" },
           { 3, "Sommeil & Endormissement" },
           { 4, "Motivation" },
           { 5, "Gestion du stress" },
           { 6, "Bien-être corporel" },
           { 7, "Éveil matinal" },
           { 8, "Énergie & Vitalité" },
           { 9, "Déconnexion" }
        };

        public static readonly List<string> FAKE_MEDIA = new()
        {
            "test.mp3",
            "test.mp4"
        };

        public static readonly List<string> FAKE_IMAGE = new()
        {
             "test.png",
            "test2.jpg"
        };



    }
}
