using System;

namespace Delhivery.ConsoleApp.Helpers
{
    public static class DisplayHelper
    {
        public static void ShowHeader(string title)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine($" {title}");
            Console.WriteLine("========================================");
        }

        public static void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public static void Pause()
        {
            Console.WriteLine();
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}