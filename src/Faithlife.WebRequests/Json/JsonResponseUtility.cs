using System;
using System.Text.RegularExpressions;

namespace Faithlife.WebRequests.Json
{
	internal static class JsonResponseUtility
	{
		/// <summary>
		/// Returns true if the content type is a JSON content type.
		/// </summary>
		/// <param name="contentType">The Content-Type http header value.</param>
		/// <returns>True if the content type is "application/json" or "application/schema+json".</returns>
		public static bool IsJsonContentType(string contentType)
		{
			if (contentType == null)
				throw new ArgumentNullException(nameof(contentType));

			return s_jsonContentTypeRegex.IsMatch(contentType);
		}

		private static readonly Regex s_jsonContentTypeRegex = new Regex(@"^\s*application\/([^\s;]+\+)?json");
	}
}
