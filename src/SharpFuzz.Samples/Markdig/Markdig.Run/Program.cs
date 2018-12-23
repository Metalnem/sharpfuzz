using System.IO;

namespace Markdig.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var text = File.ReadAllText(args[0]);
			var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
			Markdown.ToHtml(text, pipeline);
		}
	}
}
