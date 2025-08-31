using BookRental.Constants;
using System.Security.Claims;

namespace BookRental.Helpers;

public static class TokenHelper
{
    public static string? GetClaimValue(this ClaimsPrincipal user, string claimType)
    {
        return user?.FindFirst(claimType)?.Value;
    }

    public static string GetRoleName(this ClaimsPrincipal user)
    {
        var role = user.GetClaimValue(TokenClaims.RoleName);

        if (string.IsNullOrEmpty(role))
            throw new InvalidOperationException(Messages.RoleMissing);

        return role;
    }

    public static void IsAdmin(this ClaimsPrincipal user)
    {
        var role = user.GetRoleName();

        if (role != UserRoles.Admin)
            throw new UnauthorizedAccessException(Messages.YouDontHavePermission);
    }
    public static string GetUserId(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var userId = user.GetClaimValue(TokenClaims.UserId);

        if (string.IsNullOrEmpty(userId))
            throw new InvalidOperationException(Messages.MissingUserId);

        return userId;
    }

    public static int GetTenantId(this ClaimsPrincipal user)
    {
        ArgumentNullException.ThrowIfNull(user);

        var tenantId = user.GetClaimValue(TokenClaims.TenantId);

        if (string.IsNullOrEmpty(tenantId))
            throw new InvalidOperationException(Messages.MissingTenantId);

        if (!int.TryParse(tenantId, out var tenantIdInt))
            throw new FormatException(Messages.InvalidTenantId);

        return tenantIdInt;
    }
}
