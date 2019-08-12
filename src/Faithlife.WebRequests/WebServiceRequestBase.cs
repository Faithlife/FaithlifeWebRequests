using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Faithlife.Utility;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// A base class for web service requests.
	/// </summary>
	public abstract class WebServiceRequestBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceRequestBase"/> class.
		/// </summary>
		/// <param name="uri">The URI.</param>
		protected WebServiceRequestBase(Uri uri)
		{
			if (uri == null)
				throw new ArgumentNullException("uri");
			if (uri.Scheme != "http" && uri.Scheme != "https")
				throw new ArgumentException("Expected URI with http or https scheme; received {0}".FormatInvariant(uri.Scheme));

			m_uri = uri;
		}

		/// <summary>
		/// Gets the request URI.
		/// </summary>
		/// <value>The request URI.</value>
		public Uri RequestUri
		{
			get { return m_uri; }
		}

		/// <summary>
		/// Gets or sets the settings.
		/// </summary>
		/// <value>The settings.</value>
		public WebServiceRequestSettings Settings { get; set; }

		/// <summary>
		/// Gets or sets the method.
		/// </summary>
		/// <value>The method.</value>
		/// <remarks>Defaults to "GET".</remarks>
		public string Method
		{
			get { return m_method ?? "GET"; }
			set { m_method = value; }
		}

		/// <summary>
		/// Gets or sets the content.
		/// </summary>
		/// <value>The content.</value>
		public HttpContent Content { get; set; }

		/// <summary>
		/// Gets or sets the If-Match header.
		/// </summary>
		/// <value>The If-Match ETag.</value>
		public string IfMatch { get; set; }

		/// <summary>
		/// Gets or sets if modified since.
		/// </summary>
		/// <value>If modified since.</value>
		public DateTime? IfModifiedSince { get; set; }

		/// <summary>
		/// Gets or sets the If-None-Match header.
		/// </summary>
		/// <value>The If-None-Match ETag.</value>
		public string IfNoneMatch { get; set; }

		/// <summary>
		/// Gets or sets the timeout. If <see cref="WebServiceRequestSettings.GetHttpClient"/> is set, then this property is ignored.
		/// </summary>
		/// <value>The timeout.</value>
		public TimeSpan? Timeout { get; set; }

		/// <summary>
		/// Gets or sets the additional headers.
		/// </summary>
		/// <value>A collection of additional headers to include in the request.</value>
		/// <remarks>Use this property to specify headers that are not already exposed by other
		/// properties on the request.</remarks>
		public WebHeaderCollection AdditionalHeaders { get; set; }

		/// <summary>
		/// Gets or sets the value of the Accept HTTP header.
		/// </summary>
		/// <value>The value of the Accept HTTP header. The default value is null.</value>
		public string Accept { get; set; }

		/// <summary>
		/// Gets or sets the value of the Referer HTTP header.
		/// </summary>
		/// <value>The value of the Referer HTTP header. The default value is null.</value>
		public string Referer { get; set; }

		/// <summary>
		/// Gets or sets the value of the User-agent HTTP header.
		/// </summary>
		/// <value>The value of the User-agent HTTP header. The default value is null.</value>
		public string UserAgent { get; set; }

		/// <summary>
		/// True if the request content compression is allowed.
		/// </summary>
		/// <value>True if the request content compression is allowed.</value>
		/// <remarks>If it significantly reduces the content length, the request content will be compressed
		/// and the content type will be wrapped with application/x-vnd.logos.compressed; type="...".</remarks>
		public bool AllowsRequestContentCompression { get; set; }

		/// <summary>
		/// Gets or sets the byte range to be used by the Range header.
		/// </summary>
		/// <value>The byte range.</value>
		public ByteRange Range { get; set; }

		/// <summary>
		/// True if HTTP redirects should not be followed automatically. If <see cref="WebServiceRequestSettings.HttpClientFactory"/> or <see cref="WebServiceRequestSettings.GetHttpClient"/> are set, then this property is ignored.
		/// </summary>
		[Obsolete("Use HttpClientFactory")]
		public bool DisableAutoRedirect { get; set; }

		readonly Uri m_uri;
		string m_method;
	}

	/// <summary>
	/// A base class for web service requests.
	/// </summary>
	public abstract class WebServiceRequestBase<TResponse> : WebServiceRequestBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceRequestBase"/> class.
		/// </summary>
		/// <param name="uri">The URI.</param>
		protected WebServiceRequestBase(Uri uri)
			: base(uri)
		{
		}

		/// <summary>
		/// Gets the response asynchronously.
		/// </summary>
		public async Task<TResponse> GetResponseAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			var settings = Settings ?? new WebServiceRequestSettings();
			var client = CreateHttpClient(settings);
			var webRequest = CreateHttpRequestMessage(settings);
			var requestContent = GetRequestContent(webRequest);
			if (requestContent != null)
				webRequest.Content = requestContent;
			using (Settings?.StartTrace?.Invoke(webRequest))
			{
				HttpResponseMessage response;
				try
				{
					response = await client.SendAsync(webRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
				}
				catch (Exception ex) when (ShouldWrapException(ex))
				{
					throw new WebServiceException(message: "Error building request.", requestMethod: webRequest.Method.Method, requestUri: webRequest.RequestUri, innerException: ex);
				}

				return await HandleResponseAsync(webRequest, response, cancellationToken).ConfigureAwait(false);
			}
		}

		/// <summary>
		/// Called to handle the response.
		/// </summary>
		/// <param name="info">The info.</param>
		/// <returns>True if the response was handled and info.Response should be returned. False if the response
		/// was not handled and an exception should be thrown.</returns>
		/// <remarks>The following types of exceptions will be safely wrapped by a WebServiceException: InvalidDataException,
		/// IOException, WebException, and ObjectDisposedException.</remarks>
		protected abstract Task<bool> HandleResponseCoreAsync(WebServiceResponseHandlerInfo<TResponse> info);

		/// <summary>
		/// Called to modify the request before it is sent.
		/// </summary>
		/// <param name="request">The HTTP web request.</param>
		protected virtual void OnWebRequestCreated(HttpRequestMessage request)
		{
		}

#pragma warning disable CS0618
		private HttpClientHandler CreateHttpClientHandler(WebServiceRequestSettings settings)
		{
			var handler = new HttpClientHandler();
			if (settings.CookieManager != null)
				handler.CookieContainer = settings.CookieManager.CookieContainer;

			handler.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

			if (DisableAutoRedirect)
				handler.AllowAutoRedirect = false;

			return handler;
		}

		private HttpClient CreateHttpClient(WebServiceRequestSettings settings)
		{
			if (settings.HttpClientFactory != null)
			{
				return settings.HttpClientName != null ?
					settings.HttpClientFactory.CreateClient(settings.HttpClientName) :
					settings.HttpClientFactory.CreateClient();
			}

			if (settings.GetHttpClient != null)
				return settings.GetHttpClient();

			var client = new HttpClient(CreateHttpClientHandler(settings));
			var timeout = Timeout ?? settings.DefaultTimeout;
			client.Timeout = timeout ?? System.Threading.Timeout.InfiniteTimeSpan;
			return client;
		}

		private HttpRequestMessage CreateHttpRequestMessage(WebServiceRequestSettings settings)
		{
			var request = new HttpRequestMessage(new HttpMethod(Method ?? "GET"), RequestUri);

			if (settings.DefaultHeaders != null)
				request.Headers.AddWebHeaders(settings.DefaultHeaders);

			if (AdditionalHeaders != null)
				request.Headers.AddWebHeaders(AdditionalHeaders);

			if (!string.IsNullOrEmpty(Accept))
				request.Headers.Accept.ParseAdd(Accept);

			var userAgent = UserAgent ?? settings.UserAgent;
			if (!string.IsNullOrEmpty(userAgent))
				request.Headers.UserAgent.ParseAdd(userAgent);

			if (!string.IsNullOrEmpty(Referer))
				request.Headers.Add("Referer", Referer);

			if (settings.Disable100Continue)
				request.Headers.ExpectContinue = false;

			if (settings.DisableKeepAlive)
				request.Headers.ConnectionClose = true;

			if (settings.Host != null)
				request.Headers.Host = settings.Host;

			var authorizationHeader = settings.AuthorizationHeader ?? settings.AuthorizationHeaderCreator?.Invoke(new WebServiceRequestInfo(request));
			if (authorizationHeader != null)
				request.Headers.Authorization = AuthenticationHeaderValue.Parse(authorizationHeader);

			if (IfMatch != null)
				request.Headers.IfMatch.ParseAdd(IfMatch);

			if (IfModifiedSince.HasValue)
				request.Headers.IfModifiedSince = IfModifiedSince.Value;

			if (IfNoneMatch != null)
				request.Headers.IfNoneMatch.ParseAdd(IfNoneMatch);

			if (Range != null)
				request.Headers.Range = new RangeHeaderValue(Range.From, Range.HasEnd ? Range.To : default(long?));

			OnWebRequestCreated(request);
			return request;
		}
#pragma warning restore CS0618

		private HttpContent GetRequestContent(HttpRequestMessage webRequest)
		{
			HttpContent requestContent = Content;

			// IIS doesn't like a POST/PUT with implicitly empty content (TODO: confirm, case 34924)
			if (requestContent == null && (webRequest.Method == HttpMethod.Post || webRequest.Method == HttpMethod.Put))
				requestContent = new StreamContent(Stream.Null) { Headers = { ContentType = new MediaTypeHeaderValue(c_octetStreamContentType) } };

			return requestContent;
		}

		private async Task<TResponse> HandleResponseAsync(HttpRequestMessage webRequest, HttpResponseMessage webResponse, CancellationToken cancellationToken)
		{
			// handle web response
			WebServiceResponseHandlerInfo<TResponse> info = new WebServiceResponseHandlerInfo<TResponse>(webResponse, cancellationToken);
			try
			{
				if (!await HandleResponseCoreAsync(info).ConfigureAwait(false))
				{
					cancellationToken.ThrowIfCancellationRequested();
					throw new WebServiceException("Web response not handled.", webRequest.Method?.Method, RequestUri);
				}
				cancellationToken.ThrowIfCancellationRequested();

				try
				{
					// set cookies of handled responses
					SetCookie(webResponse.Headers, webRequest.RequestUri);
				}
				catch (CookieException ex)
				{
					throw new WebServiceException("Failure setting cookie.", requestMethod: webRequest.Method?.Method, requestUri: RequestUri, innerException: ex);
				}
			}
			catch (Exception ex) when (ShouldWrapException(ex))
			{
				throw HttpResponseMessageUtility.CreateWebServiceException(webResponse, "Error handling response.", ex);
			}
			finally
			{
				// dispose HttpResponseMessage unless detached
				if (info.WebResponse != null)
					((IDisposable) info.DetachWebResponse()).Dispose();
			}

			// return result
			return info.Response;
		}

#pragma warning disable CS0618
		private void SetCookie(HttpResponseHeaders headers, Uri requestUri)
		{
			if (headers == null)
				return;
			if (Settings == null || Settings.CookieManager == null)
				return;

			if (headers.TryGetValues("Set-Cookie", out var values))
			{
				string cookieHeader = values.Join("; ");
				if (cookieHeader != "")
					Settings.CookieManager.SetCookies(requestUri, cookieHeader);
			}
		}
#pragma warning restore CS0618

		private static bool ShouldWrapException(Exception exception)
		{
			// InvalidDataException can be thrown if GZip stream header returned from server is invalid.
			return
				exception is HttpRequestException ||
				exception is IOException ||
				exception is InvalidDataException ||
				exception is ProtocolViolationException;
		}

		const string c_octetStreamContentType = "application/octet-stream";
	}

	internal static class HttpHeadersUtility
	{
		public static void AddWebHeaders(this HttpHeaders self, WebHeaderCollection headers)
		{
			foreach (var headerName in headers.AllKeys)
				self.Add(headerName, headers[headerName]);
		}

		public static WebHeaderCollection ToWebHeaderCollection(this HttpHeaders self)
		{
			var collection = new WebHeaderCollection();
			foreach (var pair in self)
				collection.Add(pair.Key, pair.Value.Join("; "));
			return collection;
		}
	}
}
