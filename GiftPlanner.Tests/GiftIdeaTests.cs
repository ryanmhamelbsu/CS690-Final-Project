using Xunit;
using GiftPlanner;

public class GiftIdeaTests
{
    [Fact]
    public void GiftIdea_Constructor_ShouldSetIdDescriptionAndDefaultBought()
    //Checks that a gift idea stores ID, description, and sets default bought status (false)
    {
        var giftIdea = new GiftIdea(1, "LEGO Set");

        Assert.Equal(1, giftIdea.GiftIdeaId);
        Assert.Equal("LEGO Set", giftIdea.Description);
        Assert.False(giftIdea.Bought);
    }

    [Fact]
    public void GiftIdea_Constructor_ShouldAllowBoughtToBeTrue()
    //Checks that a gift idea can be created with Bought = true
    {
        var giftIdea = new GiftIdea(2, "Physics Book", true);

        Assert.True(giftIdea.Bought);
    }

    [Fact]
    public void GiftIdea_ToString_ShouldIncludeBoughtStatus()
    //Checks that the string representation includes the bought status.
    {
        var giftIdea = new GiftIdea(3, "Headphones", true);

        var result = giftIdea.ToString();

        Assert.Equal("3: Headphones (Yes)", result);
    }
}