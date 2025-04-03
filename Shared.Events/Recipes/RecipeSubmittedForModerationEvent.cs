namespace Shared.Messages.Recipes;

public record RecipeSubmittedForModerationEvent(Guid RecipeId, Guid PublisherId);