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
		public Uri Uri { get; }

		/// <summary>
		/// The request HTTP method.
		/// </summary>
		public string? Method { get; }

		internal WebServiceRequestInfo(Uri uri, string method)
		{
			if (uri is null)
				throw new ArgumentNullException(nameof(uri));
			if (method is null)
				throw new ArgumentNullException(nameof(method));
			if (method.Length == 0)
				throw new ArgumentException("'method' must not be empty", "method");

			Uri = uri;
			Method = method;
		}

		internal WebServiceRequestInfo(HttpRequestMessage request)
		{
			if (request is null)
				throw new ArgumentNullException(nameof(request));

			Uri = request.RequestUri;
			Method = request.Method?.Method;
		}
	}
}
