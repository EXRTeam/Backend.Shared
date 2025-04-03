namespace Shared.Messages.Feedbacks;

public record RecipeReviewDeletedEvent(
    Guid RecipeId,
    Guid PublisherId);