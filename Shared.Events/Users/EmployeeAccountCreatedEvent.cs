namespace Shared.Messages.Users;

public record EmployeeAccountCreatedEvent(Guid EmployeeId, string? Email, string Role);
