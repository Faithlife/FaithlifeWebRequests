using System.Net;
using System.Net.Http;
using Faithlife.Utility.Threading;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// Information provided to web service response handlers.
	/// </summary>
	public abstract class WebServiceResponseHandlerInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceResponseHandlerInfo&lt;TResponse&gt;"/> class.
		/// </summary>
		/// <param name="webResponse">The web response.</param>
		/// <param name="workState">State of the work.</param>
		protected WebServiceResponseHandlerInfo(HttpResponseMessage webResponse, IWorkState workState)
		{
			m_webResponse = webResponse;
			m_workState = workState;
		}

		/// <summary>
		/// Gets the web response.
		/// </summary>
		/// <value>The web response.</value>
		public HttpResponseMessage WebResponse
		{
			get { return m_webResponse; }
		}

		/// <summary>
		/// True if the content has been read from the web response.
		/// </summary>
		/// <value>True if the content has been read from the web response.</value>
		public bool IsContentRead
		{
			get { return m_isContentRead; }
		}

		/// <summary>
		/// Gets the work state.
		/// </summary>
		/// <value>The work state.</value>
		public IWorkState WorkState
		{
			get { return m_workState; }
		}

		/// <summary>
		/// Marks the content as having been read from the web response.
		/// </summary>
		public void MarkContentAsRead()
		{
			m_isContentRead = true;
		}

		/// <summary>
		/// Detaches the web response.
		/// </summary>
		/// <returns>The detached web response.</returns>
		public HttpResponseMessage DetachWebResponse()
		{
			HttpResponseMessage webResponse = m_webResponse;
			m_webResponse = null;
			return webResponse;
		}

		HttpResponseMessage m_webResponse;
		bool m_isContentRead;
		readonly IWorkState m_workState;
	}

	/// <summary>
	/// Information provided to web service response handlers.
	/// </summary>
	public sealed class WebServiceResponseHandlerInfo<TResponse> : WebServiceResponseHandlerInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WebServiceResponseHandlerInfo&lt;TResponse&gt;"/> class.
		/// </summary>
		/// <param name="webResponse">The web response.</param>
		/// <param name="workState">State of the work.</param>
		public WebServiceResponseHandlerInfo(HttpResponseMessage webResponse, IWorkState workState)
			: base(webResponse, workState)
		{
		}

		/// <summary>
		/// Gets or sets the response.
		/// </summary>
		/// <value>The response.</value>
		public TResponse Response { get; set; }
	}
}
