using System;
using System.Text;

namespace Nupload
{
	public class RandomStringGenerator
	{
		public const string AllowedCharacters = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ23456789";

		private readonly Random _random;

		public RandomStringGenerator(Random random = null)
		{
			_random = random ?? new Random();
		}

		public string GenerateString(int length)
		{
			var password = new StringBuilder();
			for (int i = 0; i < length; i++)
			{
				password.Append(AllowedCharacters[_random.Next(0, AllowedCharacters.Length - 1)]);
			}
			return password.ToString();
		}
	}
}
