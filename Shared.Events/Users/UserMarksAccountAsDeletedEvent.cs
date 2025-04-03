namespace Shared.Messages.Users;

public record UserMarksAccountAsDeletedEvent(Guid UserId);
public record UserIsCompletelyDeleted(Guid UserId);
public record UserRestoredAccount(Guid UserId);