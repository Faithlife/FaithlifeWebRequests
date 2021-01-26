using System.Net;
using System.Net.Http.Headers;
using Faithlife.Utility;

namespace Faithlife.WebRequests
{
	internal static class HttpHeadersUtility
	{
		public static void AddWebHeaders(this HttpHeaders self, WebHeaderCollection headers)
		{
			foreach (var headerName in headers.AllKeys)
				self.Add(headerName, headers[headerName]);
		}

		public static WebHeaderCollection ToWebHeaderCollection(this HttpHeaders self)
		{
			var collection = new WebHeaderCollection();
			foreach (var pair in self)
				collection.Add(pair.Key, pair.Value.Join("; "));
			return collection;
		}
	}
}
