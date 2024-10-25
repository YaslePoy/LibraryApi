using System.ComponentModel.DataAnnotations.Schema;

namespace LibApi.Model;

public class BookCopy : DbEntity
{
    [ForeignKey("Book")] public int BookId { get; set; }
    public Book Book { get; set; }
    public decimal Cost { get; set; }
    public DateTime PurchaseDate { get; set; }
    public int? UserId { get; set; }
    public bool IsLost { get; set; }
}