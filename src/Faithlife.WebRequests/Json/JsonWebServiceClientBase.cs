using System;
using System.Collections.Generic;
using Faithlife.Utility;

namespace Faithlife.WebRequests.Json
{
	/// <summary>
	/// A base class for common json service clients.
	/// </summary>
	public abstract class JsonWebServiceClientBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsonWebServiceClientBase"/> class.
		/// </summary>
		/// <param name="baseUri">The base URI.</param>
		/// <param name="clientSettings">The client settings.</param>
		protected JsonWebServiceClientBase(Uri baseUri, JsonWebServiceClientSettings clientSettings)
		{
			if (baseUri is null)
				throw new ArgumentNullException("baseUri");
			if (clientSettings is null)
				throw new ArgumentNullException("clientSettings");
			if (clientSettings.RequestSettings is object && clientSettings.RequestSettingsCreator is object)
				throw new ArgumentException("Only one of RequestSettings and RequestSettingsCreator may be set.", "clientSettings");

			m_baseUri = baseUri;
			m_clientSettings = clientSettings;
		}

		/// <summary>
		/// Creates a web request URI.
		/// </summary>
		/// <returns>The web request URI.</returns>
		protected Uri GetRequestUri() => DoGetRequestUri(null);

		/// <summary>
		/// Creates a web request URI using the specified relative URI.
		/// </summary>
		/// <param name="relativeUri">The relative URI.</param>
		/// <returns>The web request URI.</returns>
		protected Uri GetRequestUri(string relativeUri) => DoGetRequestUri(relativeUri ?? throw new ArgumentNullException(nameof(relativeUri)));

		/// <summary>
		/// Creates a web request URI using the specified relative URI pattern and parameters.
		/// </summary>
		/// <param name="relativeUriPattern">The relative URI pattern.</param>
		/// <param name="uriParameters">The URI parameters.</param>
		/// <returns>The web request URI.</returns>
		/// <remarks>See UriUtility.FromPattern for acceptable parameter values.</remarks>
		protected Uri GetRequestUri(string relativeUriPattern, IEnumerable<KeyValuePair<string, object?>> uriParameters) =>
			DoGetRequestUri(relativeUriPattern ?? throw new ArgumentNullException(nameof(relativeUriPattern)), uriParameters ?? throw new ArgumentNullException(nameof(uriParameters)));

		/// <summary>
		/// Creates a web request URI using the specified relative URI pattern and parameters.
		/// </summary>
		/// <param name="relativeUriPattern">The relative URI pattern.</param>
		/// <param name="parameters">The URI parameters.</param>
		/// <returns>The web request URI.</returns>
		/// <remarks>Each pair of parameters represents a key and a value. See UriUtility.FromPattern for acceptable parameter values.</remarks>
		protected Uri GetRequestUri(string relativeUriPattern, params string[] parameters) =>
			DoGetRequestUri(relativeUriPattern ?? throw new ArgumentNullException(nameof(relativeUriPattern)), parameters);

		/// <summary>
		/// Creates a new AutoWebServiceRequest.
		/// </summary>
		/// <typeparam name="TResponse">The type of the response.</typeparam>
		/// <returns>The new AutoWebServiceRequest.</returns>
		protected AutoWebServiceRequest<TResponse> CreateRequest<TResponse>() =>
			DoCreateRequest<TResponse>(GetRequestUri());

		/// <summary>
		/// Creates a new AutoWebServiceRequest using the specified relative URI.
		/// </summary>
		/// <typeparam name="TResponse">The type of the response.</typeparam>
		/// <param name="relativeUri">The relative URI.</param>
		/// <returns>The new AutoWebServiceRequest.</returns>
		protected AutoWebServiceRequest<TResponse> CreateRequest<TResponse>(string relativeUri) =>
			DoCreateRequest<TResponse>(GetRequestUri(relativeUri));

		/// <summary>
		/// Creates a new AutoWebServiceRequest using the specified relative URI pattern and parameters.
		/// </summary>
		/// <typeparam name="TResponse">The type of the response.</typeparam>
		/// <param name="relativeUriPattern">The relative URI pattern.</param>
		/// <param name="uriParameters">The URI parameters.</param>
		/// <returns>The new AutoWebServiceRequest.</returns>
		/// <remarks>See UriUtility.FromPattern for acceptable parameter values.</remarks>
		protected AutoWebServiceRequest<TResponse> CreateRequest<TResponse>(string relativeUriPattern, IEnumerable<KeyValuePair<string, object?>> uriParameters) =>
			DoCreateRequest<TResponse>(GetRequestUri(relativeUriPattern, uriParameters));

		/// <summary>
		/// Creates a new AutoWebServiceRequest using the specified relative URI pattern and parameters.
		/// </summary>
		/// <typeparam name="TResponse">The type of the response.</typeparam>
		/// <param name="relativeUriPattern">The relative URI pattern.</param>
		/// <param name="parameters">The URI parameters.</param>
		/// <returns>The new AutoWebServiceRequest.</returns>
		/// <remarks>Each pair of parameters represents a key and a value. See UriUtility.FromPattern for acceptable parameter values.</remarks>
		protected AutoWebServiceRequest<TResponse> CreateRequest<TResponse>(string relativeUriPattern, params string[] parameters) =>
			DoCreateRequest<TResponse>(GetRequestUri(relativeUriPattern, parameters));

		/// <summary>
		/// Called to modify the request URI before it is sent.
		/// </summary>
		/// <param name="uri">The request URI.</param>
		protected virtual void OnGetRequestUri(ref Uri uri)
		{
		}

		/// <summary>
		/// Called to modify the request before it is sent.
		/// </summary>
		/// <param name="request">The WebServiceRequestBase</param>
		protected virtual void OnRequestCreated(WebServiceRequestBase request)
		{
		}

		private Uri DoGetRequestUri(string relativeUriPattern, IEnumerable<KeyValuePair<string, object?>> uriParameters)
		{
			string uriText = m_baseUri.AbsoluteUri;

			if (!string.IsNullOrEmpty(relativeUriPattern))
				uriText = uriText.TrimEnd('/') + "/" + relativeUriPattern.TrimStart('/');

			Uri uri = uriParameters is object ? UriUtility.FromPattern(uriText, uriParameters) : new Uri(uriText);

			OnGetRequestUri(ref uri);

			return uri;
		}

		private Uri DoGetRequestUri(string? relativeUriPattern, params string[] parameters)
		{
			string uriText = m_baseUri.AbsoluteUri;

			if (!string.IsNullOrEmpty(relativeUriPattern))
				uriText = uriText.TrimEnd('/') + "/" + relativeUriPattern!.TrimStart('/');

			Uri uri = parameters?.Length > 0 ? UriUtility.FromPattern(uriText, parameters) : new Uri(uriText);

			OnGetRequestUri(ref uri);

			return uri;
		}

		private AutoWebServiceRequest<TResponse> DoCreateRequest<TResponse>(Uri uri)
		{
			var requestSettings = m_clientSettings.RequestSettings ?? m_clientSettings.RequestSettingsCreator?.Invoke();

			AutoWebServiceRequest<TResponse> request = new AutoWebServiceRequest<TResponse>(uri) { JsonSettings = m_clientSettings.JsonSettings }.WithSettings(requestSettings);

			OnRequestCreated(request);

			return request;
		}

		private readonly Uri m_baseUri;
		private readonly JsonWebServiceClientSettings m_clientSettings;
	}
}
