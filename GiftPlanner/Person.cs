namespace GiftPlanner;

public class Person
{
    public int PersonId { get; set; }
    public string Name { get; set; }

    public Person(int personId, string name)
    {
        PersonId = personId;
        Name = name;
    }

    public override string ToString()
    {
        return $"{PersonId}: {Name}";
    }
}