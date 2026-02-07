namespace AuthenticationAPI.SystemObjects
{
    public static class Constants
    {
        public static class UserRoles
        {
            public const string SuperRole = "Super";
            public const string AdminRole = "Admin";
            public const string UserRole = "User";
        }

        public static class TokenProviders
        {
            public const string Jwt = "JwtProvider";
        }

        public static class TokenTypes
        {
            public const string Login = "LoginToken";
            public const string Refresh = "RefreshToken";
        }
    }
}
