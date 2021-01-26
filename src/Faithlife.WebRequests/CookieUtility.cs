using System;
using System.Globalization;
using System.Net;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// Provides helper methods for working with <see cref="Cookie"/>.
	/// </summary>
	public static class CookieUtility
	{
		/// <summary>
		/// Returns a <see cref="string"/> representing the specified cookie, in the format prescribed by
		/// RFC 2965 for the Set-Cookie HTTP header value.
		/// </summary>
		/// <param name="cookie">The cookie.</param>
		/// <returns>A string in the Set-Cookie HTTP header format.</returns>
		public static string ToSetCookieHeaderValue(this Cookie cookie)
		{
			string strHeader = cookie.ToString();
			if (!string.IsNullOrEmpty(cookie.Domain))
				strHeader += "; domain=" + cookie.Domain;
			if (!string.IsNullOrEmpty(cookie.Path))
				strHeader += "; path=" + cookie.Path;
			if (cookie.Expires > DateTime.MinValue)
				strHeader += "; expires=" + cookie.Expires.ToUniversalTime().ToString(@"ddd, dd-MMM-yyyy HH:mm:ss G\MT", CultureInfo.InvariantCulture);
			if (cookie.HttpOnly)
				strHeader += "; HttpOnly";
			if (cookie.Secure)
				strHeader += "; secure";
			return strHeader;
		}
	}
}
