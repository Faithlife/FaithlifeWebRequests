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
			m_requestUri = request.RequestUri;
			m_requestMethod = request.Method;

			m_statusCode = statusCode;
			m_headers = headers;
			m_content = content;
		}

		/// <summary>
		/// The method used by the request.
		/// </summary>
		public string RequestMethod
		{
			get { return m_requestMethod; }
		}

		/// <summary>
		/// The request URI.
		/// </summary>
		public Uri RequestUri
		{
			get { return m_requestUri; }
		}

		/// <summary>
		/// Gets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public HttpStatusCode StatusCode
		{
			get { return m_statusCode; }
		}

		/// <summary>
		/// Gets the headers.
		/// </summary>
		/// <value>The headers.</value>
		public HttpHeaders Headers
		{
			get { return m_headers; }
		}

		/// <summary>
		/// Gets the content.
		/// </summary>
		/// <value>The content.</value>
		public HttpContent? Content
		{
			get { return m_content; }
		}

		readonly string m_requestMethod;
		readonly Uri m_requestUri;
		readonly HttpStatusCode m_statusCode;
		readonly HttpHeaders m_headers;
		readonly HttpContent? m_content;
	}
}
