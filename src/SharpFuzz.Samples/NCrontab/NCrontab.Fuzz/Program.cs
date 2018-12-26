using System;
using System.IO;
using SharpFuzz;

namespace NCrontab.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.Run(() =>
			{
				try
				{
					var text = File.ReadAllText(args[0]);
					CrontabSchedule.Parse(text);
				}
				catch (CrontabException) { }
				catch (OverflowException) { }
			});
		}
	}
}
