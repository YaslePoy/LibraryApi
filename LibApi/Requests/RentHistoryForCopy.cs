using LibApi.Model;

namespace LibApi.Requests;

public class RentHistoryForCopy
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime RentStart { get; set; }
    public DateTime RentEnd { get; set; }
    public bool IsReturned { get; set; }
}