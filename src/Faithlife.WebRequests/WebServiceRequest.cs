using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Faithlife.Utility;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// A web service request.
	/// </summary>
	public class WebServiceRequest<TResponse> : WebServiceRequestBase<TResponse>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceRequest"/> class.
		/// </summary>
		/// <param name="uri">The URI.</param>
		public WebServiceRequest(Uri uri)
			: base(uri)
		{
			Handlers = new Collection<Func<WebServiceResponseHandlerInfo<TResponse>, Task<bool>>>();
		}

		/// <summary>
		/// Gets the response handlers.
		/// </summary>
		public Collection<Func<WebServiceResponseHandlerInfo<TResponse>, Task<bool>>> Handlers { get; }

		/// <summary>
		/// Overrides HandleResponseCore.
		/// </summary>
		protected override async Task<bool> HandleResponseCoreAsync(WebServiceResponseHandlerInfo<TResponse> info)
		{
			foreach (var handler in Handlers)
			{
				if (await handler(info).ConfigureAwait(false))
					return true;
			}
			return false;
		}
	}

	/// <summary>
	/// A web service request.
	/// </summary>
	public class WebServiceRequest : WebServiceRequestBase<WebServiceResponse>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceRequest"/> class.
		/// </summary>
		/// <param name="uri">The URI.</param>
		public WebServiceRequest(Uri uri)
			: base(uri)
		{
			AcceptedStatusCodes = s_defaultAcceptedStatusCodes;
		}

		/// <summary>
		/// Gets or sets the accepted status codes.
		/// </summary>
		/// <value>The accepted status codes; if null, all status codes are accepted.</value>
		/// <remarks>The default value of this property contains HttpStatusCode.OK and HttpStatusCode.Created.</remarks>
		public IReadOnlyList<HttpStatusCode>? AcceptedStatusCodes { get; set; }

		/// <summary>
		/// Called to create the response.
		/// </summary>
		/// <param name="proposedResponse">The proposed response.</param>
		/// <returns>The response.</returns>
		/// <remarks>The default implementation simply returns the proposed response.</remarks>
		protected virtual Task<WebServiceResponse> CreateResponseAsync(WebServiceResponse proposedResponse) =>
			Task.FromResult(proposedResponse);

		/// <summary>
		/// Overrides OnWebRequestCreated.
		/// </summary>
		protected override void OnWebRequestCreated(HttpRequestMessage request)
		{
			base.OnWebRequestCreated(request);

			// if the client has disabled automatic redirection and hasn't changed the default accepted status codes, then
			//   allow redirect status codes to be passed through
			if (DisableAutoRedirect && object.ReferenceEquals(AcceptedStatusCodes, s_defaultAcceptedStatusCodes))
				AcceptedStatusCodes = s_defaultAcceptedStatusCodesWithRedirect;
		}

		/// <summary>
		/// Overrides HandleResponseCore.
		/// </summary>
		protected override async Task<bool> HandleResponseCoreAsync(WebServiceResponseHandlerInfo<WebServiceResponse> info)
		{
			HttpStatusCode statusCode = info.WebResponse!.StatusCode;
			HttpHeaders headers = info.WebResponse.Headers;
			HttpContent responseContent = info.WebResponse.Content;

			WebServiceResponse response = new WebServiceResponse(this, statusCode, headers, responseContent);

			if (AcceptedStatusCodes is object && !AcceptedStatusCodes.Contains(statusCode))
				throw WebServiceResponseUtility.CreateWebServiceException(response);

			info.Response = await CreateResponseAsync(response).ConfigureAwait(false);
			if (!info.IsContentRead)
				info.DetachWebResponse();
			return true;
		}

		static readonly IReadOnlyList<HttpStatusCode> s_defaultAcceptedStatusCodes = new[] { HttpStatusCode.OK, HttpStatusCode.Created }.AsReadOnly();
		static readonly IReadOnlyList<HttpStatusCode> s_defaultAcceptedStatusCodesWithRedirect = new[] { HttpStatusCode.OK, HttpStatusCode.Created,
			HttpStatusCode.Moved, HttpStatusCode.Redirect, HttpStatusCode.RedirectMethod,  HttpStatusCode.RedirectKeepVerb }.AsReadOnly();
	}
}
