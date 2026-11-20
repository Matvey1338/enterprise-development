namespace DryCleaning.Domain.Enums;

/// <summary>
/// Вид чистки
/// </summary>
public enum CleaningType
{
    /// <summary>Сухая чистка</summary>
    Dry = 1,

    /// <summary>Аквачистка</summary>
    Aqua = 2,

    /// <summary>Влажная чистка</summary>   
    Wet = 3,

    /// <summary>Чистка кожаных изделий</summary>
    Leather = 4,

    /// <summary>Чистка меховых изделий</summary>
    Fur = 5
}