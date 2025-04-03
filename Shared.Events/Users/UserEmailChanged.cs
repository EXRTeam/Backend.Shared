namespace Shared.Messages.Users;

public record UserEmailChanged(
    Guid Id,
    string? Email);