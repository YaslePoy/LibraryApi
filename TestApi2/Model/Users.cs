using System.ComponentModel.DataAnnotations;

namespace TestApi2.Model
{
    public class Users
    {
        [Key]
        public int id_User { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
