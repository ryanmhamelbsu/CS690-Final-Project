using Xunit;
using GiftPlanner;
using System.Linq;

public class DataManagerTests
{
    [Fact]
    public void AddPerson_ShouldIncreasePersonCount()
    {
        var manager = new DataManager();
        int startCount = manager.People.Count;

        var person = new Person(9999, "Test Person");
        manager.AddPerson(person);

        Assert.Equal(startCount + 1, manager.People.Count);
    }

    [Fact]
    public void DeletePerson_ShouldRemovePerson()
    {
        var manager = new DataManager();

        var person = new Person(8888, "Delete Test");
        manager.AddPerson(person);

        manager.DeletePerson(8888);

        var result = manager.FindPersonById(8888);

        Assert.Null(result);
    }

    [Fact]
    public void RemoveGiftIdea_ShouldRemoveIdeaFromPerson()
    {
        var manager = new DataManager();

        var person = new Person(7777, "Gift Test");
        manager.AddPerson(person);

        manager.AddGiftIdeaToPerson(7777, "Book");

        manager.RemoveGiftIdea(7777, 1);

        var result = manager.FindPersonById(7777);

        Assert.NotNull(result);
        Assert.Empty(result!.GiftIdeas);
    }
}