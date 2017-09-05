using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Faithlife.Utility;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// Manages a collection of cookies for a set of URIs.
	/// </summary>
	/// <remarks>This class is thread-safe.</remarks>
	public sealed class CookieManager
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CookieManager"/> class.
		/// </summary>
		public CookieManager()
		{
			m_objLock = new object();
			m_cookieContainer = new CookieContainer();
			m_setUris = new HashSet<Uri>();
		}

		/// <summary>
		/// Gets the HTTP Cookie header for the specified URI.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <returns>An HTTP cookie header, with strings representing Cookie instances delimited by semicolons.</returns>
		public string GetCookieHeader(Uri uri)
		{
			lock (m_objLock)
				return m_cookieContainer.GetCookieHeader(uri);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CookieManager"/> class.
		/// </summary>
		/// <param name="seqCookies">The seq cookies.</param>
		public CookieManager(IEnumerable<UriCookieHeader> seqCookies)
			: this()
		{
			foreach (UriCookieHeader cookie in seqCookies)
				SetCookies(cookie.Uri, cookie.CookieHeader);
		}

		/// <summary>
		/// Returns a reference to the <see cref="CookieContainer"/> for this instance. 
		/// </summary>
		public CookieContainer CookieContainer
		{
			get { return m_cookieContainer; }
		}

		/// <summary>
		/// Adds <see cref="UriCookieHeader"/> instances for one or more cookies from an HTTP cookie header to the <see cref="CookieManager"/> for a specific URI.
		/// </summary>
		/// <param name="uri">The URI.</param>
		/// <param name="cookieHeader">The cookie header.</param>
		public void SetCookies(Uri uri, string cookieHeader)
		{
			lock (m_objLock)
			{
				// track this URI (we need it to retrieve the cookies later)
				m_setUris.Add(uri);

				// store the cookies
				m_cookieContainer.SetCookies(uri, cookieHeader);
			}

			CookiesChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Gets the collection of cookies contained in this <see cref="CookieManager"/> instance.
		/// </summary>
		public IEnumerable<UriCookieHeader> GetCookies()
		{
			// return pairs of URIs and Set-Cookie header value for each URI registered with the CookieManager
			lock (m_objLock)
				return m_setUris.Select(uri => new UriCookieHeader(uri, m_cookieContainer.GetCookies(uri).Cast<Cookie>().Select(c => c.ToSetCookieHeaderValue()).Join(","))).ToList().AsReadOnly();
		}

		/// <summary>
		/// Gets the collection of cookies contained in this <see cref="CookieManager"/> instance.
		/// </summary>
		public IEnumerable<UriCookie> GetUriCookies()
		{
			// return pairs of URIs and Cookies for each URI registered with the CookieManager
			lock (m_objLock)
				return m_setUris.SelectMany(uri => m_cookieContainer.GetCookies(uri).Cast<Cookie>().Select(cookie => new UriCookie(uri, cookie))).ToList().AsReadOnly();
		}

		/// <summary>
		/// Expires the cookies, and raising the <see cref="CookiesChanged"/> event if there were any cookies (even potentially already expired ones).
		/// </summary>
		public void ExpireCookies()
		{
			DateTime expires = DateTime.Now.AddDays(-1);
			bool empty;
			lock (m_objLock)
			{
				empty = m_setUris.Count == 0;
				if (!empty)
					m_setUris.ForEach(uri => m_cookieContainer.GetCookies(uri).Cast<Cookie>().ForEach(x => x.Expires = expires));
			}

			if (!empty)
				CookiesChanged?.Invoke(this, EventArgs.Empty);
		}

		/// <summary>
		/// Raised when cookies are changed.
		/// </summary>
		public event EventHandler CookiesChanged;

		readonly object m_objLock;
		readonly CookieContainer m_cookieContainer;
		readonly HashSet<Uri> m_setUris;
	}
}
