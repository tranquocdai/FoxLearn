using System;

namespace FoxLearn.License
{
	public class Base32Converter
	{
		private static readonly char[] BASE32_TABLE = new char[32]
		{
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9',
			'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K',
			'L', 'M', 'N', 'P', 'R', 'S', 'T', 'V', 'W', 'X',
			'Y', 'Z'
		};

		public static string ToBase32String(byte[] buffer)
		{
			char[] array = new char[buffer.Length * 2];
			int num = buffer.Length % 3;
			if (num != 0)
			{
				throw new InvalidOperationException("Input data incorrect. Required multiple of 3 bytes length.");
			}
			int num2 = buffer.Length - num;
			int num3 = 0;
			for (int i = 0; i < num2; i += 3)
			{
				array[num3 + 0] = BASE32_TABLE[(buffer[i] & 0xF8) >> 3];
				array[num3 + 1] = BASE32_TABLE[((buffer[i] & 7) << 2) | ((buffer[i + 1] & 0xC0) >> 6)];
				array[num3 + 2] = BASE32_TABLE[(buffer[i + 1] & 0x3E) >> 1];
				array[num3 + 3] = BASE32_TABLE[((buffer[i + 1] & 1) << 4) | ((buffer[i + 2] & 0xF0) >> 4)];
				array[num3 + 4] = BASE32_TABLE[buffer[i + 2] & 0xF];
				num3 += 5;
			}
			return new string(array, 0, num3);
		}

		public static byte[] FromBase32String(string base32)
		{
			byte[] array = new byte[base32.Length];
			int num = base32.Length % 5;
			if (num != 0)
			{
				throw new InvalidOperationException("Base32 input string incorrect. Required multiple of 5 character length.");
			}
			int num2 = base32.Length - num;
			int num3 = 0;
			for (int i = 0; i < num2; i += 5)
			{
				long num4 = GetBase32Number(base32[i]) << 19;
				num4 |= (long)GetBase32Number(base32[i + 1]) << 14;
				num4 |= (long)GetBase32Number(base32[i + 2]) << 9;
				num4 |= (long)GetBase32Number(base32[i + 3]) << 4;
				num4 |= (byte)GetBase32Number(base32[i + 4]);
				array[num3 + 0] = (byte)((num4 & 0xFF0000) >> 16);
				array[num3 + 1] = (byte)((num4 & 0xFF00) >> 8);
				array[num3 + 2] = (byte)(num4 & 0xFF);
				num3 += 3;
			}
			byte[] array2 = new byte[num3];
			Array.Copy(array, 0, array2, 0, num3);
			return array2;
		}

		private static int GetBase32Number(char c)
		{
			if (c == 'I' || c == 'O' || c == 'Q' || c == 'U')
			{
				throw new ArgumentOutOfRangeException();
			}
			int num = c - 48;
			if (num > 9)
			{
				num -= 16;
				if (num > 9)
				{
					num--;
					if (num > 13)
					{
						num--;
						if (num > 14)
						{
							num--;
							if (num > 17)
							{
								num--;
							}
						}
					}
				}
				num += 9;
			}
			if (num < 0 || num > 31)
			{
				throw new ArgumentOutOfRangeException();
			}
			return num;
		}
	}
}
