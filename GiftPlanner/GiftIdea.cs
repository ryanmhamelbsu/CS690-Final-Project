namespace GiftPlanner;

// Represents a gift idea linked to a person
public class GiftIdea
{
    // Unique identifier for the gift idea
    public int GiftIdeaId { get; set; }

    // Description of the gift idea
    public string Description { get; set; }

    // Constructor initializes the gift idea with an ID and description
    public GiftIdea(int giftIdeaId, string description)
    {
        GiftIdeaId = giftIdeaId;
        Description = description;
    }

    // Returns a formatted string representation of the gift idea
    public override string ToString()
    {
        return $"{GiftIdeaId}: {Description}";
    }
}