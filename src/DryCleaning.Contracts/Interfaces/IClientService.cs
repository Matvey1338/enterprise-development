using DryCleaning.Contracts.Dto;

namespace DryCleaning.Contracts.Interfaces;

/// <summary>
/// Сервис клиентов химчистки
/// </summary>
public interface IClientService
{
    /// <summary>
    /// Возвращает список всех клиентов
    /// </summary>
    public Task<IReadOnlyList<ClientDto>> GetClientsAsync();

    /// <summary>
    /// Возвращает клиента по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор клиента.</param>
    public Task<ClientDto?> GetClientAsync(int id);

    /// <summary>
    /// Добавляет клиента и возвращает созданную сущность с присвоенным идентификатором
    /// </summary>
    /// <param name="client">Данные клиента.</param>
    public Task<ClientDto> AddClientAsync(UpsertClientDto client);

    /// <summary>
    /// Заменяет клиента с указанным идентификатором
    /// </summary>
    /// <param name="id">Идентификатор клиента</param>
    /// <param name="client">Новые данные клиента</param>
    /// <returns>Обновлённый клиент или null, если он не найден</returns>
    public Task<ClientDto?> UpdateClientAsync(int id, UpsertClientDto client);

    /// <summary>
    /// Удаляет клиента по идентификатору
    /// Пока на клиента ссылаются заказы, удалить его нельзя
    /// </summary>
    /// <param name="id">Идентификатор клиента</param>
    /// <returns>true, если клиент найден и удалён</returns>
    public Task<bool> DeleteClientAsync(int id);

    /// <summary>
    /// Возвращает топ клиентов по числу сданных изделий за период [from; to]
    /// (границы периода включаются). При равном числе изделий клиенты
    /// упорядочиваются по ФИО
    /// </summary>
    /// <param name="from">Начало периода</param>
    /// <param name="to">Конец периода</param>
    /// <param name="count">Количество клиентов в результате</param>
    public Task<IReadOnlyList<ClientItemsCountDto>> GetTopClientsByItemsCountAsync(
        DateOnly from, DateOnly to, int count = 5);

    /// <summary>
    /// Возвращает клиентов, чьи заказы имеют максимальный срок выполнения,
    /// упорядоченных по ФИО. Клиент входит в результат один раз,
    /// даже если у него несколько заказов с максимальным сроком
    /// </summary>
    public Task<IReadOnlyList<ClientDto>> GetClientsWithLongestProcessingAsync();

    /// <summary>
    /// Возвращает клиента с наибольшей суммой трат за весь период работы
    /// химчистки или null, если нет ни одного оплаченного заказа
    /// Отменённые заказы не оплачиваются и не учитываются
    /// </summary>
    public Task<ClientSpendingDto?> GetTopSpenderAsync();
}
