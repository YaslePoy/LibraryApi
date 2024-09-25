using System.ComponentModel.DataAnnotations.Schema;

namespace LibApi.Model;

public class BookRental : DbEntity
{
    [ForeignKey("User")]
    public int? UserId { get; set; }
    public User? User { get; set; }
    
    [ForeignKey("BookCopy")]
    public int BookCopyId { get; set; }
    public BookCopy BookCopy { get; set; }
    public DateTime RentStart { get; set; }
    public DateTime RentEnd { get; set; }
    public bool IsReturned { get; set; }
}