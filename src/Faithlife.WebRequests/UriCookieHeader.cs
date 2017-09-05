using System;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// Stores a <see cref="Uri"/> and an associated Set-Cookie HTTP header.
	/// </summary>
	public sealed class UriCookieHeader
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UriCookieHeader"/> class.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <param name="cookieHeader">The cookie header.</param>
		public UriCookieHeader(Uri uri, string cookieHeader)
		{
			m_uri = uri;
			m_cookieHeader = cookieHeader;
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
		/// Gets the cookie header.
		/// </summary>
		/// <value>The cookie header.</value>
		public string CookieHeader
		{
			get { return m_cookieHeader; }
		}

		readonly Uri m_uri;
		readonly string m_cookieHeader;
	}
}
