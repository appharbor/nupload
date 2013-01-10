using System;
using System.Web;

namespace Nupload
{
	public class GoogleCloudStorageUrlSigner
	{
		private readonly GoogleCredentials _credentials;

		public GoogleCloudStorageUrlSigner(GoogleCredentials credentials)
		{
			_credentials = credentials;
		}

		public Uri GetSignedUrl(Uri url, TimeSpan signatureExpiration, string method = "GET")
		{
			var expires = DateTime.UtcNow.Add(signatureExpiration).GetSecondsSinceUnixEpoch();
			var stringToSign = string.Format("{0}\n\n\n{1}\n{2}", method, expires, url.LocalPath);
			var urlEncodedSignature = HttpUtility.UrlEncode(_credentials.GetSignature(stringToSign));

			var authenticatedUrl = string.Format("{0}?GoogleAccessId={1}&Signature={2}&Expires={3}",
				url.OriginalString, _credentials.AccessId, urlEncodedSignature, expires);

			return new Uri(authenticatedUrl);
		}
	}
}
