namespace Shared.Events.Moderation;

public record RecipeRejectedEvent(
    Guid RecipeId,
    IEnumerable<Guid> NewIngredientIdsToPublish,
    Guid? NewCuisineId);