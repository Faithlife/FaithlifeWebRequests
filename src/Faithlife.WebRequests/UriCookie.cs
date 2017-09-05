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
			m_uri = uri;
			m_cookie = cookie;
		}

		/// <summary>
		/// Gets the URI.
		/// </summary>
		/// <value>The URI.</value>
		public Uri Uri
		{
			get { return m_uri; }
		}

		/// <summary>
		/// Gets the cookie.
		/// </summary>
		/// <value>The cookie.</value>
		public Cookie Cookie
		{
			get { return m_cookie; }
		}

		readonly Uri m_uri;
		readonly Cookie m_cookie;
	}
}
