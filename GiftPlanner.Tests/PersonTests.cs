using Xunit;
using GiftPlanner;

public class PersonTests
{
    [Fact]
    public void Person_Constructor_ShouldSetIdAndName()
    {
        var person = new Person(1, "Sheldon Cooper");

        Assert.Equal(1, person.PersonId);
        Assert.Equal("Sheldon Cooper", person.Name);
    }

    [Fact]
    public void Person_Constructor_ShouldInitializeGiftIdeasPurchasesAndImportantDates()
    {
        var person = new Person(1, "Leonard Hofstadter");

        Assert.NotNull(person.GiftIdeas);
        Assert.NotNull(person.Purchases);
        Assert.NotNull(person.ImportantDates);
        Assert.Empty(person.GiftIdeas);
        Assert.Empty(person.Purchases);
        Assert.Empty(person.ImportantDates);
    }

    [Fact]
    public void Person_ToString_ShouldReturnFormattedValue()
    {
        var person = new Person(2, "Penny");

        var result = person.ToString();

        Assert.Equal("2: Penny", result);
    }
}