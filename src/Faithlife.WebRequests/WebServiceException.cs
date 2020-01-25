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
		public WebServiceException(string? message = null, string? requestMethod = null, Uri? requestUri = null, HttpStatusCode? responseStatusCode = null, HttpHeaders? responseHeaders = null, string? responseContentType = null, long? responseContentLength = null, string? responseContentPreview = null, Exception? innerException = null)
			: base(message, innerException)
		{
			RequestMethod = requestMethod;
			RequestUri = requestUri;
			ResponseStatusCode = responseStatusCode;
			ResponseHeaders = responseHeaders;
			ResponseContentType = responseContentType;
			ResponseContentLength = responseContentLength;
			ResponseContentPreview = responseContentPreview;
		}

		/// <summary>
		/// Gets the request method.
		/// </summary>]
		/// <value>The request method.</value>
		public string? RequestMethod { get; }

		/// <summary>
		/// Gets the request URI.
		/// </summary>
		/// <value>The request URI.</value>
		public Uri? RequestUri { get; }

		/// <summary>
		/// Gets the response status code.
		/// </summary>
		/// <value>The response status code.</value>
		public HttpStatusCode? ResponseStatusCode { get; }

		/// <summary>
		/// Gets the response headers.
		/// </summary>
		/// <value>The response headers.</value>
		public HttpHeaders? ResponseHeaders { get; }

		/// <summary>
		/// Gets the response content type.
		/// </summary>
		/// <value>The response content type.</value>
		public string? ResponseContentType { get; }

		/// <summary>
		/// Gets the response content length.
		/// </summary>
		/// <value>The response content length.</value>
		public long? ResponseContentLength { get; }

		/// <summary>
		/// Gets the response content preview.
		/// </summary>
		/// <value>The response content preview (as a string, possibly abbreviated).</value>
		public string? ResponseContentPreview { get; }

		/// <summary>
		/// Gets the response.
		/// </summary>
		/// <value>The response.</value>
		public WebServiceResponse? Response { get; internal set; }

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

				if (RequestUri is object)
					message.Append(" Request: {0} {1}".FormatInvariant(RequestMethod ?? "GET", RequestUri.AbsoluteUri));

				if (ResponseStatusCode is object)
				{
					message.Append(" (status ").Append(ResponseStatusCode.Value);

					if (ResponseContentType is object)
						message.Append(", content type '").Append(ResponseContentType).Append("'");

					if (ResponseContentLength is object)
						message.Append(", content length ").AppendInvariant(ResponseContentLength.Value);

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
			if (ResponseContentPreview is object)
			{
				StringBuilder result = new StringBuilder(base.ToString());
				result.AppendLine().Append("content: ").Append(ResponseContentPreview);
				return result.ToString();
			}
			else if (Response?.Content is object)
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
	}
}
