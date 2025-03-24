namespace Shared.Events.Users;

public record RecipeReviewDeletedEvent(
    Guid RecipeId,
    Guid PublisherId);