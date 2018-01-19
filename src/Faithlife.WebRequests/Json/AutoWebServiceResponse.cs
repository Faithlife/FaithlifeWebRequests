using System;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Faithlife.WebRequests.Json
{
	/// <summary>
	/// Base class for AutoWebServiceRequest responses.
	/// </summary>
	public abstract class AutoWebServiceResponse
	{
		/// <summary>
		/// Gets the HTTP headers on the response.
		/// </summary>
		public HttpResponseHeaders Headers { get; private set; }

		/// <summary>
		/// Gets whether or not the response's content was able to be mapped to a strongly typed property corresponding to the HTTP status code.
		/// </summary>
		public bool IsStatusCodeHandled { get; private set; }

		/// <summary>
		/// Gets the HTTP status code on the response.
		/// </summary>
		public HttpStatusCode StatusCode => m_responseStatusCode ?? throw new InvalidOperationException("Response data has not yet been populated.");

		/// <summary>
		/// Creates an exception for the response.
		/// </summary>
		/// <returns>A new exception for the response.</returns>
		public WebServiceException CreateException(string message = null, Exception innerException = null)
		{
			return new WebServiceException(
				message: message,
				requestMethod: m_requestMethod,
				requestUri: m_requestUri,
				responseStatusCode: m_responseStatusCode,
				responseHeaders: Headers,
				responseContentType: m_responseContentType,
				responseContentLength: m_responseContentLength,
				responseContentPreview: m_responseContentPreview,
				innerException: innerException);
		}

		/// <summary>
		/// Throws an exception if the response's content was unable to be mapped to a strongly typed property corresponding to the HTTP status code.
		/// </summary>
		public void ThrowIfStatusCodeNotHandled(string message = null)
		{
			if (!IsStatusCodeHandled)
				throw CreateException(message ?? "Status code not handled.");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AutoWebServiceResponse"/> class.
		/// </summary>
		protected AutoWebServiceResponse()
		{
		}

		/// <summary>
		/// Called when the response has been handled, i.e. immediately before it is returned.
		/// </summary>
		/// <param name="info">The web service response handler information.</param>
		protected virtual async Task OnResponseHandledCoreAsync(WebServiceResponseHandlerInfo info)
		{
			m_requestMethod = info.WebResponse.RequestMessage.Method.Method;
			m_requestUri = info.WebResponse.RequestMessage.RequestUri;
			m_responseStatusCode = info.WebResponse.StatusCode;
			Headers = info.WebResponse.Headers;
			m_responseContentType = info.WebResponse.Content?.Headers?.ContentType?.ToString();
			m_responseContentLength = info.WebResponse.Content?.Headers?.ContentLength;

			if (!info.IsContentRead)
			{
				m_responseContentPreview = await HttpResponseMessageUtility.ReadContentPreviewAsync(info.WebResponse).ConfigureAwait(false);
				info.MarkContentAsRead();
			}
		}

		internal Task OnResponseHandledAsync(WebServiceResponseHandlerInfo info, bool isStatusCodeHandled)
		{
			IsStatusCodeHandled = isStatusCodeHandled;
			return OnResponseHandledCoreAsync(info);
		}

		string m_requestMethod;
		Uri m_requestUri;
		HttpStatusCode? m_responseStatusCode;
		string m_responseContentType;
		long? m_responseContentLength;
		string m_responseContentPreview;
	}
}
