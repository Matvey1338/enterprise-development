using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Models;

namespace DryCleaning.Domain.Interfaces;

/// <summary>
/// Репозиторий заказов
/// </summary>
public interface IOrderRepository : IRepository<Order>
{
    /// <summary>
    /// Возвращает заказы, находящиеся в обработке, упорядоченные по дате приёма
    /// </summary>
    public Task<IReadOnlyList<Order>> GetInProcessingAsync();

    /// <summary>
    /// Возвращает топ клиентов по числу сданных изделий
    /// за период [from; to], границы периода включаются
    /// При равном числе изделий клиенты упорядочиваются по ФИО
    /// </summary>
    /// <param name="from">Начало периода</param>
    /// <param name="to">Конец периода</param>
    /// <param name="count">Количество клиентов в результате</param>
    public Task<IReadOnlyList<ClientItemsCount>> GetTopClientsByItemsCountAsync(
        DateOnly from, DateOnly to, int count);

    /// <summary>
    /// Возвращает клиентов, чьи заказы имеют максимальный срок выполнения,
    /// упорядоченных по ФИО. Клиент входит в результат один раз,
    /// даже если у него несколько заказов с максимальным сроком
    /// </summary>
    public Task<IReadOnlyList<Client>> GetClientsWithLongestProcessingAsync();

    /// <summary>
    /// Возвращает число заказов по каждой категории за период с даты <paramref name="since"/>.
    /// Категории без заказов за период в результате отсутствуют
    /// </summary>
    /// <param name="since">Дата начала периода</param>
    public Task<IReadOnlyDictionary<int, int>> GetCategoryOrdersCountsAsync(DateOnly since);

    /// <summary>
    /// Возвращает сумму потраченных клиентом средств за весь период работы химчистки
    /// Отменённые заказы не оплачиваются и не учитываются
    /// Клиенты без заказов в результате отсутствуют
    /// </summary>
    public Task<IReadOnlyDictionary<int, decimal>> GetClientSpendingAsync();

    /// <summary>
    /// Проверяет, существует ли заказ, ссылающийся на клиента
    /// с указанным идентификатором. Используется при удалении клиента:
    /// пока на него ссылаются заказы, клиента удалить нельзя
    /// </summary>
    /// <param name="clientId">Идентификатор клиента</param>
    public Task<bool> HasClientAsync(int clientId);

    /// <summary>
    /// Проверяет, существует ли заказ, ссылающийся на изделие
    /// с указанным идентификатором. Используется при удалении изделия:
    /// пока на него ссылаются заказы, изделие удалить нельзя
    /// </summary>
    /// <param name="itemId">Идентификатор изделия</param>
    public Task<bool> HasItemAsync(int itemId);
}
