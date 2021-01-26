using Faithlife.Json;

namespace Faithlife.WebRequests.Json
{
	/// <summary>
	/// Utility methods for JSON and WebServiceRequest.
	/// </summary>
	public static class JsonWebServiceRequestUtility
	{
		/// <summary>
		/// Sets the Content of the WebServiceRequest.
		/// </summary>
		public static TWebServiceRequest WithJsonContent<TWebServiceRequest, TContentValue>(this TWebServiceRequest request, TContentValue contentValue)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.Content = JsonWebServiceContent.FromValue(contentValue);
			return request;
		}

		/// <summary>
		/// Sets the Content of the WebServiceRequest.
		/// </summary>
		public static TWebServiceRequest WithJsonContent<TWebServiceRequest, TContentValue>(this TWebServiceRequest request, TContentValue contentValue, JsonSettings settings)
			where TWebServiceRequest : WebServiceRequestBase
		{
			request.Content = JsonWebServiceContent.FromValue(contentValue, settings);
			return request;
		}

		/// <summary>
		/// Sets the JsonSettings of the WebServiceRequest.
		/// </summary>
		public static JsonWebServiceRequest<TResponseContent> WithJsonSettings<TResponseContent>(this JsonWebServiceRequest<TResponseContent> request, JsonSettings settings)
		{
			request.JsonSettings = settings;
			return request;
		}
	}
}
