using System;

namespace Norm.Tests
{
    internal class TestHelpers
    {
        private static Random _random = new Random();

        public static string GetRandomString(int length)
        {
            string validChars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] letters = new char[length];

            for (int i = 0; i < length; i++)
            {
                letters[i] = validChars[_random.Next(0, validChars.Length - 1)];
            }

            return new string(letters);
        }
    }
}
