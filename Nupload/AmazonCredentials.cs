using System;
using System.Security.Cryptography;
using System.Text;

namespace Nupload
{
	public class AmazonCredentials
	{
		private readonly string _accessKeyId;
		private readonly string _secretAccessKey;

		public AmazonCredentials(string accessKeyId, string secretAccessKey)
		{
			_accessKeyId = accessKeyId;
			_secretAccessKey = secretAccessKey;
		}

		public string AccessKeyId
		{
			get
			{
				return _accessKeyId;
			}
		}

		public string SecretAccessKey
		{
			get
			{
				return _secretAccessKey;
			}
		}

		public string GetSignature(string input)
		{
			var encoding = Encoding.UTF8;
			using (var hashAlgorithm = new HMACSHA1(encoding.GetBytes(_secretAccessKey)))
			{
				return Convert.ToBase64String(hashAlgorithm.ComputeHash(encoding.GetBytes(input)));
			}
		}
	}
}
