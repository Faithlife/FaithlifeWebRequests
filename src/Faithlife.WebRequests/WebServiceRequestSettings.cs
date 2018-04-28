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
		public string UserAgent { get; set; }

		/// <summary>
		/// Gets or sets the cookie manager.
		/// </summary>
		/// <value>The cookie manager.</value>
		public CookieManager CookieManager { get; set; }

		/// <summary>
		/// Gets or sets the authorization header.
		/// </summary>
		/// <value>The authorization header.</value>
		/// <remarks>Only one of AuthorizationHeader and AuthorizationHeaderCreator can be set.</remarks>
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
		public WebHeaderCollection DefaultHeaders { get; set; }

		/// <summary>
		/// Gets or sets the default timeout.
		/// </summary>
		/// <value>The default timeout.</value>
		public TimeSpan? DefaultTimeout { get; set; }

		/// <summary>
		/// True if 100-Continue behavior should be disabled.
		/// </summary>
		/// <value>True if 100-Continue behavior should be disabled.</value>
		/// <remarks>May not be supported on all platforms.</remarks>
		public bool Disable100Continue { get; set; }

		/// <summary>
		/// True if the connection should be closed after this web service request is made.
		/// </summary>
		/// <remarks>May not be supported on all platforms.</remarks>
		public bool DisableKeepAlive { get; set; }

		/// <summary>
		/// Gets or sets the host.
		/// </summary>
		/// <value>The host.</value>
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
		/// Clones this instance.
		/// </summary>
		/// <returns>The clone.</returns>
		public WebServiceRequestSettings Clone()
		{
			return (WebServiceRequestSettings) MemberwiseClone();
		}
	}
}
