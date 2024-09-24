namespace LibApi.Requests;

public class GetUserResponse
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public DateTime BirthDate { get; set; }
    public string About { get; set; }
    public string Phone { get; set; }
}