using System;
using System.Net;
using System.Net.Http;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// Web service request settings that are often common to multiple requests.
	/// </summary>
	public class WebServiceRequestSettings
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceRequestSettings"/> class.
		/// </summary>
		public WebServiceRequestSettings()
		{
		}

		/// <summary>
		/// Gets or sets the user agent.
		/// </summary>
		/// <value>The user agent.</value>
		[Obsolete("Use HttpClientFactory to set default values on HttpClient.")]
		public string UserAgent { get; set; }

		/// <summary>
		/// Gets or sets the cookie manager. If <see cref="HttpClientFactory"/> or <see cref="GetHttpClient"/> are set, then this property is ignored.
		/// </summary>
		/// <value>The cookie manager.</value>
		[Obsolete("Use HttpClientFactory to set default values on HttpClient.")]
		public CookieManager CookieManager { get; set; }

		/// <summary>
		/// Gets or sets the authorization header.
		/// </summary>
		/// <value>The authorization header.</value>
		/// <remarks>Only one of AuthorizationHeader and AuthorizationHeaderCreator can be set.</remarks>
		[Obsolete("Use HttpClientFactory to set default values on HttpClient.")]
		public string AuthorizationHeader { get; set; }

		/// <summary>
		/// Gets or sets the authorization header creator.
		/// </summary>
		/// <value>The authorization header creator.</value>
		/// <remarks>Only one of AuthorizationHeader and AuthorizationHeaderCreator can be set.</remarks>
		public Func<WebServiceRequestInfo, string> AuthorizationHeaderCreator { get; set; }

		/// <summary>
		/// Gets or sets the default headers.
		/// </summary>
		/// <value>A collection of additional headers to include in every request.</value>
		/// <remarks>Use this property to specify headers that are not already exposed by other
		/// properties on the settings.</remarks>
		[Obsolete("Use HttpClientFactory to set default values on HttpClient.")]
		public WebHeaderCollection DefaultHeaders { get; set; }

		/// <summary>
		/// Gets or sets the default timeout. If <see cref="HttpClientFactory"/> or <see cref="GetHttpClient"/> are set, then this property is ignored.
		/// </summary>
		/// <value>The default timeout.</value>
		[Obsolete("Use HttpClientFactory to set default values on HttpClient.")]
		public TimeSpan? DefaultTimeout { get; set; }

		/// <summary>
		/// True if 100-Continue behavior should be disabled.
		/// </summary>
		/// <value>True if 100-Continue behavior should be disabled.</value>
		/// <remarks>May not be supported on all platforms.</remarks>
		[Obsolete("Use HttpClientFactory to set default values on HttpClient.")]
		public bool Disable100Continue { get; set; }

		/// <summary>
		/// True if the connection should be closed after this web service request is made.
		/// </summary>
		/// <remarks>May not be supported on all platforms.</remarks>
		[Obsolete("Use HttpClientFactory to set default values on HttpClient.")]
		public bool DisableKeepAlive { get; set; }

		/// <summary>
		/// Gets or sets the host.
		/// </summary>
		/// <value>The host.</value>
		[Obsolete("Use HttpClientFactory to set default values on HttpClient.")]
		public string Host { get; set; }

		/// <summary>
		/// Called when an error occurs while building a request or handling a response.
		/// </summary>
		public Action<WebServiceErrorInfo> ErrorReporter { get; set; }

		/// <summary>
		/// A <see cref="Func{HttpRequestMessage, IDisposable}"/> that, if set, is called to start
		/// a trace when a web request begins; its return value will be disposed when the web
		/// request ends.
		/// </summary>
		public Func<HttpRequestMessage, IDisposable> StartTrace { get; set; }

		/// <summary>
		/// A delegate that, if set, is called to retrieve an <see cref="HttpClient"/>. If this property is set, then the consumer is responsible for the entire lifetime of the <see cref="HttpClient"/>, including disposal.
		/// </summary>
		/// <remarks>
		/// If this property is set, then <see cref="CookieManager"/>, <see cref="WebServiceRequestBase.DisableAutoRedirect"/>, <see cref="DefaultTimeout"/>, and <see cref="WebServiceRequestBase.Timeout"/> are ignored.
		/// </remarks>
		[Obsolete("Use HttpClientFactory and HttpClientName.")]
		public Func<HttpClient> GetHttpClient { get; set; }

		/// <summary>
		/// A factory that, if set, is called to create an <see cref="HttpClient"/>.
		/// </summary>
		/// <remarks>
		/// If this property is set, then <see cref="CookieManager"/>, <see cref="WebServiceRequestBase.DisableAutoRedirect"/>, <see cref="DefaultTimeout"/>, and <see cref="WebServiceRequestBase.Timeout"/> are ignored.
		/// </remarks>
		public IHttpClientFactory HttpClientFactory { get; set; }

		/// <summary>
		/// The logical name of the client to create with <see cref="HttpClientFactory"/>.
		/// </summary>
		public string HttpClientName { get; set; }

		/// <summary>
		/// Clones this instance.
		/// </summary>
		/// <returns>The clone.</returns>
		public WebServiceRequestSettings Clone()
		{
			return (WebServiceRequestSettings) MemberwiseClone();
		}
	}
}
