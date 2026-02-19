namespace Substrate.Examples;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Substrate Examples");
        Console.WriteLine("==================");
        Console.WriteLine();

        BasicExample.Run();
        EGraphExample.Run();

        Console.WriteLine("Done!");
    }
}
