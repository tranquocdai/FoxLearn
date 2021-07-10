using System.IO;
using System.Security.Cryptography;
using System.Text;
using FoxLearn.Encrypt;

namespace FoxLearn.Algorithm
{
	public class AlgorithmDES : Encryptor
	{
		private byte[] Key { get; set; }

		private byte[] IV { get; set; }

		public AlgorithmDES(string secretKey, AlgorithmKeyType AlgType)
			: base(secretKey, AlgType)
		{
		}

		public override void GenerateKey(string secretKey, AlgorithmKeyType type)
		{
			Key = new byte[8];
			IV = new byte[8];
			byte[] bytes = Encoding.UTF8.GetBytes(secretKey);
			switch (type)
			{
			case AlgorithmKeyType.MD5:
			{
				using (MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider())
				{
					mD5CryptoServiceProvider.ComputeHash(bytes);
					byte[] hash5 = mD5CryptoServiceProvider.Hash;
					for (int m = 0; m < 8; m++)
					{
						Key[m] = hash5[m];
					}
					for (int num5 = 15; num5 >= 8; num5--)
					{
						IV[15 - num5] = hash5[num5];
					}
				}
				break;
			}
			case AlgorithmKeyType.SHA1:
			{
				using (SHA1Managed sHA1Managed = new SHA1Managed())
				{
					sHA1Managed.ComputeHash(bytes);
					byte[] hash4 = sHA1Managed.Hash;
					for (int l = 0; l < 8; l++)
					{
						Key[l] = hash4[l];
					}
					for (int num4 = 19; num4 > 11; num4--)
					{
						IV[19 - num4] = hash4[num4];
					}
				}
				break;
			}
			case AlgorithmKeyType.SHA256:
			{
				using (SHA256Managed sHA256Managed = new SHA256Managed())
				{
					sHA256Managed.ComputeHash(bytes);
					byte[] hash3 = sHA256Managed.Hash;
					for (int k = 0; k < 8; k++)
					{
						Key[k] = hash3[k];
					}
					for (int num3 = 31; num3 >= 24; num3--)
					{
						IV[31 - num3] = hash3[num3];
					}
				}
				break;
			}
			case AlgorithmKeyType.SHA384:
			{
				using (SHA384Managed sHA384Managed = new SHA384Managed())
				{
					sHA384Managed.ComputeHash(bytes);
					byte[] hash2 = sHA384Managed.Hash;
					for (int j = 0; j < 8; j++)
					{
						Key[j] = hash2[j];
					}
					for (int num2 = 47; num2 > 39; num2--)
					{
						IV[47 - num2] = hash2[num2];
					}
				}
				break;
			}
			case AlgorithmKeyType.SHA512:
			{
				using (SHA512Managed sHA512Managed = new SHA512Managed())
				{
					sHA512Managed.ComputeHash(bytes);
					byte[] hash = sHA512Managed.Hash;
					for (int i = 0; i < 8; i++)
					{
						Key[i] = hash[i];
					}
					for (int num = 63; num > 55; num--)
					{
						IV[63 - num] = hash[num];
					}
				}
				break;
			}
			case AlgorithmKeyType.None:
				break;
			}
		}

		public override byte[] Transform(byte[] data, TransformType type)
		{
			MemoryStream memoryStream = null;
			ICryptoTransform cryptoTransform = null;
			DES dES = DES.Create();
			try
			{
				memoryStream = new MemoryStream();
				dES.Key = Key;
				dES.IV = IV;
				cryptoTransform = ((type != 0) ? dES.CreateDecryptor() : dES.CreateEncryptor());
				if (data != null && data.Length != 0)
				{
					CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
					cryptoStream.Write(data, 0, data.Length);
					cryptoStream.FlushFinalBlock();
					return memoryStream.ToArray();
				}
				return null;
			}
			catch (CryptographicException ex)
			{
				throw new CryptographicException(ex.Message);
			}
			finally
			{
				dES?.Clear();
				cryptoTransform?.Dispose();
				memoryStream.Close();
			}
		}
	}
}
