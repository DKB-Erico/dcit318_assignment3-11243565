using System;
using System.Collections.Generic;

public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }
}

public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }
}

public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}


public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        return _items[id];
    }

    public void RemoveItem(int id)
    {
        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        _items.Remove(id);
    }

    public List<T> GetAllItems()
    {
        return new List<T>(_items.Values);
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
            throw new InvalidQuantityException("Quantity cannot be negative.");

        if (!_items.ContainsKey(id))
            throw new ItemNotFoundException($"Item with ID {id} not found.");

        _items[id].Quantity = newQuantity;
    }
}

public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
    private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 5, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Smartphone", 10, "Samsung", 12));

        _groceries.AddItem(new GroceryItem(1, "Milk", 20, DateTime.Now.AddDays(10)));
        _groceries.AddItem(new GroceryItem(2, "Bread", 15, DateTime.Now.AddDays(3)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
        }
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var currentItem = repo.GetItemById(id);
            repo.UpdateQuantity(id, currentItem.Quantity + quantity);
            Console.WriteLine($"Stock updated for {currentItem.Name}. New Quantity: {currentItem.Quantity}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating stock: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Item with ID {id} removed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing item: {ex.Message}");
        }
    }

    public InventoryRepository<ElectronicItem> Electronics => _electronics;
    public InventoryRepository<GroceryItem> Groceries => _groceries;
}

class Program
{
    static void Main(string[] args)
    {
        var manager = new WareHouseManager();
        manager.SeedData();

        while (true)
        {
            Console.WriteLine("\nWarehouse Menu:");
            Console.WriteLine("1. View Grocery Items");
            Console.WriteLine("2. View Electronic Items");
            Console.WriteLine("3. Try Adding Duplicate Item");
            Console.WriteLine("4. Try Removing Non-Existent Item");
            Console.WriteLine("5. Try Updating with Invalid Quantity");
            Console.WriteLine("Press Enter to Exit");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine();

            if (string.IsNullOrEmpty(choice)) break;

            switch (choice)
            {
                case "1":
                    Console.WriteLine("\nGrocery Items:");
                    manager.PrintAllItems(manager.Groceries);
                    break;
                case "2":
                    Console.WriteLine("\nElectronic Items:");
                    manager.PrintAllItems(manager.Electronics);
                    break;
                case "3":
                    try
                    {
                        manager.Electronics.AddItem(new ElectronicItem(1, "Tablet", 3, "Apple", 12));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    break;
                case "4":
                    manager.RemoveItemById(manager.Groceries, 99);
                    break;
                case "5":
                    try
                    {
                        manager.Electronics.UpdateQuantity(1, -5);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                    break;
                default:
                    Console.WriteLine("Invalid choice.");
                    break;
            }
        }

        Console.WriteLine("Exiting application...");
    }
}


