namespace AdoptMe.Infrastructure
{
    using System.Security.Claims;

    using static Common.GlobalConstants.Roles;

    public static class ClaimsPrincipalExtensions
    {
        public static string GetId(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.NameIdentifier).Value;

        public static string GetEmail(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.Email).Value;

        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.IsInRole(AdminRoleName);

        public static bool IsShelter(this ClaimsPrincipal user)
            => user.IsInRole(ShelterRoleName);
    }
}
