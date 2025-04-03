namespace Shared.Messages.Feedbacks;

public record RecipeReviewPublishedEvent(
    Guid RecipeId, 
    Guid PublisherId,
    string? Comment,
    byte Rating);