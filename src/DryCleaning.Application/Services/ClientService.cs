using System.ComponentModel.DataAnnotations;
using DryCleaning.Application.Exceptions;
using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Interfaces;
using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Interfaces;

namespace DryCleaning.Application.Services;

/// <summary>
/// Операции с клиентами и преобразование данных в DTO
/// </summary>
public class ClientService(
    IClientRepository clientRepository,
    IOrderRepository orderRepository) : IClientService
{
    /// <inheritdoc/>
    public async Task<IReadOnlyList<ClientDto>> GetClientsAsync() =>
        [.. (await clientRepository.GetAllAsync()).Select(MapToDto)];

    /// <inheritdoc/>
    public async Task<ClientDto?> GetClientAsync(int id)
    {
        var client = await clientRepository.GetByIdAsync(id);
        return client is null ? null : MapToDto(client);
    }

    /// <inheritdoc/>
    public async Task<ClientDto> AddClientAsync(UpsertClientDto client) =>
        MapToDto(await clientRepository.AddAsync(MapToEntity(client)));

    /// <inheritdoc/>
    public async Task<ClientDto?> UpdateClientAsync(int id, UpsertClientDto client)
    {
        if (await clientRepository.GetByIdAsync(id) is null)
        {
            return null;
        }

        var entity = MapToEntity(client);
        if (!await clientRepository.UpdateAsync(id, entity))
        {
            return null;
        }

        entity.ClientId = id;
        return MapToDto(entity);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteClientAsync(int id)
    {
        if (await orderRepository.HasClientAsync(id))
        {
            throw new EntityInUseException("За клиентом числятся заказы — сначала удалите их.");
        }

        return await clientRepository.DeleteAsync(id);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ClientItemsCountDto>> GetTopClientsByItemsCountAsync(
        DateOnly from, DateOnly to, int count = 5) =>
        [.. (await orderRepository.GetTopClientsByItemsCountAsync(from, to, count))
            .Select(x => new ClientItemsCountDto
            {
                ClientId = x.Client.ClientId,
                FullName = x.Client.FullName,
                ItemsCount = x.ItemsCount,
            })];

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ClientDto>> GetClientsWithLongestProcessingAsync() =>
        [.. (await orderRepository.GetClientsWithLongestProcessingAsync()).Select(MapToDto)];

    /// <inheritdoc/>
    public async Task<IReadOnlyList<ClientSpendingDto>> GetClientsSpendingAsync()
    {
        var spending = await orderRepository.GetClientSpendingAsync();
        var clients = await clientRepository.GetAllAsync();

        return [.. clients
            .Select(client => new ClientSpendingDto
            {
                ClientId = client.ClientId,
                FullName = client.FullName,
                TotalSpent = spending.GetValueOrDefault(client.ClientId),
            })
            .OrderByDescending(x => x.TotalSpent)
            .ThenBy(x => x.FullName, StringComparer.Ordinal)];
    }

    /// <inheritdoc/>
    public async Task<ClientSpendingDto?> GetTopSpenderAsync() =>
        (await GetClientsSpendingAsync()).FirstOrDefault(client => client.TotalSpent > 0);

    /// <summary>
    /// Преобразует сущность в DTO
    /// </summary>
    private static ClientDto MapToDto(Client client) => new()
    {
        ClientId = client.ClientId,
        FullName = client.FullName,
        Phone = client.Phone,
    };

    /// <summary>
    /// Преобразует DTO в сущность с проверкой данных
    /// </summary>
    private static Client MapToEntity(UpsertClientDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), validateAllProperties: true);
        return new Client
        {
            ClientId = 0,
            FullName = dto.FullName,
            Phone = dto.Phone,
        };
    }
}
