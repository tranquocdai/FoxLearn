using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FoxLearn.Algorithm;
using FoxLearn.Encrypt;

namespace FoxLearn.License
{
	public class ObjectPacketLicense : ObjectEncryptor
	{
		private string _fileName;

		private byte[] _data;

		public ObjectPacketLicense()
		{
		}

		public ObjectPacketLicense(byte[] data)
		{
			_data = data;
		}

		public ObjectPacketLicense(string fileName)
		{
			_fileName = fileName;
		}

		public byte[] ObjectToByteArray(object obj)
		{
			if (obj == null)
			{
				return null;
			}
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			using MemoryStream memoryStream = new MemoryStream();
			binaryFormatter.Serialize(memoryStream, obj);
			return memoryStream.ToArray();
		}

		public object ByteArrayToObject(byte[] arrBytes)
		{
			using MemoryStream memoryStream = new MemoryStream();
			BinaryFormatter binaryFormatter = new BinaryFormatter();
			memoryStream.Write(arrBytes, 0, arrBytes.Length);
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return binaryFormatter.Deserialize(memoryStream);
		}

		protected virtual void WriteFile(LicenseInfo licInfo, byte version, AlgorithmType algType, AlgorithmKeyType algKeyType)
		{
			using FileStream fileStream = new FileStream(_fileName, FileMode.Create);
			using BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			try
			{
				binaryWriter.Write((short)5);
				binaryWriter.Write(version);
				binaryWriter.Write(Convert.ToByte((int)algType));
				binaryWriter.Write(Convert.ToByte((int)algKeyType));
				binaryWriter.Write(licInfo.Data);
				binaryWriter.Flush();
			}
			catch (IOException ex)
			{
				throw ex;
			}
			finally
			{
				binaryWriter.Close();
				fileStream.Close();
			}
		}

		protected virtual void WriteFile(LicenseInfo licInfo, byte version, Encryptor encryptor, AlgorithmType algType, AlgorithmKeyType algKeyType)
		{
			using FileStream fileStream = new FileStream(_fileName, FileMode.Create);
			using BinaryWriter binaryWriter = new BinaryWriter(fileStream);
			try
			{
				binaryWriter.Write((short)5);
				binaryWriter.Write(version);
				binaryWriter.Write(Convert.ToByte((int)algType));
				binaryWriter.Write(Convert.ToByte((int)algKeyType));
				binaryWriter.Write(encryptor.ObjectCryptography(licInfo.Data, TransformType.ENCRYPT));
				binaryWriter.Flush();
			}
			catch (IOException ex)
			{
				throw ex;
			}
			finally
			{
				binaryWriter.Close();
				fileStream.Close();
			}
		}

		protected virtual byte[] WriteStream(LicenseInfo licInfo, byte version, Encryptor encryptor, AlgorithmType algType, AlgorithmKeyType algKeyType)
		{
			using MemoryStream memoryStream = new MemoryStream();
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				try
				{
					binaryWriter.Write((short)5);
					binaryWriter.Write(version);
					binaryWriter.Write(Convert.ToByte((int)algType));
					binaryWriter.Write(Convert.ToByte((int)algKeyType));
					binaryWriter.Write(encryptor.ObjectCryptography(licInfo.Data, TransformType.ENCRYPT));
					binaryWriter.Flush();
				}
				catch (Exception ex)
				{
					throw ex;
				}
				finally
				{
					binaryWriter.Close();
					memoryStream.Close();
				}
			}
			return memoryStream.ToArray();
		}

		protected virtual byte[] WriteStream(LicenseInfo licInfo, byte version, AlgorithmType algType, AlgorithmKeyType algKeyType)
		{
			using MemoryStream memoryStream = new MemoryStream();
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				try
				{
					binaryWriter.Write((short)5);
					binaryWriter.Write(version);
					binaryWriter.Write(Convert.ToByte((int)algType));
					binaryWriter.Write(Convert.ToByte((int)algKeyType));
					binaryWriter.Write(licInfo.ProductKey);
					binaryWriter.Flush();
				}
				catch (IOException ex)
				{
					throw ex;
				}
				finally
				{
					binaryWriter.Close();
					memoryStream.Close();
				}
			}
			return memoryStream.ToArray();
		}

		protected virtual string ReadFile(short lHeader, byte version, out AlgorithmType algType, out AlgorithmKeyType algKeyType)
		{
			using FileStream fileStream = new FileStream(_fileName, FileMode.Open);
			using BinaryReader binaryReader = new BinaryReader(fileStream);
			string result = null;
			algType = AlgorithmType.None;
			algKeyType = AlgorithmKeyType.None;
			try
			{
				if (binaryReader.ReadInt16() == lHeader && binaryReader.ReadByte() == version)
				{
					algType = (AlgorithmType)binaryReader.ReadByte();
					algKeyType = (AlgorithmKeyType)binaryReader.ReadByte();
					result = binaryReader.ReadString();
				}
			}
			catch (IOException ex)
			{
				throw ex;
			}
			finally
			{
				binaryReader.Close();
				fileStream.Close();
			}
			return result;
		}

		protected virtual string ReadStream(short lHeader, byte version, out AlgorithmType algType, out AlgorithmKeyType algKeyType)
		{
			using MemoryStream memoryStream = new MemoryStream(_data);
			using BinaryReader binaryReader = new BinaryReader(memoryStream);
			string result = null;
			algType = AlgorithmType.None;
			algKeyType = AlgorithmKeyType.None;
			try
			{
				if (binaryReader.ReadInt16() == lHeader && binaryReader.ReadByte() == version)
				{
					algType = (AlgorithmType)binaryReader.ReadByte();
					algKeyType = (AlgorithmKeyType)binaryReader.ReadByte();
					result = binaryReader.ReadString();
				}
			}
			catch (IOException ex)
			{
				throw ex;
			}
			finally
			{
				binaryReader.Close();
				memoryStream.Close();
			}
			return result;
		}

		public virtual bool IsValidFileFormat(short lHeader, byte version)
		{
			using FileStream fileStream = new FileStream(_fileName, FileMode.Open);
			using BinaryReader binaryReader = new BinaryReader(fileStream);
			try
			{
				if (binaryReader.ReadInt16() == lHeader && binaryReader.ReadByte() == version)
				{
					return true;
				}
			}
			catch (IOException ex)
			{
				throw ex;
			}
			finally
			{
				binaryReader.Close();
				fileStream.Close();
			}
			return false;
		}

		public virtual bool IsValidStreamFormat(short lHeader, byte version)
		{
			using MemoryStream memoryStream = new MemoryStream(_data);
			using BinaryReader binaryReader = new BinaryReader(memoryStream);
			try
			{
				if (binaryReader.ReadInt16() == lHeader && binaryReader.ReadByte() == version)
				{
					return true;
				}
			}
			catch (IOException ex)
			{
				throw ex;
			}
			finally
			{
				binaryReader.Close();
				memoryStream.Close();
			}
			return false;
		}

		public virtual bool IsValidFileFormat(short lHeader, byte version, out AlgorithmType algType, out AlgorithmKeyType algKeyType)
		{
			using FileStream fileStream = new FileStream(_fileName, FileMode.Open);
			using BinaryReader binaryReader = new BinaryReader(fileStream);
			algType = AlgorithmType.None;
			algKeyType = AlgorithmKeyType.None;
			try
			{
				if (binaryReader.ReadInt16() == lHeader && binaryReader.ReadByte() == version)
				{
					algType = (AlgorithmType)binaryReader.ReadByte();
					algKeyType = (AlgorithmKeyType)binaryReader.ReadByte();
					return true;
				}
			}
			catch (IOException ex)
			{
				throw ex;
			}
			finally
			{
				binaryReader.Close();
				fileStream.Close();
			}
			return false;
		}

		public virtual bool IsValidStreamFormat(short lHeader, byte version, out AlgorithmType algType, out AlgorithmKeyType algKeyType)
		{
			using MemoryStream memoryStream = new MemoryStream(_data);
			using BinaryReader binaryReader = new BinaryReader(memoryStream);
			algType = AlgorithmType.None;
			algKeyType = AlgorithmKeyType.None;
			try
			{
				if (binaryReader.ReadInt16() == lHeader && binaryReader.ReadByte() == version)
				{
					algType = (AlgorithmType)binaryReader.ReadByte();
					algKeyType = (AlgorithmKeyType)binaryReader.ReadByte();
					return true;
				}
			}
			catch (IOException ex)
			{
				throw ex;
			}
			finally
			{
				binaryReader.Close();
				memoryStream.Close();
			}
			return false;
		}

		public virtual LicenseInfo ReadLicense(string secretKey, byte version)
		{
			string text = null;
			string text2 = null;
			text2 = ((_data == null) ? ReadFile(5, version, out var algType, out var algKeyType) : ReadStream(5, version, out algType, out algKeyType));
			if (text2 != null)
			{
				switch (algType)
				{
				case AlgorithmType.DES:
					try
					{
						base.Encryptor = new AlgorithmDES(secretKey, algKeyType);
						text = base.Encryptor.ObjectCryptography(text2, TransformType.DECRYPT);
					}
					catch (Exception ex3)
					{
						throw ex3;
					}
					break;
				case AlgorithmType.Rijndael:
					try
					{
						base.Encryptor = new AlgorithmRijndael(secretKey, algKeyType);
						text = base.Encryptor.ObjectCryptography(text2, TransformType.DECRYPT);
					}
					catch (Exception ex2)
					{
						throw ex2;
					}
					break;
				case AlgorithmType.TripleDES:
					try
					{
						base.Encryptor = new AlgorithmTripleDES(secretKey, algKeyType);
						text = base.Encryptor.ObjectCryptography(text2, TransformType.DECRYPT);
					}
					catch (Exception ex)
					{
						throw ex;
					}
					break;
				case AlgorithmType.None:
					text = text2;
					break;
				}
			}
			string[] array = text.Split('#');
			if (array.Length == 5)
			{
				return new LicenseInfo
				{
					FullName = array[0],
					ProductKey = array[1],
					Day = Convert.ToInt32(array[2]),
					Month = Convert.ToInt32(array[3]),
					Year = Convert.ToInt32(array[4])
				};
			}
			return null;
		}

		public virtual void SaveLicenseToFile(string secretKey, LicenseInfo licInfo, byte version, AlgorithmType algType, AlgorithmKeyType algKeyType)
		{
			switch (algType)
			{
			case AlgorithmType.DES:
				try
				{
					base.Encryptor = new AlgorithmDES(secretKey, algKeyType);
					WriteFile(licInfo, version, base.Encryptor, AlgorithmType.DES, algKeyType);
				}
				catch (Exception ex4)
				{
					throw ex4;
				}
				break;
			case AlgorithmType.Rijndael:
				try
				{
					base.Encryptor = new AlgorithmRijndael(secretKey, algKeyType);
					WriteFile(licInfo, version, base.Encryptor, AlgorithmType.Rijndael, algKeyType);
				}
				catch (Exception ex3)
				{
					throw ex3;
				}
				break;
			case AlgorithmType.TripleDES:
				try
				{
					base.Encryptor = new AlgorithmTripleDES(secretKey, algKeyType);
					WriteFile(licInfo, version, base.Encryptor, AlgorithmType.TripleDES, algKeyType);
				}
				catch (Exception ex2)
				{
					throw ex2;
				}
				break;
			case AlgorithmType.None:
				try
				{
					WriteFile(licInfo, version, AlgorithmType.None, AlgorithmKeyType.None);
				}
				catch (Exception ex)
				{
					throw ex;
				}
				break;
			}
		}

		public virtual byte[] SaveLicenseToStream(string secretKey, LicenseInfo licInfo, byte version, AlgorithmType algType, AlgorithmKeyType algKeyType)
		{
			byte[] result = null;
			switch (algType)
			{
			case AlgorithmType.DES:
				try
				{
					base.Encryptor = new AlgorithmDES(secretKey, algKeyType);
					return WriteStream(licInfo, version, base.Encryptor, AlgorithmType.DES, algKeyType);
				}
				catch (Exception ex4)
				{
					throw ex4;
				}
			case AlgorithmType.Rijndael:
				try
				{
					base.Encryptor = new AlgorithmRijndael(secretKey, algKeyType);
					return WriteStream(licInfo, version, base.Encryptor, AlgorithmType.Rijndael, algKeyType);
				}
				catch (Exception ex3)
				{
					throw ex3;
				}
			case AlgorithmType.TripleDES:
				try
				{
					base.Encryptor = new AlgorithmTripleDES(secretKey, algKeyType);
					return WriteStream(licInfo, version, base.Encryptor, AlgorithmType.TripleDES, algKeyType);
				}
				catch (Exception ex2)
				{
					throw ex2;
				}
			case AlgorithmType.None:
				try
				{
					return WriteStream(licInfo, version, AlgorithmType.None, AlgorithmKeyType.None);
				}
				catch (Exception ex)
				{
					throw ex;
				}
			default:
				return result;
			}
		}
	}
}
