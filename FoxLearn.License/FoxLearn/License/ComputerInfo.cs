using System.Management;
using System.Security.Cryptography;
using System.Text;

namespace FoxLearn.License
{
	public static class ComputerInfo
	{
		public static string GetComputerId()
		{
			return GetHash("CPU >> " + CpuId() + "\nBIOS >> " + BiosId() + "\nBASE >> " + BaseId());
		}

		private static string GetHash(string s)
		{
			MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
			byte[] bytes = new ASCIIEncoding().GetBytes(s);
			return GetHexString(mD5CryptoServiceProvider.ComputeHash(bytes));
		}

		private static string GetHexString(byte[] bt)
		{
			string text = string.Empty;
			for (int i = 0; i < bt.Length; i++)
			{
				byte num = bt[i];
				int num2 = num & 0xF;
				int num3 = (num >> 4) & 0xF;
				text = ((num3 <= 9) ? (text + num3) : (text + (char)(num3 - 10 + 65)));
				text = ((num2 <= 9) ? (text + num2) : (text + (char)(num2 - 10 + 65)));
				if (i + 1 != bt.Length && (i + 1) % 2 == 0)
				{
					text += "-";
				}
			}
			return text;
		}

		private static string Identifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
		{
			string text = "";
			foreach (ManagementObject instance in new ManagementClass(wmiClass).GetInstances())
			{
				if (instance[wmiMustBeTrue].ToString() == "True" && text == "")
				{
					try
					{
						text = instance[wmiProperty].ToString();
						return text;
					}
					catch
					{
					}
				}
			}
			return text;
		}

		private static string Identifier(string wmiClass, string wmiProperty)
		{
			string text = "";
			foreach (ManagementObject instance in new ManagementClass(wmiClass).GetInstances())
			{
				if (text == "")
				{
					try
					{
						text = instance[wmiProperty].ToString();
						return text;
					}
					catch
					{
					}
				}
			}
			return text;
		}

		private static string CpuId()
		{
			string text = Identifier("Win32_Processor", "UniqueId");
			if (text == "")
			{
				text = Identifier("Win32_Processor", "ProcessorId");
				if (text == "")
				{
					text = Identifier("Win32_Processor", "Name");
					if (text == "")
					{
						text = Identifier("Win32_Processor", "Manufacturer");
					}
					text += Identifier("Win32_Processor", "MaxClockSpeed");
				}
			}
			return text;
		}

		private static string BiosId()
		{
			return Identifier("Win32_BIOS", "Manufacturer") + Identifier("Win32_BIOS", "SMBIOSBIOSVersion") + Identifier("Win32_BIOS", "IdentificationCode") + Identifier("Win32_BIOS", "SerialNumber") + Identifier("Win32_BIOS", "ReleaseDate") + Identifier("Win32_BIOS", "Version");
		}

		private static string DiskId()
		{
			return Identifier("Win32_DiskDrive", "Model") + Identifier("Win32_DiskDrive", "Manufacturer") + Identifier("Win32_DiskDrive", "Signature") + Identifier("Win32_DiskDrive", "TotalHeads");
		}

		private static string BaseId()
		{
			return Identifier("Win32_BaseBoard", "Model") + Identifier("Win32_BaseBoard", "Manufacturer") + Identifier("Win32_BaseBoard", "Name") + Identifier("Win32_BaseBoard", "SerialNumber");
		}

		private static string VideoId()
		{
			return Identifier("Win32_VideoController", "DriverVersion") + Identifier("Win32_VideoController", "Name");
		}

		private static string MacId()
		{
			return Identifier("Win32_NetworkAdapterConfiguration", "MACAddress", "IPEnabled");
		}
	}
}
