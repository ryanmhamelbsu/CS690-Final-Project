using System.Collections.Generic;
using System.Linq;

namespace GiftPlanner;

public class DataManager
{
    public List<Person> People { get; }

    public DataManager()
    {
        People = new List<Person>();
    }

    public void AddPerson(Person person)
    {
        People.Add(person);
    }

    public Person? FindPersonById(int personId)
    {
        return People.FirstOrDefault(p => p.PersonId == personId);
    }

    public GiftIdea? AddGiftIdeaToPerson(int personId, string description)
    {
        var person = FindPersonById(personId);
        if (person == null)
        {
            return null;
        }

        int newGiftIdeaId = person.GiftIdeas.Count + 1;

        var giftIdea = new GiftIdea(newGiftIdeaId, description);
        person.GiftIdeas.Add(giftIdea);

        return giftIdea;
    }

    public Purchase? AddPurchaseToPerson(int personId, decimal amount)
    {
        var person = FindPersonById(personId);
        if (person == null)
        {
            return null;
        }

        int newPurchaseId = person.Purchases.Count + 1;

        var purchase = new Purchase(newPurchaseId, amount);
        person.Purchases.Add(purchase);

        return purchase;
    }
}