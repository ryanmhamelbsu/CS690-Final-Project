using Xunit;
using GiftPlanner;

public class GiftIdeaTests
{
    [Fact]
    public void GiftIdea_Constructor_ShouldSetIdDescriptionAndDefaultBought()
    {
        var giftIdea = new GiftIdea(1, "LEGO Set");

        Assert.Equal(1, giftIdea.GiftIdeaId);
        Assert.Equal("LEGO Set", giftIdea.Description);
        Assert.False(giftIdea.Bought);
    }

    [Fact]
    public void GiftIdea_Constructor_ShouldAllowBoughtToBeTrue()
    {
        var giftIdea = new GiftIdea(2, "Physics Book", true);

        Assert.True(giftIdea.Bought);
    }

    [Fact]
    public void GiftIdea_ToString_ShouldIncludeBoughtStatus()
    {
        var giftIdea = new GiftIdea(3, "Headphones", true);

        var result = giftIdea.ToString();

        Assert.Equal("3: Headphones (Yes)", result);
    }
}