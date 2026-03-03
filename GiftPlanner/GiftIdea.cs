namespace GiftPlanner;

public class GiftIdea
{
    public int GiftIdeaId { get; set; }
    public string Description { get; set; }

    public GiftIdea(int giftIdeaId, string description)
    {
        GiftIdeaId = giftIdeaId;
        Description = description;
    }

    public override string ToString()
    {
        return $"{GiftIdeaId}: {Description}";
    }
}