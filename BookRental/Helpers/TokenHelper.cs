using BookRental.Constants;
using System.Security.Claims;

namespace BookRental.Helpers;

public static class TokenHelper
{
   public static string? GetClaimValue(this ClaimsPrincipal user, string claimType)
    {
        return user?.FindFirst(claimType)?.Value;
    }

    public static string? GetRoleName(this ClaimsPrincipal user)
    {
        return user.GetClaimValue(TokenClaims.RoleName);
    }

    public static string? GetUserId(this ClaimsPrincipal user)
    {
        return user.GetClaimValue(TokenClaims.UserId);
    }

    public static string? GetTenantId(this ClaimsPrincipal user)
    {
        return user.GetClaimValue(TokenClaims.TenantId);
    }
}
