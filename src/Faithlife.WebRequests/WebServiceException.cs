using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Faithlife.Utility;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// A web service exception.
	/// </summary>
	public class WebServiceException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceException"/> class.
		/// </summary>
		public WebServiceException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		public WebServiceException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		public WebServiceException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="requestMethod">The request method.</param>
		/// <param name="requestUri">The request URI.</param>
		/// <param name="responseStatusCode">The response status code.</param>
		/// <param name="responseHeaders">The response headers.</param>
		/// <param name="responseContentType">The response content type.</param>
		/// <param name="responseContentLength">The response content length.</param>
		/// <param name="responseContentPreview">A preview of the response content (as a string, possibly abbreviated).</param>
		/// <param name="innerException">The inner exception.</param>
		public WebServiceException(string message = null, string requestMethod = null, Uri requestUri = null, HttpStatusCode? responseStatusCode = null, HttpHeaders responseHeaders = null, string responseContentType = null, long? responseContentLength = null, string responseContentPreview = null, Exception innerException = null)
			: base(message, innerException)
		{
			m_requestMethod = requestMethod;
			m_requestUri = requestUri;
			m_responseStatusCode = responseStatusCode;
			m_responseHeaders = responseHeaders;
			m_responseContentType = responseContentType;
			m_responseContentLength = responseContentLength;
			m_responseContentPreview = responseContentPreview;
		}

		/// <summary>
		/// Gets the request method.
		/// </summary>]
		/// <value>The request method.</value>
		public string RequestMethod
		{
			get { return m_requestMethod; }
		}

		/// <summary>
		/// Gets the request URI.
		/// </summary>
		/// <value>The request URI.</value>
		public Uri RequestUri
		{
			get { return m_requestUri; }
		}

		/// <summary>
		/// Gets the response status code.
		/// </summary>
		/// <value>The response status code.</value>
		public HttpStatusCode? ResponseStatusCode
		{
			get { return m_responseStatusCode; }
		}

		/// <summary>
		/// Gets the response headers.
		/// </summary>
		/// <value>The response headers.</value>
		public HttpHeaders ResponseHeaders
		{
			get { return m_responseHeaders; }
		}

		/// <summary>
		/// Gets the response content type.
		/// </summary>
		/// <value>The response content type.</value>
		public string ResponseContentType
		{
			get { return m_responseContentType; }
		}

		/// <summary>
		/// Gets the response content length.
		/// </summary>
		/// <value>The response content length.</value>
		public long? ResponseContentLength
		{
			get { return m_responseContentLength; }
		}

		/// <summary>
		/// Gets the response content preview.
		/// </summary>
		/// <value>The response content preview (as a string, possibly abbreviated).</value>
		public string ResponseContentPreview
		{
			get { return m_responseContentPreview; }
		}

		/// <summary>
		/// Gets the response.
		/// </summary>
		/// <value>The response.</value>
		public WebServiceResponse Response { get; internal set; }

		/// <summary>
		/// Gets a message that describes the current exception.
		/// </summary>
		/// <value>The message that describes the current exception.</value>
		public override string Message
		{
			get
			{
				StringBuilder message = new StringBuilder(base.Message);

				if (message.Length == 0)
					message.Append("Web service error.");

				if (m_requestUri != null)
					message.Append(" Request: {0} {1}".FormatInvariant(m_requestMethod ?? "GET", m_requestUri.AbsoluteUri));

				if (m_responseStatusCode != null)
				{
					message.Append(" (status ").Append(m_responseStatusCode.Value);

					if (m_responseContentType != null)
						message.Append(", content type '").Append(m_responseContentType).Append("'");

					if (m_responseContentLength != null)
						message.Append(", content length ").AppendInvariant(m_responseContentLength.Value);

					message.Append(")");
				}

				return message.ToString();
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Don't want exceptions thrown from ToString.")]
		public override string ToString()
		{
			if (m_responseContentPreview != null)
			{
				StringBuilder result = new StringBuilder(base.ToString());
				result.AppendLine().Append("content: ").Append(m_responseContentPreview);
				return result.ToString();
			}
			else if (Response != null && Response.Content != null)
			{
				StringBuilder result = new StringBuilder(base.ToString());

				result.AppendLine().Append("content: ");

				try
				{
					string responseContentAsText = Response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

					const int maxResponseContentAsTextLength = 1000;
					if (responseContentAsText.Length > maxResponseContentAsTextLength)
						result.Append(responseContentAsText.Substring(0, maxResponseContentAsTextLength)).Append("...");
					else
						result.Append(responseContentAsText);
				}
				catch (Exception x)
				{
					result.AppendLine(x.Message);
				}

				return result.ToString();
			}
			else
			{
				return base.ToString();
			}
		}

		readonly string m_requestMethod;
		readonly Uri m_requestUri;
		readonly HttpStatusCode? m_responseStatusCode;
		readonly string m_responseContentType;
		readonly long? m_responseContentLength;
		readonly HttpHeaders m_responseHeaders;
		readonly string m_responseContentPreview;
	}
}
