using System.IO;
using SharpFuzz;

namespace Markdig.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				var text = File.ReadAllText(args[0]);
				var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
				Markdown.ToHtml(text, pipeline);
			});
		}
	}
}
