using System.ComponentModel.DataAnnotations;

namespace TestApi2.Model;

public abstract class DbEntity
{
    [Key]
    public int Id { get; set; }
}