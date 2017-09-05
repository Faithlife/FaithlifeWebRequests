using System;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// A web service error.
	/// </summary>
	public sealed class WebServiceErrorInfo
	{
		/// <summary>
		/// Creates a web service error.
		/// </summary>
		public WebServiceErrorInfo(WebServiceErrorKind kind, Exception exception, WebServiceRequestBase request)
		{
			m_kind = kind;
			m_exception = exception;
			m_request = request;
		}

		/// <summary>
		/// The kind of web service error.
		/// </summary>
		public WebServiceErrorKind Kind
		{
			get { return m_kind; }
		}

		/// <summary>
		/// The exception to be wrapped in a WebServiceException and rethrown.
		/// </summary>
		public Exception Exception
		{
			get { return m_exception; }
		}

		/// <summary>
		/// The web service request.
		/// </summary>
		public WebServiceRequestBase Request
		{
			get { return m_request; }
		}

		readonly WebServiceErrorKind m_kind;
		readonly Exception m_exception;
		readonly WebServiceRequestBase m_request;
	}
}
