using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Nupload
{
	public class GoogleCloudStorageUploadConfiguration : IUploadConfiguration
	{
		private const FormMethod Method = FormMethod.Post;

		private readonly Encoding _encoding = Encoding.UTF8;
		private readonly GoogleCredentials _credentials;
		private readonly string _bucket;
		private readonly GoogleCloudStorageObjectConfiguration _objectConfiguration;
		private readonly TimeSpan _policyExpiration;

		public GoogleCloudStorageUploadConfiguration(GoogleCredentials credentials, string bucket, GoogleCloudStorageObjectConfiguration objectConfiguration)
			: this(credentials, bucket, objectConfiguration, TimeSpan.FromMinutes(20))
		{
		}

		public GoogleCloudStorageUploadConfiguration(GoogleCredentials credentials, string bucket, GoogleCloudStorageObjectConfiguration objectConfiguration, TimeSpan policyExpiration)
		{
			_credentials = credentials;
			_bucket = bucket;
			_objectConfiguration = objectConfiguration;
			_policyExpiration = policyExpiration;
		}

		public IDictionary<string, string> FormAttributes
		{
			get
			{
				var actionUrl = new Uri(string.Format("https://{0}.storage.googleapis.com/", _bucket));
				return new Dictionary<string, string>
					{
						{ "accept-charset", _encoding.HeaderName },
						{ "action", actionUrl.OriginalString },
						{ "method", HtmlHelper.GetFormMethodString(Method) },
					};
			}
		}

		public IDictionary<string, string> FormFields
		{
			get
			{
				return new Dictionary<string, string>
					{
						{ "key", _objectConfiguration.Key },
						{ "acl", _objectConfiguration.Acl },
						{ "policy", Policy },
						{ "signature", _credentials.GetSignature(Policy) },
						{ "content-type", "application/octet-stream" },
						{ "GoogleAccessId", _credentials.AccessId } ,
						{ "success_action_status", "201"}
					};
			}
		}

		private string Policy
		{
			get
			{
				return Convert.ToBase64String(_encoding.GetBytes(JsonConvert.SerializeObject(PolicyData)));
			}
		}

		private object PolicyData
		{
			get
			{
				return new
				{
					expiration = DateTime.UtcNow.Add(_policyExpiration).ToString("yyyy-MM-ddTHH:mm:ssZ"),
					conditions = new object[]
						{
							new string[] { "starts-with", "$key", "" },
							new string[] { "starts-with", "$content-type", "" },
							new string[] { "starts-with", "$acl", _objectConfiguration.Acl },
							new object[] { "content-length-range", 0, _objectConfiguration.MaxFileSize },
							new { success_action_status = "201" },
							new { bucket = _bucket },
							new { acl = _objectConfiguration.Acl }
						}
				};
			}
		}
	}
}
