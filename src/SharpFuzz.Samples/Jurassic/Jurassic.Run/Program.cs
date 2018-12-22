namespace Jurassic.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var engine = new ScriptEngine();
			engine.ExecuteFile(args[0]);
		}
	}
}
