using System;
using System.IO;
using SharpFuzz;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace YamlDotNet.Fuzz
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Fuzzer.OutOfProcess.Run(() =>
			{
				try
				{
					using (var file = File.OpenText(args[0]))
					{
						new YamlStream().Load(file);
					}
				}
				catch (ArgumentException) { }
				catch (YamlException) { }
			});
		}
	}
}
