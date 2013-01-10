using System.Text.RegularExpressions;

namespace Nupload
{
	public class GoogleCloudStorageObjectConfiguration
	{
		private readonly string _key;
		private readonly GoogleCloudStorageCannedAcl _cannedAcl;
		private readonly int _maxFileSize;

		public GoogleCloudStorageObjectConfiguration(string key, GoogleCloudStorageCannedAcl cannedAcl, int maxFileSize)
		{
			_key = key;
			_cannedAcl = cannedAcl;
			_maxFileSize = maxFileSize;
		}

		public string Key
		{
			get
			{
				return _key;
			}
		}

		public string Acl
		{
			get
			{
				return Regex.Replace(_cannedAcl.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower();
			}
		}

		public long MaxFileSize
		{
			get
			{
				return _maxFileSize;
			}
		}
	}
}
