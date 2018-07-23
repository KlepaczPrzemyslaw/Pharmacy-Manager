using System;

namespace Pharmacy_Manager
{
	public static class ConsoleEx
	{
		public static void WriteLineInYellow(string text)
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine(text + "\n");
			Console.ResetColor();
		}

		public static void WriteLineInGreen(string text)
		{
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine(text + "\n");
			Console.ResetColor();
		}

		public static void WriteLineInCyan(string text)
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine(text + "\n");
			Console.ResetColor();
		}

		public static void WriteLineInRed(string text)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(text + "\n");
			Console.ResetColor();
		}

		public static void WriteInGreen(string text)
		{
			Console.ForegroundColor = ConsoleColor.Green;
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
