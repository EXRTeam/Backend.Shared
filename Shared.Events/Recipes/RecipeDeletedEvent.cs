namespace Shared.Messages.Recipes;

public record RecipeDeletedEvent(Guid RecipeId, Guid PublisherId);