using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Enums;

namespace DryCleaning.Tests;

/// <summary>
/// Фикстура, содержащая тестовые данные химчистк
/// </summary>
public class DryCleaningFixture
{
    /// <summary>
    /// Момент времени, относительно которого построены тестовые данные
    /// </summary>
    public DateTime Now { get; }

    /// <summary>
    /// Дата, относительно которой построены тестовые данные
    /// </summary>
    public DateOnly Today { get; }

    /// <summary>
    /// Справочник категорий изделий
    /// </summary>
    public readonly List<Category> Categories;

    /// <summary>
    /// Клиенты химчистки
    /// </summary>
    public readonly List<Client> Clients;

    /// <summary>
    /// Принятые изделия (по одному на каждый заказ, Id изделия = Id заказа
    /// </summary>
    public readonly List<Item> Items;

    /// <summary>
    /// Заказы химчистки
    /// </summary>
    public readonly List<Order> Orders;

    /// <summary>
    /// Инициализирует тестовые данные
    /// </summary>
    public DryCleaningFixture()
    {
        Now = DateTime.Now;
        Today = DateOnly.FromDateTime(Now);

        Categories =
        [
            new Category { CategoryId = 0, Name = "Верхняя одежда",  RecommendedCleaningType = CleaningType.Dry,     CleaningPrice = 1500m },
            new Category { CategoryId = 1, Name = "Костюмы",         RecommendedCleaningType = CleaningType.Dry,     CleaningPrice = 950m },
            new Category { CategoryId = 2, Name = "Платья",          RecommendedCleaningType = CleaningType.Aqua,    CleaningPrice = 800m },
            new Category { CategoryId = 3, Name = "Рубашки",         RecommendedCleaningType = CleaningType.Aqua,    CleaningPrice = 350m },
            new Category { CategoryId = 4, Name = "Брюки",           RecommendedCleaningType = CleaningType.Dry,     CleaningPrice = 450m },
            new Category { CategoryId = 5, Name = "Ковры",           RecommendedCleaningType = CleaningType.Wet,     CleaningPrice = 2500m },
            new Category { CategoryId = 6, Name = "Меховые изделия", RecommendedCleaningType = CleaningType.Fur,     CleaningPrice = 3200m },
            new Category { CategoryId = 7, Name = "Кожаные изделия", RecommendedCleaningType = CleaningType.Leather, CleaningPrice = 1900m },
            new Category { CategoryId = 8, Name = "Шторы",           RecommendedCleaningType = CleaningType.Wet,     CleaningPrice = 1200m },
            new Category { CategoryId = 9, Name = "Одеяла",          RecommendedCleaningType = CleaningType.Wet,     CleaningPrice = 1000m },
        ];

        Clients =
        [
            new Client { ClientId = 0,  FullName = "Иванов Иван Иванович",          Phone = "+7 (912) 345-67-89" },
            new Client { ClientId = 1,  FullName = "Петров Пётр Петрович",          Phone = "+7 (923) 456-78-90" },
            new Client { ClientId = 2,  FullName = "Сидорова Анна Сергеевна",       Phone = "+7 (934) 567-89-01" },
            new Client { ClientId = 3,  FullName = "Кузнецов Алексей Владимирович", Phone = "+7 (945) 678-90-12" },
            new Client { ClientId = 4,  FullName = "Смирнова Елена Андреевна",      Phone = "+7 (956) 789-01-23" },
            new Client { ClientId = 5,  FullName = "Васильев Дмитрий Николаевич",   Phone = "+7 (967) 890-12-34" },
            new Client { ClientId = 6,  FullName = "Морозова Ольга Ивановна",       Phone = "+7 (978) 901-23-45" },
            new Client { ClientId = 7,  FullName = "Фёдоров Сергей Павлович",       Phone = "+7 (989) 012-34-56" },
            new Client { ClientId = 8,  FullName = "Новикова Мария Александровна",  Phone = "+7 (990) 123-45-67" },
            new Client { ClientId = 9,  FullName = "Волков Артём Игоревич",         Phone = "+7 (901) 234-56-78" },
            new Client { ClientId = 10, FullName = "Зайцева Наталья Викторовна",    Phone = "+7 (912) 345-67-80" },
            new Client { ClientId = 11, FullName = "Соловьёв Игорь Олегович",       Phone = "+7 (923) 456-78-91" },
        ];

        // Дата приёма как смещение в днях от Today
        DateOnly Day(int offset) => Today.AddDays(offset);

        // Заказ и изделие создаются парой
        Order NewOrder(int id, Client client, string itemName, string material,
            Category category, int receiptOffset, int termDays, OrderStatus status) => new()
        {
            OrderId = id,
            Client = client,
            Item = new Item { ItemId = id, Name = itemName, Material = material, Category = category },
            ReceiptDate = Day(receiptOffset),
            CompletionTermDays = termDays,
            Status = status
        };

        Orders =
        [
            // Заказы внутри "заданного периода" [Today-100; Today-41] (включительно)
            // Изделий за период: Иванов - 4, Петров - 3, Сидорова - 3,
            // Кузнецов - 2, Смирнова - 2, Васильев - 1
            NewOrder(0,  Clients[0],  "Пальто", "шерсть", Categories[0], -42, 5, OrderStatus.Issued),
            NewOrder(1,  Clients[0],  "Пиджак", "полушерсть", Categories[1], -50, 4, OrderStatus.Completed),
            NewOrder(2,  Clients[0],  "Рубашка", "хлопок", Categories[3], -90, 3, OrderStatus.Completed),
            NewOrder(3,  Clients[0],  "Брюки классические", "шерсть", Categories[4], -100, 3, OrderStatus.Issued), // нижняя граница периода
            NewOrder(4,  Clients[1],  "Пуховик", "синтепон", Categories[0], -45, 5, OrderStatus.Issued),
            NewOrder(5,  Clients[1],  "Пиджак", "полушерсть", Categories[1], -65, 4, OrderStatus.Issued),
            NewOrder(6,  Clients[1],  "Ковёр", "шерсть", Categories[5], -41, 10, OrderStatus.Completed), // верхняя граница периода
            NewOrder(7,  Clients[2],  "Пальто", "кашемир", Categories[0], -55, 5, OrderStatus.Completed),
            NewOrder(8,  Clients[2],  "Платье вечернее", "шёлк", Categories[2], -70, 3, OrderStatus.Issued),
            NewOrder(9,  Clients[2],  "Джинсы", "деним", Categories[4], -95, 3, OrderStatus.Completed),
            NewOrder(10, Clients[3],  "Пиджак", "полушерсть", Categories[1], -60, 4, OrderStatus.Issued),
            NewOrder(11, Clients[3],  "Рубашка", "хлопок", Categories[3], -75, 3, OrderStatus.Issued),
            NewOrder(12, Clients[4],  "Пальто", "драп", Categories[0], -80, 5, OrderStatus.Issued),
            NewOrder(13, Clients[4],  "Платье", "вискоза", Categories[2], -85, 3, OrderStatus.Completed),
            NewOrder(14, Clients[5],  "Рубашка", "хлопок", Categories[3], -99, 3, OrderStatus.Completed),

            // Свежие заказы: 15–18 - "В обработке"; у 17 и 18 - максимальный срок (14 дней)
            NewOrder(15, Clients[1],  "Пальто", "шерсть", Categories[0], -1, 5, OrderStatus.InProcessing),
            NewOrder(16, Clients[5],  "Пиджак", "полушерсть", Categories[1], -12, 4, OrderStatus.InProcessing),
            NewOrder(17, Clients[6],  "Шуба", "норка", Categories[6], -15, 14, OrderStatus.InProcessing),
            NewOrder(18, Clients[11], "Куртка кожаная", "кожа", Categories[7], -20, 14, OrderStatus.InProcessing),
            NewOrder(19, Clients[8],  "Платье вечернее", "шёлк", Categories[2], -300, 3, OrderStatus.Issued),
            NewOrder(20, Clients[0],  "Пуховик", "синтепон", Categories[0], -3, 6, OrderStatus.Accepted),
            NewOrder(21, Clients[4],  "Пиджак", "полушерсть", Categories[1], -5, 4, OrderStatus.Completed),

            // Заказы старше года. Заказ 22 - тоже срок 14 дней; заказ 25 - отменён (не оплачивается)
            NewOrder(22, Clients[6],  "Шторы", "лён", Categories[8], -380, 14, OrderStatus.Issued),
            NewOrder(23, Clients[10], "Одеяло", "синтепон", Categories[9], -400, 7, OrderStatus.Issued),
            NewOrder(24, Clients[9],  "Куртка кожаная", "кожа", Categories[7], -390, 6, OrderStatus.Completed),
            NewOrder(25, Clients[7],  "Ковёр", "шерсть", Categories[5], -420, 8, OrderStatus.Cancelled),
            NewOrder(26, Clients[9],  "Пальто", "драп", Categories[0], -370, 5, OrderStatus.Issued),
            NewOrder(27, Clients[10], "Шуба", "норка", Categories[6], -410, 10, OrderStatus.Issued),
        ];

        Items = Orders.Select(o => o.Item).ToList();
    }
}