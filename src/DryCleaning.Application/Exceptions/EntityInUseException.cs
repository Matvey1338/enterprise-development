namespace DryCleaning.Application.Exceptions;

/// <summary>
/// Удаление невозможно из-за существующих ссылок на сущность
/// </summary>
/// <param name="message">Описание зависимости, препятствующей удалению</param>
public class EntityInUseException(string message) : Exception(message);
