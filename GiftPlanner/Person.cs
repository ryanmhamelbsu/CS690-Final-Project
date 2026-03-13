using System.Collections.Generic;

namespace GiftPlanner;

// Represents a person in the gift planning system
public class Person
{
    // Unique identifier for the person
    public int PersonId { get; set; }

    // Name of the person
    public string Name { get; set; }

    // List of gift ideas associated with this person
    public List<GiftIdea> GiftIdeas { get; set; }

    // List of purchases recorded for this person
    public List<Purchase> Purchases { get; set; }

    // List of important dates associated with this person
    public List<ImportantDate> ImportantDates { get; set; }

    // Constructor initializes the person and their related lists
    public Person(int personId, string name)
    {
        PersonId = personId;
        Name = name;

        GiftIdeas = new List<GiftIdea>();
        Purchases = new List<Purchase>();
        ImportantDates = new List<ImportantDate>();
    }

    // Returns a formatted string representation of the person
    public override string ToString()
    {
        return $"{PersonId}: {Name}";
    }
}