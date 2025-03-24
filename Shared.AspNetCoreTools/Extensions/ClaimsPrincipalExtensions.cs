using System.Security.Claims;

namespace Shared.AspNetCoreTools.Extensions;

public static class ClaimsPrincipalExtensions {
    public static Guid? GetParsedUserId(this ClaimsPrincipal claims) {
        var userId = claims.GetUserId();
        if (userId == null) return null;

        return Guid.Parse(userId);
    }

    public static Guid GetRequiredParsedUserId(this ClaimsPrincipal claims)
        => claims.GetParsedUserId()!.Value;

    public static string GetRequiredUserId(this ClaimsPrincipal claims)
        => claims.GetUserId()!;

    public static string? GetUserId(this ClaimsPrincipal claims)
        => claims.GetClaimValue(ClaimTypes.NameIdentifier);

    public static string? GetRole(this ClaimsPrincipal claims)
        => claims.GetClaimValue(ClaimTypes.Role);

    public static string? GetUsername(this ClaimsPrincipal claims)
        => claims.GetClaimValue(ClaimTypes.Name);

    private static string? GetClaimValue(
        this ClaimsPrincipal claims,
        string claimType)
        => claims.FindFirst(claimType)?.Value;
}
