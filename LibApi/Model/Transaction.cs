using System.ComponentModel.DataAnnotations.Schema;

namespace LibApi.Model;

public class Transaction : DbEntity
{
    [ForeignKey("User")] public int UserId { get; set; }
    public User User { get; set; }
    public DateTime TransactionTime { get; set; }
    public decimal Movement { get; set; }
}