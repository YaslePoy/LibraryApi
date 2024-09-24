namespace LibApi.Model;

public class Book : DbEntity
{
    public string Name { get; set; }
    public string Author { get; set; }
    public string Description { get; set; }
    public DateOnly PublicationDate { get; set; }
}