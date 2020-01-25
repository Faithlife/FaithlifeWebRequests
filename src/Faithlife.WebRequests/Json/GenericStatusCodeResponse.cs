using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Faithlife.WebRequests.Json
{
	/// <summary>
	/// AutoWebServiceRequest responses with an explicitly set status code. 
	/// </summary>
	public class GenericStatusCodeResponse : AutoWebServiceResponse
	{
		/// <summary>
		/// Exposes the response status code.
		/// </summary>
		public HttpStatusCode StatusCode { get; private set; }

		/// <summary>
		/// Exposes the response headers.
		/// </summary>
		public HttpResponseHeaders? Headers { get; private set; }

		/// <summary>
		/// Sets Headers property.
		/// </summary>
		/// <param name="info">The web service response handler information.</param>
		protected override async Task OnResponseHandledCoreAsync(WebServiceResponseHandlerInfo info)
		{
			await base.OnResponseHandledCoreAsync(info).ConfigureAwait(false);
			Headers = info.WebResponse!.Headers;
		}
	}
}
