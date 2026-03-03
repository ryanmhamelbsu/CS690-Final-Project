using System;
using System.Collections.Generic;

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
}