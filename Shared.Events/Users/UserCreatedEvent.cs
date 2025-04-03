namespace Shared.Messages.Users;

public record UserCreatedEvent(Guid UserId, string? Email);
