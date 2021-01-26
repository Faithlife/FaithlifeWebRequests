using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Faithlife.Utility;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// Utility methods for WebServiceRequest and WebServiceRequestBase.
	/// </summary>
	public static class WebServiceRequestUtility
	{
		/// <summary>
		/// Sets the Settings of the WebServiceRequest.
		/// </summary>
		public static TWebServiceRequest WithSettings<TWebServiceRequest>(this TWebServiceRequest request, WebServiceRequestSettings? settings)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.Settings = settings;
			return request;
		}

		/// <summary>
		/// Add authorization header to <see cref="WebServiceRequestBase"/>
		/// </summary>
		public static TWebServiceRequest WithAuthorizationHeader<TWebServiceRequest>(this TWebServiceRequest request, string authorizationHeader)
			where TWebServiceRequest : WebServiceRequestBase
		{
			var additionalHeaders = new WebHeaderCollection();
			additionalHeaders[HttpRequestHeader.Authorization] = authorizationHeader;
			return request.WithAdditionalHeaders(additionalHeaders);
		}

		/// <summary>
		/// Sets the Accept of the WebServiceRequest.
		/// </summary>
		public static TWebServiceRequest WithAccept<TWebServiceRequest>(this TWebServiceRequest request, string accept)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.Accept = accept;
			return request;
		}

		/// <summary>
		/// Sets the AcceptedStatusCodes of the WebServiceRequest.
		/// </summary>
		public static TWebServiceRequest WithAcceptedStatusCodes<TWebServiceRequest>(this TWebServiceRequest request, params HttpStatusCode[] acceptedStatusCodes)
			where TWebServiceRequest : WebServiceRequest
		{
			request.AcceptedStatusCodes = acceptedStatusCodes?.AsReadOnly();
			return request;
		}

		/// <summary>
		/// Sets the AcceptedStatusCodes of the WebServiceRequest.
		/// </summary>
		public static TWebServiceRequest WithAcceptedStatusCodes<TWebServiceRequest>(this TWebServiceRequest request, IEnumerable<HttpStatusCode>? acceptedStatusCodes)
			where TWebServiceRequest : WebServiceRequest
		{
			request.AcceptedStatusCodes = acceptedStatusCodes?.ToList().AsReadOnly();
			return request;
		}

		/// <summary>
		/// Allows request content compression.
		/// </summary>
		public static TWebServiceRequest WithContentCompression<TWebServiceRequest>(this TWebServiceRequest request)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.AllowsRequestContentCompression = true;
			return request;
		}

		/// <summary>
		/// Allows request content compression.
		/// </summary>
		public static TWebServiceRequest WithContentCompression<TWebServiceRequest>(this TWebServiceRequest request, bool isEnabled)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.AllowsRequestContentCompression = isEnabled;
			return request;
		}

		/// <summary>
		/// Sets the Method of the WebServiceRequest.
		/// </summary>
		public static TWebServiceRequest WithMethod<TWebServiceRequest>(this TWebServiceRequest request, string method)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.Method = method;
			return request;
		}

		/// <summary>
		/// Sets the Method of the WebServiceRequest to "PATCH".
		/// </summary>
		public static TWebServiceRequest WithPatchMethod<TWebServiceRequest>(this TWebServiceRequest request)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.Method = "PATCH";
			return request;
		}

		/// <summary>
		/// Sets the Method of the WebServiceRequest to "POST".
		/// </summary>
		public static TWebServiceRequest WithPostMethod<TWebServiceRequest>(this TWebServiceRequest request)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.Method = "POST";
			return request;
		}

		/// <summary>
		/// Sets the Method of the WebServiceRequest to "PUT".
		/// </summary>
		public static TWebServiceRequest WithPutMethod<TWebServiceRequest>(this TWebServiceRequest request)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.Method = "PUT";
			return request;
		}

		/// <summary>
		/// Sets the Method of the WebServiceRequest to "DELETE".
		/// </summary>
		public static TWebServiceRequest WithDeleteMethod<TWebServiceRequest>(this TWebServiceRequest request)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.Method = "DELETE";
			return request;
		}

		/// <summary>
		/// Sets the Method of the WebServiceRequest to "HEAD".
		/// </summary>
		public static TWebServiceRequest WithHeadMethod<TWebServiceRequest>(this TWebServiceRequest request)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.Method = "HEAD";
			return request;
		}

		/// <summary>
		/// Sets the Content of the WebServiceRequest.
		/// </summary>
		public static TWebServiceRequest WithContent<TWebServiceRequest>(this TWebServiceRequest request, HttpContent content)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.Content = content;
			return request;
		}

		/// <summary>
		/// Sets the IfMatch of the WebServiceRequest.
		/// </summary>
		public static TWebServiceRequest WithIfMatch<TWebServiceRequest>(this TWebServiceRequest request, string eTag)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.IfMatch = eTag;
			return request;
		}

		/// <summary>
		/// Sets the IfModifiedSince of the WebServiceRequest.
		/// </summary>
		public static TWebServiceRequest WithIfModifiedSince<TWebServiceRequest>(this TWebServiceRequest request, DateTime? ifModifiedSince)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.IfModifiedSince = ifModifiedSince;
			return request;
		}

		/// <summary>
		/// Sets the IfNoneMatch of the WebServiceRequest.
		/// </summary>
		public static TWebServiceRequest WithIfNoneMatch<TWebServiceRequest>(this TWebServiceRequest request, string eTag)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.IfNoneMatch = eTag;
			return request;
		}

		/// <summary>
		/// Sets the Timeout of the WebServiceRequest.
		/// </summary>
		public static TWebServiceRequest WithTimeout<TWebServiceRequest>(this TWebServiceRequest request, TimeSpan? timeout)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.Timeout = timeout;
			return request;
		}

		/// <summary>
		/// Adds to the Handlers of the WebServiceRequest.
		/// </summary>
		public static WebServiceRequest<TWebServiceResponse> WithHandler<TWebServiceResponse>(this WebServiceRequest<TWebServiceResponse> request, Func<WebServiceResponseHandlerInfo<TWebServiceResponse>, Task<bool>> handler)
		{
			request.Handlers.Add(handler);
			return request;
		}

		/// <summary>
		/// Adds to the Additional Headers of the WebServiceRequest.
		/// </summary>
		public static TWebServiceRequest WithAdditionalHeaders<TWebServiceRequest>(this TWebServiceRequest request, WebHeaderCollection headers)
			where TWebServiceRequest : WebServiceRequestBase
		{
			if (headers is null)
				throw new ArgumentNullException(nameof(headers));

			if (request.AdditionalHeaders is null || request.AdditionalHeaders.Count == 0)
			{
				// replace the existing collection if it is null or empty
				request.AdditionalHeaders = headers;
			}
			else if (headers.Count != 0)
			{
				// avoid mutating an existing collection in case it is used by others
				WebHeaderCollection newHeaders = new WebHeaderCollection();
				newHeaders.Add(request.AdditionalHeaders);
				newHeaders.Add(headers);
				request.AdditionalHeaders = newHeaders;
			}

			return request;
		}
	}
}
