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
		/// Creates an exception for the response.
		/// </summary>
		/// <returns>A new exception for the response.</returns>
		public WebServiceException CreateException(string? message = null, Exception? innerException = null)
		{
			return new WebServiceException(
				message: message,
				requestMethod: m_requestMethod,
				requestUri: m_requestUri,
				responseStatusCode: m_responseStatusCode,
				responseHeaders: m_responseHeaders,
				responseContentType: m_responseContentType,
				responseContentLength: m_responseContentLength,
				responseContentPreview: m_responseContentPreview,
				innerException: innerException);
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
			m_requestMethod = info.WebResponse!.RequestMessage.Method.Method;
			m_requestUri = info.WebResponse.RequestMessage.RequestUri;
			m_responseStatusCode = info.WebResponse.StatusCode;
			m_responseHeaders = info.WebResponse.Headers;
			m_responseContentType = info.WebResponse.Content?.Headers?.ContentType?.ToString();
			m_responseContentLength = info.WebResponse.Content?.Headers?.ContentLength;

			if (!info.IsContentRead)
			{
				m_responseContentPreview = await HttpResponseMessageUtility.ReadContentPreviewAsync(info.WebResponse).ConfigureAwait(false);
				info.MarkContentAsRead();
			}
		}

		internal Task OnResponseHandledAsync(WebServiceResponseHandlerInfo info)
		{
			return OnResponseHandledCoreAsync(info);
		}

		private string? m_requestMethod;
		private Uri? m_requestUri;
		private HttpStatusCode? m_responseStatusCode;
		private HttpHeaders? m_responseHeaders;
		private string? m_responseContentType;
		private long? m_responseContentLength;
		private string? m_responseContentPreview;
	}
}
