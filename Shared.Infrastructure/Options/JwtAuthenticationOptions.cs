namespace Shared.Infrastructure.Options;

public class JwtAuthenticationOptions {
    public byte[] Key { get; set; } = [];
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}

