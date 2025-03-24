namespace Shared.Events.Users;

public record UserProfileChangedEvent(
    Guid Id, 
    string Firstname,
    string? Lastname, 
    string? AvatarUrl);