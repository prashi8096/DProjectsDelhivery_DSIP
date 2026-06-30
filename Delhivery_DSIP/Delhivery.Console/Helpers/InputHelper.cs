using System;

namespace Delhivery.ConsoleApp.Helpers
{
    public static class InputHelper
    {
        public static string ReadString(string message)
        {
            string input;

            do
            {
                Console.Write(message);
                input = Console.ReadLine();
            }
            while (string.IsNullOrWhiteSpace(input));

            return input;
        }

        public static double ReadDouble(string message)
        {
            double value;

            while (true)
            {
                Console.Write(message);

                if (double.TryParse(Console.ReadLine(), out value) && value > 0)
                    return value;

                DisplayHelper.ShowError("Please enter a valid positive number.");
            }
        }

        public static int ReadInteger(string message)
        {
            int value;

            while (true)
            {
                Console.Write(message);

                if (int.TryParse(Console.ReadLine(), out value))
                    return value;

                DisplayHelper.ShowError("Please enter a valid integer.");
            }
        }
    }
}