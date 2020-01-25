using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// A web service response.
	/// </summary>
	public class WebServiceResponse
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceResponse"/> class.
		/// </summary>
		/// <param name="request">The <see cref="WebServiceRequest"/>.</param>
		/// <param name="statusCode">The status code.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="content">The content.</param>
		public WebServiceResponse(WebServiceRequest request, HttpStatusCode statusCode, HttpHeaders headers, HttpContent? content)
		{
			// store just the values we need (not the whole request object), so that we don't keep large objects (request.Content) alive longer than necessary
			RequestUri = request.RequestUri;
			RequestMethod = request.Method;

			StatusCode = statusCode;
			Headers = headers;
			Content = content;
		}

		/// <summary>
		/// The method used by the request.
		/// </summary>
		public string RequestMethod { get; }

		/// <summary>
		/// The request URI.
		/// </summary>
		public Uri RequestUri { get; }

		/// <summary>
		/// Gets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public HttpStatusCode StatusCode { get; }

		/// <summary>
		/// Gets the headers.
		/// </summary>
		/// <value>The headers.</value>
		public HttpHeaders Headers { get; }

		/// <summary>
		/// Gets the content.
		/// </summary>
		/// <value>The content.</value>
		public HttpContent? Content { get; }
	}
}
