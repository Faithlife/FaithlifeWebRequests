using System;
using System.Globalization;
using System.Net;
using System.Text;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// Provides helper methods for working with <see cref="Cookie"/>.
	/// </summary>
	public static class CookieUtility
	{
		/// <summary>
		/// Returns a <see cref="String"/> representing the specified cookie, in the format prescribed by
		/// RFC 2965 for the Set-Cookie HTTP header value.
		/// </summary>
		/// <param name="cookie">The cookie.</param>
		/// <returns>A string in the Set-Cookie HTTP header format.</returns>
		public static string ToSetCookieHeaderValue(this Cookie cookie)
		{
			StringBuilder strHeader = new StringBuilder(cookie.ToString());
			if (!string.IsNullOrEmpty(cookie.Domain))
				strHeader.Append("; domain=").Append(cookie.Domain);
			if (!string.IsNullOrEmpty(cookie.Path))
				strHeader.Append($"; path={cookie.Path}");
			if (cookie.Expires > DateTime.MinValue)
				strHeader.Append("; expires=" + cookie.Expires.ToUniversalTime().ToString(@"ddd, dd-MMM-yyyy HH:mm:ss G\MT", CultureInfo.InvariantCulture));
			if (cookie.HttpOnly)
				strHeader.Append("; HttpOnly");
			if (cookie.Secure)
				strHeader.Append("; secure");
			return strHeader.ToString();
		}
	}
}
