using System.IO;

namespace NCrontab.Run
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var text = File.ReadAllText(args[0]);
			CrontabSchedule.Parse(text);
		}
	}
}
