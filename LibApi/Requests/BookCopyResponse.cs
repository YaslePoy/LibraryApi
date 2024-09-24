namespace LibApi.Requests;

public class BookCopyResponse
{
    public int BookId { get; set; }
    public decimal Cost { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public int? UserId { get; set; }

}