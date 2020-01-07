using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Faithlife.Utility;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// Provides utility methods for working with <see cref="HttpResponseMessage"/>.
	/// </summary>
	public static class HttpResponseMessageUtility
	{
		/// <summary>
		/// Creates an exception for the specified response.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <param name="message">The message.</param>
		/// <param name="contentPreview">The content preview.</param>
		/// <param name="innerException">The inner exception.</param>
		/// <returns>A new exception.</returns>
		public static WebServiceException CreateWebServiceException(HttpResponseMessage response, string message = null, Exception innerException = null, string contentPreview = null)
		{
			return new WebServiceException(
				message: message,
				requestMethod: response.RequestMessage.Method.Method,
				requestUri: response.RequestMessage.RequestUri,
				responseStatusCode: response.StatusCode,
				responseHeaders: response.Headers,
				responseContentType: response.Content?.Headers.ContentType?.ToString(),
				responseContentLength: response.Content?.Headers.ContentLength,
				responseContentPreview: contentPreview,
				innerException: innerException);
		}

		/// <summary>
		/// Creates an exception for the specified response, reading a content preview from the response.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <param name="message">The message.</param>
		/// <param name="innerException">The inner exception.</param>
		/// <returns>A new exception.</returns>
		/// <remarks>Do not call this method if the response stream has already been retrieved.</remarks>
		public static async Task<WebServiceException> CreateWebServiceExceptionWithContentPreviewAsync(HttpResponseMessage response, string message = null, Exception innerException = null)
		{
			var contentPreview = await ReadContentPreviewAsync(response).ConfigureAwait(false);
			return CreateWebServiceException(response, message, innerException, contentPreview);
		}

		/// <summary>
		/// Reads a content preview from the response.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <returns>The content preview.</returns>
		public static async Task<string> ReadContentPreviewAsync(HttpResponseMessage response)
		{
			try
			{
				Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
				using (WrappingStream wrappingStream = new WrappingStream(stream, Ownership.None))
				using (StreamReader reader = new StreamReader(stream))
				{
					char[] buffer = new char[c_contentPreviewCharacterCount];
					int readCount = await reader.ReadBlockAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
					if (readCount == buffer.Length)
						buffer[readCount - 1] = '\u2026';
					return new string(buffer, 0, readCount);
				}
			}
			catch (Exception)
			{
				// ignore failure to read a content preview
				return null;
			}
		}

		const int c_contentPreviewCharacterCount = 2000;
	}
}
