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
		/// <typeparam name="TWebServiceRequest">The type of the web service request.</typeparam>
		/// <typeparam name="TContentValue">The type of the content value.</typeparam>
		/// <param name="request">The request.</param>
		/// <param name="contentValue">The content value.</param>
		/// <returns>The request.</returns>
		public static TWebServiceRequest WithJsonContent<TWebServiceRequest, TContentValue>(this TWebServiceRequest request, TContentValue contentValue) where TWebServiceRequest : WebServiceRequestBase
		{
			request.Content = JsonWebServiceContent.FromValue(contentValue);
			return request;
		}

		/// <summary>
		/// Sets the Content of the WebServiceRequest.
		/// </summary>
		/// <typeparam name="TWebServiceRequest">The type of the web service request.</typeparam>
		/// <typeparam name="TContentValue">The type of the content value.</typeparam>
		/// <param name="request">The request.</param>
		/// <param name="contentValue">The content value.</param>
		/// <param name="settings">The settings.</param>
		/// <returns>The request.</returns>
		public static TWebServiceRequest WithJsonContent<TWebServiceRequest, TContentValue>(this TWebServiceRequest request, TContentValue contentValue, JsonOutputSettings settings) where TWebServiceRequest : WebServiceRequestBase
		{
			request.Content = JsonWebServiceContent.FromValue(contentValue, settings);
			return request;
		}

		/// <summary>
		/// Sets the InputSettings of the WebServiceRequest.
		/// </summary>
		/// <typeparam name="TResponseContent">The type of the response content.</typeparam>
		/// <param name="request">The request.</param>
		/// <param name="settings">The settings.</param>
		/// <returns>The request.</returns>
		public static JsonWebServiceRequest<TResponseContent> WithInputSettings<TResponseContent>(this JsonWebServiceRequest<TResponseContent> request, JsonInputSettings settings)
		{
			request.InputSettings = settings;
			return request;
		}
	}
}
