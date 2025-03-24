namespace Shared.Events.Users;

public record RecipeReviewSubmittedForModerationEvent(
    Guid RecipeId,
    Guid PublisherId,
    string? Comment);