using System.ComponentModel.DataAnnotations.Schema;

namespace LibApi.Model;

public class BookCopy : DbEntity
{
    [ForeignKey("Book")]
    public int BookId { get; set; }
    public Book Book { get; set; }
    
    
}