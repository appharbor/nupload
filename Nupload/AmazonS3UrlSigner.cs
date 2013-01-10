using System;
using System.Web;

namespace Nupload
{
	public class AmazonS3UrlSigner
	{
		private readonly AmazonCredentials _credentials;

		public AmazonS3UrlSigner(AmazonCredentials credentials)
		{
			_credentials = credentials;
		}

		public Uri GetSignedUrl(Uri url, TimeSpan signatureExpiration, string method = "GET")
		{
			var expires = DateTime.UtcNow.Add(signatureExpiration).GetSecondsSinceUnixEpoch();
			var stringToSign = string.Format("{0}\n\n\n{1}\n/{2}{3}",
				method, expires, url.Host.Split('.')[0], url.LocalPath);

			var urlEncodedSignature = HttpUtility.UrlEncode(_credentials.GetSignature(stringToSign));

			var authenticatedUrl = string.Format("{0}?AWSAccessKeyId={1}&Signature={2}&Expires={3}",
				url.OriginalString, _credentials.AccessKeyId, urlEncodedSignature, expires);

			return new Uri(authenticatedUrl);
		}
	}
}
