namespace Substrate.Examples;

public static class EGraphExample
{
    public static void Run()
    {
        Console.WriteLine("=== E-Graph Example ===");
        Console.WriteLine();

        using var substrate = new Engine.Substrate();
        var nodes = substrate.CreateTable("nodes");
        
        var classIds = nodes.AddColumn<int>("class_id");
        var hashes = nodes.AddColumn<uint>("hash");
        var operations = nodes.AddColumn<string>("operation");

        // Add some e-graph nodes
        var nodeData = new[]
        {
            (ClassId: 1, Hash: 0x12345678u, Op: "Add"),
            (ClassId: 1, Hash: 0x87654321u, Op: "Mul"),
            (ClassId: 2, Hash: 0xABCDEF00u, Op: "Const"),
            (ClassId: 3, Hash: 0xDEADBEEFu, Op: "Var"),
            (ClassId: 2, Hash: 0xCAFEBABEu, Op: "Const"),
        };

        foreach (var data in nodeData)
        {
            int row = nodes.AddRow();
            classIds[row] = data.ClassId;
            hashes[row] = data.Hash;
            operations[row] = data.Op;
        }

        Console.WriteLine($"Created e-graph with {nodes.RowCount} nodes");
        
        // Display nodes
        for (int i = 0; i < nodes.RowCount; i++)
        {
            Console.WriteLine($"Node {i}: ClassId={classIds[i]}, Hash=0x{hashes[i]:X8}, Op={operations[i]}");
        }

        // Demonstrate SIMD operations on hashes
        var hashSpan = hashes.AsSpan();
        var hashArray = hashSpan.ToArray();
        var intHashArray = new int[hashArray.Length];
        for (int i = 0; i < hashArray.Length; i++)
        {
            intHashArray[i] = (int)hashArray[i];
        }

        Console.WriteLine();
        Console.WriteLine($"Min hash: 0x{Compute.ColumnOps.Min(intHashArray):X8}");
        Console.WriteLine($"Max hash: 0x{Compute.ColumnOps.Max(intHashArray):X8}");

        Console.WriteLine();
    }
}
