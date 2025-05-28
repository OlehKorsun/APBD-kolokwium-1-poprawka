using Kolokwium1Poprawka.Exceptions;
using Kolokwium1Poprawka.Models_DTOs;
using Microsoft.Data.SqlClient;

namespace Kolokwium1Poprawka.Services;

public class ClientService : IClientService
{
    
    private readonly string _connectionString;

    public ClientService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
    }
    
    public async Task<ClientDTO> GetClientAsync(int clientId)
    {
        ClientDTO clientDto = null;

        var query = @"SELECT CLIENTS.ID, FIRSTNAME, LASTNAME, ADDRESS, VIN, COLORS.NAME, MODELS.NAME, DATEFROM, DATETO, totalprice FROM CLIENTS 
                        JOIN CAR_RENTALS ON CLIENTS.ID = CAR_RENTALS.CLIENTID
                        JOIN CARS ON CARS.ID = CAR_RENTALS.CARID
                        JOIN MODELS ON MODELS.ID = CARS.MODELID
                        JOIN COLORS ON COLORS.ID = CARS.COLORID
                        WHERE CLIENTS.ID = @ClientId;";
        
        using (SqlConnection connection = new SqlConnection(_connectionString))
        using (SqlCommand command = new SqlCommand(query, connection))
        {
            await connection.OpenAsync();
            command.Parameters.AddWithValue("@ClientId", clientId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (!reader.HasRows)
                    throw new ClientNotFoundException($"Nie znaleziono klienta o id: {clientId} lub on nie ma wypożyczeń!");

                while (await reader.ReadAsync())
                {
                    if (clientDto == null)
                    {
                        clientDto = new ClientDTO()
                        {
                            Id = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            Address = reader.GetString(3),
                            Rentals = new List<RentalDTO>()
                        };
                    }
                    clientDto.Rentals.Add(new RentalDTO()
                    {
                        Vin = reader.GetString(4),
                        Color = reader.GetString(5),
                        Model = reader.GetString(6),
                        DateFrom = reader.GetDateTime(7),
                        DateTo = reader.GetDateTime(8),
                        TotalPrice = reader.GetInt32(9),
                    });
                    
                }
            }
        }
        return clientDto;
    }



    

    public async Task PostClientAndRentAsync(CreateClientAndRentDTO client)
    {
        string query = @"select pricePerDay from cars where id=@IdCar";
        int pricePerDay = 0;
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync();
            SqlTransaction transaction = connection.BeginTransaction();
            try
            {
                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@IdCar", client.CarId);
                    var result = await command.ExecuteScalarAsync();
                    if (result == null)
                    {
                        throw new CarNotFoundException($"Nie znaleziono samochodu o id: {client.CarId}");
                    }
                    pricePerDay = Convert.ToInt32(result);
                }

                
                query = @"INSERT INTO CLIENTS(FIRSTNAME, LASTNAME, ADDRESS) VALUES(@FirstName, @LastName, @Address);
                            Select @@IDENTITY ";
                int newId = 0;

                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@FirstName", client.Client.FirstName);
                    command.Parameters.AddWithValue("@LastName", client.Client.LastName);
                    command.Parameters.AddWithValue("@Address", client.Client.Address);
                    
                    var res = await command.ExecuteScalarAsync();
                    if (res == null)
                    {
                        throw new Exception("Problem ze wstawianiem nowego klienta");
                    }
                    newId = Convert.ToInt32(res);
                }
                
                query = @"INSERT INTO CAR_RENTALS(CLIENTID, CARID, DATEFROM, DATETO, TOTALPRICE) 
                            VALUES (@ClientId, @CarId, @DateFrom, @DateTo, @TotalPrice);";

                using (SqlCommand command = new SqlCommand(query, connection, transaction))
                {
                    command.Parameters.AddWithValue("@ClientId", newId);
                    command.Parameters.AddWithValue("@CarId", client.CarId);
                    command.Parameters.AddWithValue("@DateFrom", client.DateFrom);
                    command.Parameters.AddWithValue("@DateTo", client.DateTo);
                    command.Parameters.AddWithValue("@TotalPrice", (client.DateTo - client.DateFrom).Days*pricePerDay);
                    
                    var result = await command.ExecuteNonQueryAsync();
                    if (result == 0)
                    {
                        throw new Exception($"Nie udało się wypożyczyć samochód o id: {client.CarId}");
                    }
                }
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
            }
        }
    }
    
    
}