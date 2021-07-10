using System;

namespace FoxLearn.License
{
	public class LicenseInfo
	{
		public string FullName { get; set; }

		public string ProductKey { get; set; }

		public int Day { get; set; }

		public int Month { get; set; }

		public int Year { get; set; }

		public DateTime CheckDate => new DateTime(Year, Month, Day).Date;

		public string Data => $"{FullName}#{ProductKey}#{Day}#{Month}#{Year}";
	}
}
