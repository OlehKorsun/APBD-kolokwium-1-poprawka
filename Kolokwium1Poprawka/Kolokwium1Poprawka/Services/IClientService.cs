using Kolokwium1Poprawka.Models_DTOs;

namespace Kolokwium1Poprawka.Services;

public interface IClientService
{
    Task<ClientDTO> GetClientAsync(int clientId);
    Task PostClientAndRentAsync(CreateClientAndRentDTO client);
}