namespace Shared.Messages.Moderation;

public record RecipeApprovedEvent(
    Guid RecipeId,
    IEnumerable<Guid> NewIngredientIds,
    Guid? NewCuisineId);