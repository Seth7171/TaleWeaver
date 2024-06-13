using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Book
{
    public string Name;
    public string Description;

    public Book(string name, string description)
    {
        Name = name;
        Description = description;
    }
}