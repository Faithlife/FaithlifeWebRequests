using System;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// Provides helper methods for working with <see cref="WebServiceResponse"/>.
	/// </summary>
	public static class WebServiceResponseUtility
	{
		/// <summary>
		/// Creates an exception for the specified response.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		/// <returns>A new exception.</returns>
		public static WebServiceException CreateWebServiceException(WebServiceResponse response, string? message = null, Exception? innerException = null)
		{
			WebServiceException exception = new WebServiceException(
				message: message,
				requestMethod: response.RequestMethod,
				requestUri: response.RequestUri,
				responseStatusCode: response.StatusCode,
				responseHeaders: response.Headers,
				responseContentType: response.Content?.Headers.ContentType?.ToString(),
				responseContentLength: response.Content?.Headers.ContentLength,
				innerException: innerException);
			exception.Response = response;
			return exception;
		}
	}
}
