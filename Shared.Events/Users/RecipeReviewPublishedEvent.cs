namespace Shared.Events.Users;

public record RecipeReviewPublishedEvent(
    Guid RecipeId, 
    Guid PublisherId,
    string? Comment,
    byte Rating);