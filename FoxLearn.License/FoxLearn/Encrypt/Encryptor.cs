using System;
using System.Security.Cryptography;
using System.Text;
using FoxLearn.Algorithm;

namespace FoxLearn.Encrypt
{
	public abstract class Encryptor
	{
		public Encryptor(string secretKey, AlgorithmKeyType AlgType)
		{
			GenerateKey(secretKey, AlgType);
		}

		public string ObjectCryptography(string data, TransformType type)
		{
			string result = null;
			try
			{
				if (data.Length > 0)
				{
					switch (type)
					{
					case TransformType.ENCRYPT:
					{
						byte[] bytes = Encoding.UTF8.GetBytes(data);
						return Convert.ToBase64String(Transform(bytes, TransformType.ENCRYPT));
					}
					case TransformType.DECRYPT:
					{
						byte[] data2 = Convert.FromBase64String(data);
						return Encoding.UTF8.GetString(Transform(data2, TransformType.DECRYPT));
					}
					default:
						return result;
					}
				}
				return result;
			}
			catch (CryptographicException ex)
			{
				throw ex;
			}
		}

		public byte[] ObjectCryptography(byte[] data, TransformType type)
		{
			byte[] array = null;
			try
			{
				if (data != null)
				{
					if (data.Length != 0)
					{
						return type switch
						{
							TransformType.ENCRYPT => Transform(data, TransformType.ENCRYPT), 
							TransformType.DECRYPT => Transform(data, TransformType.DECRYPT), 
							_ => array, 
						};
					}
					return array;
				}
				return array;
			}
			catch (CryptographicException ex)
			{
				throw ex;
			}
		}

		public string Encrypt(string data)
		{
			try
			{
				if (data.Length > 0)
				{
					byte[] bytes = Encoding.UTF8.GetBytes(data);
					return Convert.ToBase64String(Transform(bytes, TransformType.ENCRYPT));
				}
				return null;
			}
			catch (CryptographicException ex)
			{
				throw ex;
			}
		}

		public string Decrypt(string data)
		{
			try
			{
				if (data.Length > 0)
				{
					byte[] data2 = Convert.FromBase64String(data);
					return Encoding.UTF8.GetString(Transform(data2, TransformType.DECRYPT));
				}
				return null;
			}
			catch (CryptographicException ex)
			{
				throw ex;
			}
		}

		public abstract void GenerateKey(string secretKey, AlgorithmKeyType type);

		public abstract byte[] Transform(byte[] data, TransformType type);
	}
}
