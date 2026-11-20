using DryCleaning.Domain.Entities;

namespace DryCleaning.Domain.Models;

/// <summary>
/// Результат аналитического запроса: клиент и число сданных им изделий за период
/// </summary>
/// <param name="Client">Клиент</param>
/// <param name="ItemsCount">Число изделий, сданных клиентом за период</param>
public readonly record struct ClientItemsCount(Client Client, int ItemsCount);
