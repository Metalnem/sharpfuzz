using System.Text.Json;
using SharpFuzz;

namespace SystemTextJson;

class Program
{
    public class X
    {
        public int A { get; set; }
        public string B { get; set; }
        public Dictionary<int, string> C { get; set; }
    }

    static void Main(string[] args)
    {
        Fuzzer.LibFuzzer.Run(span =>
        {
            try
            {
                JsonSerializer.Deserialize<X>(span);
            }
            catch { }
        });
    }
}
