using System.Text;
using SharpFuzz;

namespace Library.LibFuzzer;

public class Program
{
    public static void Main(string[] args)
    {
        Fuzzer.LibFuzzer.Run(span =>
        {
            var s = Encoding.UTF8.GetString(span);
            Parser.Parse(s);
        });
    }
}
