using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Faithlife.WebRequests.Json
{
	/// <summary>
	/// A JSON web response.
	/// </summary>
	public class JsonWebServiceResponse : WebServiceResponse
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsonWebServiceResponse&lt;TContent&gt;"/> class.
		/// </summary>
		/// <param name="request">The <see cref="WebServiceRequest"/>.</param>
		/// <param name="statusCode">The status code.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="content">The content.</param>
		public JsonWebServiceResponse(WebServiceRequest request, HttpStatusCode statusCode, HttpHeaders headers, HttpContent? content)
			: base(request, statusCode, headers, content)
		{
		}

		/// <summary>
		/// Gets the value, if any.
		/// </summary>
		/// <value>The value, if any.</value>
		/// <remarks>The Value will be null if the response content was not JSON.</remarks>
		public string? Json
		{
			get
			{
				return Content is JsonWebServiceContent content ? content.Json : null;
			}
		}
	}

	/// <summary>
	/// A JSON web response.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Generic.")]
	public class JsonWebServiceResponse<TValue> : JsonWebServiceResponse
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsonWebServiceResponse&lt;TContent&gt;"/> class.
		/// </summary>
		/// <param name="request">The <see cref="WebServiceRequest"/>.</param>
		/// <param name="statusCode">The status code.</param>
		/// <param name="headers">The headers.</param>
		/// <param name="content">The content.</param>
		public JsonWebServiceResponse(WebServiceRequest request, HttpStatusCode statusCode, HttpHeaders headers, HttpContent? content)
			: base(request, statusCode, headers, content)
		{
		}

		/// <summary>
		/// Gets the value, if any.
		/// </summary>
		/// <value>The value, if any.</value>
		/// <remarks>The Value will be null if the response content was not JSON or could not be
		/// deserialized into an instance of TValue.</remarks>
		public TValue Value => Content is JsonWebServiceContent<TValue> content ? content.Value : default!;
	}
}
