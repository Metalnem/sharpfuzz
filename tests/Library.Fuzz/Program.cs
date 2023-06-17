using SharpFuzz;
using System.IO;

namespace Library.Fuzz;

public class Program
{
    public static void Main(string[] args)
    {
        try {
            Fuzzer.Run(s =>
            {
                Parser.Parse(s);
            });
        }
        catch (Exception ex) {
            File.WriteAllText("log.txt", ex.ToString());
        }
    }
}
