using System.Collections.Generic;

namespace Scribe.Systems;

public class Book
{
    public string Title { get; set; } = "My Book";
    public List<Chapter> Chapters { get; set; } = new();
}