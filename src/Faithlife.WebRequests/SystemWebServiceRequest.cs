using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// A web service request that returns HttpResponseMessage.
	/// </summary>
	public class SystemWebServiceRequest : WebServiceRequestBase<HttpResponseMessage>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="SystemWebServiceRequest"/> class.
		/// </summary>
		/// <param name="uri">The URI.</param>
		public SystemWebServiceRequest(Uri uri)
			: base(uri)
		{
		}

		/// <summary>
		/// Overrides HandleResponseCoreAsync.
		/// </summary>
		protected override Task<bool> HandleResponseCoreAsync(WebServiceResponseHandlerInfo<HttpResponseMessage> info)
		{
			info.Response = info.DetachWebResponse()!;
			return Task.FromResult(true);
		}
	}
}
