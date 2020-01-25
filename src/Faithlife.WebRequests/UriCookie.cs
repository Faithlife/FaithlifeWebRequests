using System;
using System.Net;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// Stores a <see cref="Uri"/> and an associated Cookie.
	/// </summary>
	public sealed class UriCookie
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UriCookieHeader"/> class.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <param name="cookie">The cookie.</param>
		public UriCookie(Uri uri, Cookie cookie)
		{
			Uri = uri;
			Cookie = cookie;
		}

		/// <summary>
		/// Gets the URI.
		/// </summary>
		/// <value>The URI.</value>
		public Uri Uri { get; }

		/// <summary>
		/// Gets the cookie.
		/// </summary>
		/// <value>The cookie.</value>
		public Cookie Cookie { get; }
	}
}
