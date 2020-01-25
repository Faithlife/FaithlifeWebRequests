using System;
using Faithlife.Json;

namespace Faithlife.WebRequests.Json
{
	/// <summary>
	/// Client settings for JsonWebServiceClientBase
	/// </summary>
	public class JsonWebServiceClientSettings
	{
		/// <summary>
		/// Gets or sets the request settings.
		/// </summary>
		/// <value>The request settings.</value>
		/// <remarks>Only one of RequestSettings and RequestSettingsCreator can be set.</remarks>
		public WebServiceRequestSettings? RequestSettings { get; set; }

		/// <summary>
		/// Gets or sets the request settings creator.
		/// </summary>
		/// <value>The request settings creator.</value>
		/// <remarks>Only one of RequestSettings and RequestSettingsCreator can be set.</remarks>
		public Func<WebServiceRequestSettings>? RequestSettingsCreator { get; set; }

		/// <summary>
		/// Gets or sets the JSON settings.
		/// </summary>
		/// <value>The JSON settings.</value>
		public JsonSettings? JsonSettings { get; set; }
	}
}
