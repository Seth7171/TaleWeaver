using System;
using System.Collections.Generic;

[Serializable]
public class Book
{
    public string Name;
    public string Description;
    public List<Page> Pages;

    public Book(string name, string description)
    {
        Name = name;
        Description = description;
        Pages = new List<Page>();
    }
}

[Serializable]
public class Page
{
    public string Narrative;
    public string ImageUrl;

    public Page(string narrative, string imageUrl)
    {
        Narrative = narrative;
        ImageUrl = imageUrl;
    }
}
