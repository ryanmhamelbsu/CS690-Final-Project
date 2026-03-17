using Xunit;
using GiftPlanner;
using System;

public class ImportantDateTests
{
    [Fact]
    public void ImportantDate_Constructor_ShouldSetTypeAndDate()
    //Checks that the constructor correctly stores the event type and date
    {
        var date = new DateTime(1998, 2, 22);
        var importantDate = new ImportantDate("Birthday", date);

        Assert.Equal("Birthday", importantDate.Type);
        Assert.Equal(date, importantDate.Date);
    }

    [Fact]
    public void ImportantDate_ShouldStoreCorrectDate()
    //Checks that the date values are stored correctly
    {
        var importantDate = new ImportantDate("Anniversary", new DateTime(2020, 6, 10));

        Assert.Equal(6, importantDate.Date.Month);
        Assert.Equal(10, importantDate.Date.Day);
    }

    [Fact]
    public void ImportantDate_ToString_ShouldReturnReadableFormat()
    //Checks that the string representation includes the event type
    {
        var importantDate = new ImportantDate("Birthday", new DateTime(1998, 2, 22));

        var result = importantDate.ToString();

        Assert.Contains("Birthday", result);
    }
}