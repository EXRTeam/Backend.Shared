namespace Shared.Events.Recipes;

public record RecipeDeletedEvent(Guid RecipeId, Guid PublisherId);