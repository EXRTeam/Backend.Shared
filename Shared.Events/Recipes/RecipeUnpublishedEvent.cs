namespace Shared.Messages.Recipes;

public record RecipeUnpublishedEvent(Guid RecipeId, Guid PublisherId);