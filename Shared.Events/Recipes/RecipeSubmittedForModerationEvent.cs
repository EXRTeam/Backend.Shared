namespace Shared.Events.Recipes;

public record RecipeSubmittedForModerationEvent(Guid RecipeId, Guid PublisherId);