using SharpFuzz;

namespace Library.Fuzz;

public class Program
{
    public static void Main(string[] args)
    {
        Fuzzer.Run(s =>
        {
            Parser.Parse(s);
        });
    }
}
