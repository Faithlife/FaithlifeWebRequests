using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Faithlife.Json;
using Faithlife.Utility;
using Newtonsoft.Json;

namespace Faithlife.WebRequests.Json
{
	/// <summary>
	/// A JSON web service request.
	/// </summary>
	public class JsonWebServiceRequest : WebServiceRequest
	{
		/// <summary>
		/// Initializes a new instance of the JsonWebServiceRequest class.
		/// </summary>
		/// <param name="uri">The URI.</param>
		public JsonWebServiceRequest(Uri uri)
			: base(uri)
		{
		}

		/// <summary>
		/// Gets the response asynchronously.
		/// </summary>
		public new async Task<JsonWebServiceResponse> GetResponseAsync(CancellationToken cancellationToken = default) =>
			(JsonWebServiceResponse) await base.GetResponseAsync(cancellationToken).ConfigureAwait(false);

		/// <summary>
		/// Called to create the response.
		/// </summary>
		/// <param name="proposedResponse">The proposed response.</param>
		/// <returns>The response.</returns>
		protected override async Task<WebServiceResponse> CreateResponseAsync(WebServiceResponse proposedResponse)
		{
			HttpStatusCode statusCode = proposedResponse.StatusCode;
			HttpHeaders headers = proposedResponse.Headers;
			var content = proposedResponse.Content;

			// check for content
			if (content is object)
			{
				// check content type
				if (proposedResponse.HasJson())
				{
					string responseJson = await proposedResponse.GetJsonAsync().ConfigureAwait(false);

					// success
					return new JsonWebServiceResponse(this, statusCode, headers, JsonWebServiceContent.FromJson(responseJson));
				}
				else
				{
					// got content with the wrong content type (HTML error information, perhaps; allowed for non-OK)
					string message = "Response content type is not JSON: {0}".FormatInvariant(content.Headers.ContentType);
					if (statusCode == HttpStatusCode.OK)
						throw WebServiceResponseUtility.CreateWebServiceException(proposedResponse, message);
				}
			}

			// missing or non-JSON content
			return new JsonWebServiceResponse(this, statusCode, headers, content);
		}
	}

	/// <summary>
	/// A JSON web service request.
	/// </summary>
	/// <typeparam name="TResponseValue">The type of the response value.</typeparam>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Generic.")]
	public class JsonWebServiceRequest<TResponseValue> : JsonWebServiceRequest
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsonWebServiceRequest&lt;TResponseContent&gt;"/> class.
		/// </summary>
		/// <param name="uri">The URI.</param>
		public JsonWebServiceRequest(Uri uri)
			: base(uri)
		{
		}

		/// <summary>
		/// Gets or sets the JSON settings.
		/// </summary>
		/// <value>The JSON settings (used when converting the response from JSON).</value>
		public JsonSettings? JsonSettings { get; set; }

		/// <summary>
		/// Gets the response asynchronously.
		/// </summary>
		public new async Task<JsonWebServiceResponse<TResponseValue>> GetResponseAsync(CancellationToken cancellationToken = default) =>
			(JsonWebServiceResponse<TResponseValue>) await base.GetResponseAsync(cancellationToken).ConfigureAwait(false);

		/// <summary>
		/// Called to create the response.
		/// </summary>
		/// <param name="proposedResponse">The proposed response.</param>
		/// <returns>The response.</returns>
		protected override async Task<WebServiceResponse> CreateResponseAsync(WebServiceResponse proposedResponse)
		{
			proposedResponse = await base.CreateResponseAsync(proposedResponse).ConfigureAwait(false);

			HttpStatusCode statusCode = proposedResponse.StatusCode;
			HttpHeaders headers = proposedResponse.Headers;
			HttpContent? content = proposedResponse.Content;

			var json = ((JsonWebServiceResponse) proposedResponse).Json;
			if (json is object)
			{
				// don't allow missing JSON
				if (json.Length == 0)
					throw WebServiceResponseUtility.CreateWebServiceException(proposedResponse, "JSON response is empty.");

				try
				{
					// parse JSON to desired value
					TResponseValue value = JsonUtility.FromJson<TResponseValue>(json, JsonSettings);
					return new JsonWebServiceResponse<TResponseValue>(this, statusCode, headers, JsonWebServiceContent.FromValue(value));
				}
				catch (JsonReaderException x)
				{
					// don't allow invalid JSON
					throw WebServiceResponseUtility.CreateWebServiceException(proposedResponse, "Response is not valid JSON: " + x.Message, x);
				}
				catch (JsonSerializationException x)
				{
					// got JSON that can't be deserialized (error information, perhaps; allowed for non-OK)
					string message = "Response JSON could not be deserialized to {0}: {1} / JSON: {2}".FormatInvariant(typeof(TResponseValue), x.Message, json);
					if (statusCode == HttpStatusCode.OK)
						throw WebServiceResponseUtility.CreateWebServiceException(proposedResponse, message, x);
				}
			}
			else if (statusCode == HttpStatusCode.OK)
			{
				// 200 OK needs to have content
				throw WebServiceResponseUtility.CreateWebServiceException(proposedResponse, "OK but no content.");
			}

			return new JsonWebServiceResponse<TResponseValue>(this, statusCode, headers, content);
		}
	}
}
