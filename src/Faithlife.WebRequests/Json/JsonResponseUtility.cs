using System;
using System.Text.RegularExpressions;

namespace Faithlife.WebRequests.Json
{
	internal static class JsonResponseUtility
	{
		/// <summary>
		/// Returns true if the content type is a JSON content type.
		/// </summary>
		/// <param name="contentType">The response.</param>
		/// <returns>True if the content type is "application/json" or "application/schema+json".</returns>
		public static bool IsJsonContentType(string contentType)
		{
			if (contentType == null)
				throw new ArgumentNullException(nameof(contentType));

			// Length comparison is to short-circuit more-expensive regex check. Could potentially call Trim() and check StartsWith "application" as another performance optimization.
			return contentType.Length >= 16 && s_jsonContentTypeRegex.IsMatch(contentType);
		}

		private static readonly Regex s_jsonContentTypeRegex = new Regex(@"^\s*application\/([^\s;]+\+)?json");
	}
}
