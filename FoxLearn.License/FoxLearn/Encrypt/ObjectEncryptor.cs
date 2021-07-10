using FoxLearn.Algorithm;

namespace FoxLearn.Encrypt
{
	public class ObjectEncryptor
	{
		protected Encryptor Encryptor { get; set; }

		public virtual string ObjectCryptography(string secretKey, string data, TransformType type, AlgorithmType algType, AlgorithmKeyType algKeyType)
		{
			string result = null;
			switch (algType)
			{
			case AlgorithmType.DES:
				Encryptor = new AlgorithmDES(secretKey, algKeyType);
				result = Encryptor.ObjectCryptography(data, type);
				break;
			case AlgorithmType.Rijndael:
				Encryptor = new AlgorithmRijndael(secretKey, algKeyType);
				result = Encryptor.ObjectCryptography(data, type);
				break;
			case AlgorithmType.TripleDES:
				Encryptor = new AlgorithmTripleDES(secretKey, algKeyType);
				result = Encryptor.ObjectCryptography(data, type);
				break;
			}
			return result;
		}
	}
}
