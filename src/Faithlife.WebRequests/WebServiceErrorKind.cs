namespace Faithlife.WebRequests
{
	/// <summary>
	/// The kind of web service error.
	/// </summary>
	public enum WebServiceErrorKind
	{
		/// <summary>
		/// The error occurred while building the request.
		/// </summary>
		Request,

		/// <summary>
		/// The error occurred while handling the response.
		/// </summary>
		Response,
	}
}
