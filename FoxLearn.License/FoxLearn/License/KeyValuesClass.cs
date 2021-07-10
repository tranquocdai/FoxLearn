using System;

namespace FoxLearn.License
{
	public class KeyValuesClass
	{
		public byte Header { get; set; }

		public byte Footer { get; set; }

		public byte ProductCode { get; set; }

		public Edition Edition { get; set; }

		public byte Version { get; set; }

		public LicenseType Type { get; set; }

		public DateTime Expiration { get; set; }
	}
}
