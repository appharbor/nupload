using System;

namespace Nupload
{
	public static class DateTimeExtensions
	{
		public static long GetSecondsSinceUnixEpoch(this DateTime dateTime)
		{
			var unixTime = dateTime.ToUniversalTime() -
				new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			return (long)unixTime.TotalSeconds;
		}
	}
}
