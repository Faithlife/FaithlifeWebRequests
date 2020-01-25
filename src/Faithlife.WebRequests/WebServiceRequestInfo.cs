using System;
using System.Net;
using System.Net.Http;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// A web service request.
	/// </summary>
	public sealed class WebServiceRequestInfo
	{
		/// <summary>
		/// The request URI.
		/// </summary>
		public Uri Uri
		{
			get { return m_uri; }
		}

		/// <summary>
		/// The request HTTP method.
		/// </summary>
		public string? Method
		{
			get { return m_method; }
		}

		internal WebServiceRequestInfo(Uri uri, string method)
		{
			if (uri is null)
				throw new ArgumentNullException(nameof(uri));
			if (method is null)
				throw new ArgumentNullException(nameof(method));
			if (method.Length == 0)
				throw new ArgumentException("'method' must not be empty", "method");

			m_uri = uri;
			m_method = method;
		}

		internal WebServiceRequestInfo(HttpRequestMessage request)
		{
			if (request is null)
				throw new ArgumentNullException(nameof(request));

			m_uri = request.RequestUri;
			m_method = request.Method?.Method;
		}

		readonly Uri m_uri;
		readonly string? m_method;
	}
}
