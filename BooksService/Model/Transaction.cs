using System.ComponentModel.DataAnnotations.Schema;

namespace LibApi.Model;

public class Transaction : DbEntity
{
    public int UserId { get; set; }
    public DateTime TransactionTime { get; set; }
    public decimal Movement { get; set; }
}