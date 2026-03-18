namespace GiftPlanner;

// Handles gift ideas releated to a person
public class GiftIdea
{
    // Unique identifier for the gift idea
    public int GiftIdeaId { get; set; }

    // Description of the gift idea
    public string Description { get; set; }

    // Indicates whether the gift has been bought
    public bool Bought { get; set; }

    // Constructor initializes the gift idea
    public GiftIdea(int giftIdeaId, string description, bool bought = false)
    {
        GiftIdeaId = giftIdeaId;
        Description = description;
        Bought = bought;
    }

    // Returns a formatted string representation of the gift idea
    public override string ToString()
    {
        return $"{GiftIdeaId}: {Description} ({(Bought ? "Yes" : "No")})";
    }
}