using System.Security.Claims;

namespace BooksAdmin
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user) =>
            user.FindFirstValue(ClaimTypes.NameIdentifier)
            //var username = user.FindFirst("preferred_username")?.Value;
            ?? throw new InvalidOperationException("User is not authenticated");
    }
}
