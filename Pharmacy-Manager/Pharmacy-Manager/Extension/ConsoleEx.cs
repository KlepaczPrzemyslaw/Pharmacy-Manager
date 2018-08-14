using System;

namespace Pharmacy_Manager
{
	public static class ConsoleEx
	{
		public static void WriteLineWithColor(string text, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.WriteLine(text + "\n");
			Console.ResetColor();
		}

		public static void WriteWithColor(string text, ConsoleColor color)
		{
			Console.ForegroundColor = color;
			Console.Write(text + "\n");
			Console.ResetColor();
		}

		public static void ImportantQuestion(string text)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write(text);
			Console.ResetColor();
		}
	}
}
