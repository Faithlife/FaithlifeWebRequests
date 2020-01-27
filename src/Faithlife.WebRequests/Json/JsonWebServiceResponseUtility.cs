using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using Faithlife.Utility;
using Faithlife.Json;
using Newtonsoft.Json;

namespace Faithlife.WebRequests.Json
{
	/// <summary>
	/// Utility methods for JSON and WebServiceResponse.
	/// </summary>
	/// <remarks>The JSON in the response content must be represented using UTF-8.</remarks>
	public static class JsonWebServiceResponseUtility
	{
		/// <summary>
		/// Returns true if the response content uses the JSON content type.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <returns>True if the response content uses the JSON content type ("application/json") and the content is not empty.</returns>
		public static bool HasJson(this WebServiceResponse response)
		{
			bool hasJson = response.Content?.Headers.ContentLength > 0;
			var contentType = response.Content?.Headers.ContentType?.ToString();
			hasJson &= contentType is object && contentType.Length >= JsonWebServiceContent.JsonContentType.Length &&
				contentType.Trim().StartsWith(JsonWebServiceContent.JsonContentType, StringComparison.Ordinal);

			return hasJson;
		}

		/// <summary>
		/// Gets the JSON.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <returns>The unverified JSON.</returns>
		/// <exception cref="WebServiceException">The response content does not use the JSON content type or the content is empty.</exception>
		public static Task<string> GetJsonAsync(this WebServiceResponse response)
		{
			if (!response.HasJson())
				throw WebServiceResponseUtility.CreateWebServiceException(response, "The response does not have JSON content.");

			return response.Content!.ReadAsStringAsync();
		}

		/// <summary>
		/// Parses the JSON into an object of the specified type.
		/// </summary>
		/// <typeparam name="T">The specified type.</typeparam>
		/// <param name="response">The response.</param>
		/// <returns>An object of the specified type.</returns>
		/// <remarks>Use JToken as the type to parse arbitrary JSON.</remarks>
		/// <exception cref="WebServiceException">The response content does not use the JSON content type, or the content is empty,
		/// or the text is not valid JSON, or the JSON cannot be deserialized into the specified type.</exception>
		[return: MaybeNull]
		public static Task<T> GetJsonAsAsync<T>(this WebServiceResponse response) => response.GetJsonAsAsync<T>(null)!;

		/// <summary>
		/// Parses the JSON into an object of the specified type.
		/// </summary>
		/// <typeparam name="T">The specified type.</typeparam>
		/// <param name="response">The response.</param>
		/// <param name="jsonSettings">The JSON settings.</param>
		/// <returns>An object of the specified type.</returns>
		/// <exception cref="WebServiceException">The response content does not use the JSON content type, or the content is empty,
		/// or the text is not valid JSON, or the JSON cannot be deserialized into the specified type.</exception>
		[return: MaybeNull]
		public static async Task<T> GetJsonAsAsync<T>(this WebServiceResponse response, JsonSettings? jsonSettings)
		{
			try
			{
				// parse JSON to desired value
				using var responseStream = await response.Content!.ReadAsStreamAsync().ConfigureAwait(false);
				using var reader = new StreamReader(responseStream);
				return JsonUtility.FromJsonTextReader<T>(reader, jsonSettings)!;
			}
			catch (JsonReaderException x)
			{
				// JSON invalid
				throw WebServiceResponseUtility.CreateWebServiceException(response, "Response is not valid JSON: " + x.Message, x);
			}
			catch (JsonSerializationException x)
			{
				// JSON can't be deserialized
				throw WebServiceResponseUtility.CreateWebServiceException(response, "Response JSON could not be deserialized to {0}: {1}".FormatInvariant(typeof(T), x.Message), x);
			}
		}
	}
}
