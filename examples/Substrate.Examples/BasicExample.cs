namespace Substrate.Examples;

public static class BasicExample
{
    public static void Run()
    {
        Console.WriteLine("=== Basic Example ===");
        Console.WriteLine();

        using var substrate = new Engine.Substrate();
        var users = substrate.CreateTable("users");
        
        var ids = users.AddColumn<int>("id");
        var names = users.AddColumn<string>("name");
        var ages = users.AddColumn<int>("age");

        // Add some rows
        for (int i = 0; i < 5; i++)
        {
            int row = users.AddRow();
            ids[row] = i + 1;
            names[row] = $"User{i + 1}";
            ages[row] = 20 + i;
        }

        Console.WriteLine($"Created table '{users.Name}' with {users.RowCount} rows");
        
        // Display data
        for (int i = 0; i < users.RowCount; i++)
        {
            Console.WriteLine($"Row {i}: ID={ids[i]}, Name={names[i]}, Age={ages[i]}");
        }

        Console.WriteLine();
        Console.WriteLine($"Memory used: {substrate.MemoryBudget.Used} bytes");
        Console.WriteLine($"Memory pressure: {substrate.MemoryBudget.Pressure:P2}");
        Console.WriteLine();
    }
}
