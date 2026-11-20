using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Enums;

namespace DryCleaning.Domain.Seeds;

/// <summary>
/// Генератор набора данных химчистки: не менее 10 экземпляров каждого класса.
/// </summary>
public static class DryCleaningDataSeeder
{
    /// <summary>
    /// Создаёт набор данных относительно указанного момента времени
    /// </summary>
    /// <param name="now">Момент времени, относительно которого вычисляются даты;
    /// по умолчанию — текущий момент.</param>
    public static DryCleaningData Create(DateTime? now = null)
    {
        var moment = now ?? DateTime.Now;
        var referenceDate = DateOnly.FromDateTime(moment);

        IReadOnlyList<Category> categories =
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

        IReadOnlyList<Client> clients =
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

        // Дата приёма как смещение в днях от опорной даты
        DateOnly Day(int offset) => referenceDate.AddDays(offset);

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

        IReadOnlyList<Order> orders =
        [
            // Заказы внутри «заданного периода» [ReferenceDate-100; ReferenceDate-41] (включительно)
            // Изделий за период: Иванов - 4, Петров - 3, Сидорова - 3,
            // Кузнецов - 2, Смирнова - 2, Васильев - 1
            NewOrder(0,  clients[0],  "Пальто", "шерсть", categories[0], -42, 5, OrderStatus.Issued),
            NewOrder(1,  clients[0],  "Пиджак", "полушерсть", categories[1], -50, 4, OrderStatus.Completed),
            NewOrder(2,  clients[0],  "Рубашка", "хлопок", categories[3], -90, 3, OrderStatus.Completed),
            NewOrder(3,  clients[0],  "Брюки классические", "шерсть", categories[4], -100, 3, OrderStatus.Issued), // нижняя граница периода
            NewOrder(4,  clients[1],  "Пуховик", "синтепон", categories[0], -45, 5, OrderStatus.Issued),
            NewOrder(5,  clients[1],  "Пиджак", "полушерсть", categories[1], -65, 4, OrderStatus.Issued),
            NewOrder(6,  clients[1],  "Ковёр", "шерсть", categories[5], -41, 10, OrderStatus.Completed), // верхняя граница периода
            NewOrder(7,  clients[2],  "Пальто", "кашемир", categories[0], -55, 5, OrderStatus.Completed),
            NewOrder(8,  clients[2],  "Платье вечернее", "шёлк", categories[2], -70, 3, OrderStatus.Issued),
            NewOrder(9,  clients[2],  "Джинсы", "деним", categories[4], -95, 3, OrderStatus.Completed),
            NewOrder(10, clients[3],  "Пиджак", "полушерсть", categories[1], -60, 4, OrderStatus.Issued),
            NewOrder(11, clients[3],  "Рубашка", "хлопок", categories[3], -75, 3, OrderStatus.Issued),
            NewOrder(12, clients[4],  "Пальто", "драп", categories[0], -80, 5, OrderStatus.Issued),
            NewOrder(13, clients[4],  "Платье", "вискоза", categories[2], -85, 3, OrderStatus.Completed),
            NewOrder(14, clients[5],  "Рубашка", "хлопок", categories[3], -99, 3, OrderStatus.Completed),

            // Свежие заказы: 15–18 - "В обработке"; у 17 и 18 - максимальный срок (14 дней)
            NewOrder(15, clients[1],  "Пальто", "шерсть", categories[0], -1, 5, OrderStatus.InProcessing),
            NewOrder(16, clients[5],  "Пиджак", "полушерсть", categories[1], -12, 4, OrderStatus.InProcessing),
            NewOrder(17, clients[6],  "Шуба", "норка", categories[6], -15, 14, OrderStatus.InProcessing),
            NewOrder(18, clients[11], "Куртка кожаная", "кожа", categories[7], -20, 14, OrderStatus.InProcessing),
            NewOrder(19, clients[8],  "Платье вечернее", "шёлк", categories[2], -300, 3, OrderStatus.Issued),
            NewOrder(20, clients[0],  "Пуховик", "синтепон", categories[0], -3, 6, OrderStatus.Accepted),
            NewOrder(21, clients[4],  "Пиджак", "полушерсть", categories[1], -5, 4, OrderStatus.Completed),

            // Заказы старше года. Заказ 22 - тоже срок 14 дней; заказ 25 - отменён (не оплачивается)
            NewOrder(22, clients[6],  "Шторы", "лён", categories[8], -380, 14, OrderStatus.Issued),
            NewOrder(23, clients[10], "Одеяло", "синтепон", categories[9], -400, 7, OrderStatus.Issued),
            NewOrder(24, clients[9],  "Куртка кожаная", "кожа", categories[7], -390, 6, OrderStatus.Completed),
            NewOrder(25, clients[7],  "Ковёр", "шерсть", categories[5], -420, 8, OrderStatus.Cancelled),
            NewOrder(26, clients[9],  "Пальто", "драп", categories[0], -370, 5, OrderStatus.Issued),
            NewOrder(27, clients[10], "Шуба", "норка", categories[6], -410, 10, OrderStatus.Issued),
        ];

        return new DryCleaningData
        {
            Categories = categories,
            Clients = clients,
            Items = [.. orders.Select(o => o.Item)],
            Orders = orders,
        };
    }
}
