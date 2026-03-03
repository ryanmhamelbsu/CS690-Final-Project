using System.Collections.Generic;

namespace GiftPlanner;

public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }

    // Gift ideas linked to this person
    public List<GiftIdea> GiftIdeas { get; set; }

    // NEW: Purchases linked to this person
    public List<Purchase> Purchases { get; set; }

    public Person(int personId, string name)
    {
        PersonId = personId;
        Name = name;

        GiftIdeas = new List<GiftIdea>();
        Purchases = new List<Purchase>();
    }

    public override string ToString()
    {
        return $"{PersonId}: {Name}";
    }
}