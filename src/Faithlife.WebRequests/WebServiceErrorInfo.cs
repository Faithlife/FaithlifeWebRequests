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
			Kind = kind;
			Exception = exception;
			Request = request;
		}

		/// <summary>
		/// The kind of web service error.
		/// </summary>
		public WebServiceErrorKind Kind { get; }

		/// <summary>
		/// The exception to be wrapped in a WebServiceException and rethrown.
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// The web service request.
		/// </summary>
		public WebServiceRequestBase Request { get; }
	}
}
