using System.Collections.Generic;

namespace Nupload
{
	public interface IUploadConfiguration
	{
		IDictionary<string, string> FormAttributes { get; }
		IDictionary<string, string> FormFields { get; }
	}
}
