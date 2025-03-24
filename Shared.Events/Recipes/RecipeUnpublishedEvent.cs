namespace Shared.Events.Recipes;

public record RecipeUnpublishedEvent(Guid RecipeId, Guid PublisherId);