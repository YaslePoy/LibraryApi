namespace LibApi.Model;

public class Tariff : DbEntity
{
    public decimal PaymentPerDay { get; set; }
    public DateTime TariffStart { get; set; }
    public bool IsActive { get; set; }
}