using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using FoxLearn.Algorithm;

namespace FoxLearn.License
{
	public class KeyManager
	{
		private string EncryptionKey = string.Empty;

		public KeyManager(string encryptionKey)
		{
			EncryptionKey = encryptionKey;
		}

		public bool GenerateKey(KeyValuesClass KeyValues, ref string ProductKey)
		{
			CspParameters cspParameters = new CspParameters();
			cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
			new RSACryptoServiceProvider(1024, cspParameters);
			try
			{
				string empty = string.Empty;
				byte[] bytes = BitConverter.GetBytes(KeyValues.Header);
				byte[] bytes2 = BitConverter.GetBytes(KeyValues.ProductCode);
				byte[] bytes3 = BitConverter.GetBytes(KeyValues.Version);
				byte[] bytes4 = BitConverter.GetBytes((byte)KeyValues.Edition);
				byte[] bytes5 = BitConverter.GetBytes((byte)KeyValues.Type);
				byte[] bytes6 = BitConverter.GetBytes(Convert.ToUInt32(string.Concat(string.Concat(empty + KeyValues.Expiration.Day.ToString().PadLeft(2, '0'), KeyValues.Expiration.Month.ToString().PadLeft(2, '0')), KeyValues.Expiration.Year.ToString())));
				byte[] bytes7 = BitConverter.GetBytes(new Random().Next(0, 255));
				byte[] bytes8 = BitConverter.GetBytes(KeyValues.Footer);
				byte[] array;
				using (MemoryStream memoryStream = new MemoryStream())
				{
					memoryStream.Write(bytes, 0, 1);
					memoryStream.Write(bytes2, 0, 1);
					memoryStream.Write(bytes3, 0, 1);
					memoryStream.Write(bytes4, 0, 1);
					memoryStream.Write(bytes5, 0, 1);
					memoryStream.Write(bytes6, 0, 4);
					memoryStream.Write(bytes7, 0, 1);
					memoryStream.Write(bytes8, 0, 1);
					array = memoryStream.ToArray();
				}
				byte[] bytes9 = Encoding.UTF8.GetBytes(EncryptionKey);
				byte[] array2 = new byte[32];
				for (int i = 0; i < 32; i++)
				{
					array2[i] = bytes9[i];
				}
				byte[] array3 = new byte[16];
				int num = 0;
				for (int num2 = bytes9.Length - 1; num2 >= 23; num2--)
				{
					array3[num] = bytes9[num2];
					num++;
				}
				byte[] array4 = new RijndaelManaged().CreateEncryptor(array2, array3).TransformFinalBlock(array, 0, array.Length);
				byte[] buffer;
				using (MemoryStream memoryStream2 = new MemoryStream())
				{
					memoryStream2.Write(array4, 0, 8);
					memoryStream2.Write(bytes, 0, 1);
					memoryStream2.Write(bytes7, 0, 1);
					memoryStream2.Write(array4, 8, array4.Length - 8);
					buffer = memoryStream2.ToArray();
				}
				string text = Base32Converter.ToBase32String(buffer);
				ProductKey = $"{text.Substring(0, 5)}-{text.Substring(5, 5)}-{text.Substring(10, 5)}-{text.Substring(15, 5)}-{text.Substring(20, 5)}-{text.Substring(25, 5)}";
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool DisassembleKey(string ProductKey, ref KeyValuesClass KeyValues)
		{
			CspParameters cspParameters = new CspParameters();
			cspParameters.Flags = CspProviderFlags.UseMachineKeyStore;
			new RSACryptoServiceProvider(1024, cspParameters);
			try
			{
				if (string.IsNullOrEmpty(ProductKey))
				{
					throw new ArgumentNullException("Product Key is null or empty.");
				}
				if (ProductKey.Length != 35)
				{
					throw new ArgumentException("Product key is invalid.");
				}
				byte[] buffer = Base32Converter.FromBase32String(ProductKey.Replace("-", ""));
				byte[] buffer2 = new byte[2];
				byte[] buffer3 = new byte[1];
				byte[] array = new byte[16];
				using (MemoryStream memoryStream = new MemoryStream(buffer))
				{
					memoryStream.Read(array, 0, 8);
					memoryStream.Read(buffer2, 0, 1);
					memoryStream.Read(buffer3, 0, 1);
					memoryStream.Read(array, 8, array.Length - 8);
					memoryStream.ToArray();
				}
				byte[] bytes = Encoding.UTF8.GetBytes(EncryptionKey);
				byte[] array2 = new byte[32];
				for (int i = 0; i < 32; i++)
				{
					array2[i] = bytes[i];
				}
				byte[] array3 = new byte[16];
				int num = 0;
				for (int num2 = bytes.Length - 1; num2 >= 23; num2--)
				{
					array3[num] = bytes[num2];
					num++;
				}
				byte[] buffer4 = new RijndaelManaged().CreateDecryptor(array2, array3).TransformFinalBlock(array, 0, array.Length);
				byte[] array4 = new byte[2];
				byte[] array5 = new byte[2];
				byte[] array6 = new byte[2];
				byte[] array7 = new byte[2];
				byte[] array8 = new byte[2];
				byte[] array9 = new byte[4];
				byte[] buffer5 = new byte[2];
				byte[] array10 = new byte[2];
				using (MemoryStream memoryStream2 = new MemoryStream(buffer4))
				{
					memoryStream2.Read(array4, 0, 1);
					memoryStream2.Read(array5, 0, 1);
					memoryStream2.Read(array6, 0, 1);
					memoryStream2.Read(array7, 0, 1);
					memoryStream2.Read(array8, 0, 1);
					memoryStream2.Read(array9, 0, 4);
					memoryStream2.Read(buffer5, 0, 1);
					memoryStream2.Read(array10, 0, 1);
				}
				KeyValuesClass keyValuesClass = new KeyValuesClass();
				keyValuesClass.Header = (byte)BitConverter.ToInt16(array4, 0);
				keyValuesClass.ProductCode = (byte)BitConverter.ToInt16(array5, 0);
				keyValuesClass.Version = (byte)BitConverter.ToInt16(array6, 0);
				keyValuesClass.Edition = (Edition)BitConverter.ToInt16(array7, 0);
				keyValuesClass.Type = (LicenseType)BitConverter.ToInt16(array8, 0);
				if (keyValuesClass.Type == LicenseType.TRIAL)
				{
					string text = BitConverter.ToUInt32(array9, 0).ToString().PadLeft(8, '0');
					keyValuesClass.Expiration = new DateTime(Convert.ToInt16(text.Substring(4, 4)), Convert.ToInt16(text.Substring(2, 2)), Convert.ToInt16(text.Substring(0, 2)));
				}
				keyValuesClass.Footer = (byte)BitConverter.ToInt16(array10, 0);
				KeyValues = keyValuesClass;
				return true;
			}
			catch
			{
				return false;
			}
		}

		public bool ValidKey(ref string ProductKey)
		{
			using (new RSACryptoServiceProvider())
			{
				try
				{
					if (string.IsNullOrEmpty(ProductKey))
					{
						throw new ArgumentNullException("Product Key is null or empty.");
					}
					if (ProductKey.Length != 35)
					{
						throw new ArgumentException("Product key is invalid.");
					}
					byte[] buffer = Base32Converter.FromBase32String(ProductKey.Replace("-", ""));
					byte[] buffer2 = new byte[1];
					byte[] buffer3 = new byte[1];
					byte[] array = new byte[16];
					using (MemoryStream memoryStream = new MemoryStream(buffer))
					{
						memoryStream.Read(array, 0, 8);
						memoryStream.Read(buffer2, 0, 1);
						memoryStream.Read(buffer3, 0, 1);
						memoryStream.Read(array, 8, array.Length - 8);
						memoryStream.ToArray();
					}
					byte[] bytes = Encoding.UTF8.GetBytes(EncryptionKey);
					byte[] array2 = new byte[32];
					for (int i = 0; i < 32; i++)
					{
						array2[i] = bytes[i];
					}
					byte[] array3 = new byte[16];
					int num = 0;
					for (int num2 = bytes.Length - 1; num2 >= 23; num2--)
					{
						array3[num] = bytes[num2];
						num++;
					}
					new RijndaelManaged().CreateDecryptor(array2, array3).TransformFinalBlock(array, 0, array.Length);
					return true;
				}
				catch
				{
					return false;
				}
			}
		}

		public int LoadSuretyFile(string filename, ref LicenseInfo LicInfo)
		{
			if (!File.Exists(filename))
			{
				return -1;
			}
			ObjectPacketLicense objectPacketLicense = new ObjectPacketLicense(filename);
			try
			{
				LicenseInfo licenseInfo = objectPacketLicense.ReadLicense(EncryptionKey, 1);
				if (licenseInfo == null)
				{
					return -3;
				}
				LicInfo = licenseInfo;
				return 1;
			}
			catch
			{
				return -2;
			}
		}

		public bool SaveSuretyFile(string filename, LicenseInfo licInfo)
		{
			try
			{
				new ObjectPacketLicense(filename).SaveLicenseToFile(EncryptionKey, licInfo, 1, AlgorithmType.Rijndael, AlgorithmKeyType.MD5);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
