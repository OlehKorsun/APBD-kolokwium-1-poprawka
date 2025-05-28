namespace Kolokwium1Poprawka.Models_DTOs;

public class CreateClientAndRentDTO
{
    public CreateClientDTO Client { get; set; }
    public int CarId { get; set; }
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
}

public class CreateClientDTO
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
}