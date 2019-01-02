namespace HtmlAgilityPack.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			new HtmlDocument().Load(args[0]);
		}
	}
}
