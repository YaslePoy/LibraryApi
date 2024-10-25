using System.ComponentModel.DataAnnotations;

namespace LibApi.Model;

public abstract class DbEntity
{
    [Key]
    public int Id { get; set; }
}