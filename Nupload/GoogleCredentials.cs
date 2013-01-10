using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Nupload
{
	public class GoogleCredentials
	{
		private readonly string _accessId;
		private readonly X509Certificate2 _certificate;

		public GoogleCredentials(string accessId, X509Certificate2 certificate)
		{
			_accessId = accessId;
			_certificate = certificate;
		}

		public GoogleCredentials(string accessId, Stream certificateStream, string certificatePassword)
		{
			_accessId = accessId;
			using (var memoryStream = new MemoryStream())
			{
				certificateStream.CopyTo(memoryStream);
				_certificate = new X509Certificate2(memoryStream.ToArray(), certificatePassword, X509KeyStorageFlags.Exportable);
			}
		}

		public string AccessId
		{
			get
			{
				return _accessId;
			}
		}

		public string GetSignature(string input)
		{
			using (var cryptoServiceProvider = GetRSACryptoServiceProvider())
			{
				using (var hashAlgorithm = new SHA256Managed())
				{
					var messageDigest = cryptoServiceProvider.SignData(Encoding.UTF8.GetBytes(input), hashAlgorithm);
					return Convert.ToBase64String(messageDigest);
				}
			}
		}

		private RSACryptoServiceProvider GetRSACryptoServiceProvider()
		{
			var provider = new RSACryptoServiceProvider();

			//Hack to enable the newly initialized provider to sign with the SHA-256 algorithm
			using (var tempProvider = _certificate.PrivateKey as RSACryptoServiceProvider)
			{
				provider.ImportParameters(tempProvider.ExportParameters(true));
			}

			return provider;
		}
	}
}
