namespace Shared.Events.Users;

public record EmployeeAccountCreatedEvent(Guid EmployeeId, string Role);
