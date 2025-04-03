namespace Shared.Messages.Feedbacks;

public record RecipeReviewSubmittedForModerationEvent(
    Guid RecipeId,
    Guid PublisherId,
    string? Comment);