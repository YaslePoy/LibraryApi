using LibApi.Model;

namespace LibApi.Requests;

public class RentHistoryForUser
{
    public int Id { get; set; }
    public int BookCopyId { get; set; }
    public DateTime RentStart { get; set; }
    public DateTime RentEnd { get; set; }
}