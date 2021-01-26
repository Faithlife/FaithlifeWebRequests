using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Faithlife.Json;
using Faithlife.Utility;

namespace Faithlife.WebRequests.Json
{
	/// <summary>
	/// A web service request whose response is initialized via reflection.
	/// </summary>
	/// <typeparam name="TResponse">The type of the response.</typeparam>
	/// <remarks><para>AutoWebServiceRequest automatically sets properties on an instance of TResponse
	/// when handling the web service response.</para>
	/// <para>First, a property is located whose name matches the name of the returned status code. If that
	/// property is Boolean, it is set to true. If the response content is JSON, it is deserialized into the property
	/// according to the property type. If no such property is found on TResponse, a WebServiceException is
	/// thrown.</para>
	/// <para>After the status code is handled, the response headers are deserialized into properties that match
	/// the header names without the hyphens. If the property is a string, it is set to the header value without
	/// modification; other property types will cause the header value to be parsed accordingly, e.g. Int32, Int64,
	/// Uri, DateTime (RFC1123), LanguageName, and byte[] (base-64). Response headers without corresponding
	/// properties are ignored.</para></remarks>
	public class AutoWebServiceRequest<TResponse> : WebServiceRequestBase<TResponse>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AutoWebServiceRequest&lt;TResponse&gt;"/> class.
		/// </summary>
		/// <param name="uri">The URI.</param>
		public AutoWebServiceRequest(Uri uri)
			: base(uri)
		{
		}

		/// <summary>
		/// Gets or sets the JSON settings.
		/// </summary>
		/// <value>The json settings.</value>
		public JsonSettings? JsonSettings { get; set; }

		/// <summary>
		/// Creates an empty response.
		/// </summary>
		/// <returns>An empty response.</returns>
		/// <remarks>The default implementation uses the default constructor.</remarks>
		protected virtual TResponse CreateResponse() => Activator.CreateInstance<TResponse>();

		/// <summary>
		/// Overrides HandleResponseCore.
		/// </summary>
		protected override async Task<bool> HandleResponseCoreAsync(WebServiceResponseHandlerInfo<TResponse> info)
		{
			var response = CreateResponse()!;
			Type responseType = response.GetType();
			var webResponse = info.WebResponse!;

			// find specific status code property
			bool isStatusCodeHandled = false;
			HttpStatusCode statusCode = webResponse.StatusCode;
			string statusCodeText = statusCode.ToString();
			var resultProperty = GetProperty(responseType, statusCodeText);
			object? content = null;

			if (resultProperty?.CanWrite ?? false)
			{
				Type resultPropertyType = resultProperty.PropertyType;
				content = await ReadContentAsAsync(info, resultPropertyType).ConfigureAwait(false);
				resultProperty.SetValue(response, content, null);
				isStatusCodeHandled = true;
			}

			var statusCodeProperty = GetProperty(responseType, "StatusCode");
			if (statusCodeProperty?.CanWrite ?? false)
			{
				Type statusCodePropertyType = statusCodeProperty.PropertyType;
				if (statusCodePropertyType == typeof(HttpStatusCode))
					statusCodeProperty.SetValue(response, webResponse.StatusCode, null);
				else if (statusCodePropertyType == typeof(int))
					statusCodeProperty.SetValue(response, (int) webResponse.StatusCode, null);
				else
					throw await CreateExceptionAsync(info, "Web response status code cannot be read as {0}.".FormatInvariant(statusCodePropertyType)).ConfigureAwait(false);

				isStatusCodeHandled = true;
			}

			// make sure status code is handled
			if (!isStatusCodeHandled)
				throw await CreateExceptionAsync(info, "Status code not handled.").ConfigureAwait(false);

			// read headers
			foreach (var header in webResponse.Headers)
			{
				// remove hyphens before looking for property setter by name
				string headerName = header.Key;
				string propertyName = headerName.Replace("-", "");
				var headerProperty = GetProperty(responseType, propertyName);
				if (headerProperty?.CanWrite ?? false)
				{
					// get header text
					string headerText = header.Value.Join("; ");

					// convert header text to supported types
					Type headerPropertyType = headerProperty.PropertyType;
					if (headerPropertyType == typeof(string))
						headerProperty.SetValue(response, headerText, null);
					else if (headerPropertyType == typeof(int) || headerPropertyType == typeof(int?))
						headerProperty.SetValue(response, InvariantConvert.ParseInt32(headerText), null);
					else if (headerPropertyType == typeof(long) || headerPropertyType == typeof(long?))
						headerProperty.SetValue(response, InvariantConvert.ParseInt64(headerText), null);
					else if (headerPropertyType == typeof(Uri))
						headerProperty.SetValue(response, new Uri(webResponse.RequestMessage.RequestUri, headerText), null);
					else if (headerPropertyType == typeof(DateTime) || headerPropertyType == typeof(DateTime?))
						headerProperty.SetValue(response, DateTime.ParseExact(headerText, "R", CultureInfo.InvariantCulture), null);
					else if (headerPropertyType == typeof(byte[]))
						headerProperty.SetValue(response, Convert.FromBase64String(headerText), null);
					else
						throw await CreateExceptionAsync(info, "Web response header cannot be read as {0}. {1}: {2}".FormatInvariant(headerPropertyType, headerName, headerText)).ConfigureAwait(false);
				}
			}

			// allow response to read extra data
			if (response is AutoWebServiceResponse autoWebServiceResponse)
				await autoWebServiceResponse.OnResponseHandledAsync(info).ConfigureAwait(false);

			// detach response if necessary
			if (content is WebResponseStream)
				info.DetachWebResponse();

			// success
			info.Response = response;
			return true;
		}

		private static PropertyInfo? GetProperty(Type type, string propertyName)
		{
			var property = type.GetRuntimeProperties().FirstOrDefault(x => x.GetMethod is object && !x.GetMethod.IsStatic && string.Equals(x.Name, propertyName, StringComparison.OrdinalIgnoreCase));

			if (property is object && property.SetMethod is null)
				property = property.DeclaringType.GetRuntimeProperty(property.Name);

			return property;
		}

		private async Task<object> ReadContentAsAsync(WebServiceResponseHandlerInfo<TResponse> info, Type propertyType)
		{
			var webResponse = info.WebResponse!;
			object? content = null;

			if (propertyType == typeof(bool) || propertyType == typeof(bool?))
			{
				content = true;
			}
			else if (propertyType == typeof(string))
			{
				info.MarkContentAsRead();
				content = await webResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
			}
			else if (propertyType == typeof(byte[]))
			{
				info.MarkContentAsRead();
				content = await webResponse.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
			}
			else if (propertyType == typeof(Stream))
			{
				info.MarkContentAsRead();
				content = await WebResponseStream.CreateAsync(webResponse).ConfigureAwait(false);
			}
			else if (webResponse.HasJson() || webResponse.Content.Headers.ContentType is null)
			{
				info.MarkContentAsRead();
				content = await webResponse.GetJsonAsAsync(propertyType, JsonSettings).ConfigureAwait(false);
			}

			if (content is null)
				throw await CreateExceptionAsync(info, "Web response content cannot be read as {0}.".FormatInvariant(propertyType)).ConfigureAwait(false);

			return content;
		}

		private static Task<WebServiceException> CreateExceptionAsync(WebServiceResponseHandlerInfo info, string message)
		{
			if (info.IsContentRead)
			{
				return Task.FromResult(HttpResponseMessageUtility.CreateWebServiceException(info.WebResponse!, message));
			}
			else
			{
				info.MarkContentAsRead();
				return HttpResponseMessageUtility.CreateWebServiceExceptionWithContentPreviewAsync(info.WebResponse!, message);
			}
		}

		private sealed class WebResponseStream : WrappingStreamBase
		{
			public static async Task<WebResponseStream> CreateAsync(HttpResponseMessage webResponse)
			{
				var stream = await webResponse.Content.ReadAsStreamAsync().ConfigureAwait(false);
				return new WebResponseStream(webResponse, stream);
			}

			private WebResponseStream(HttpResponseMessage webResponse, Stream stream)
				: base(stream, Ownership.Owns)
			{
				m_webResponse = webResponse;
			}

			public override int Read(byte[] buffer, int offset, int count)
				=> WrappedStream.Read(buffer, offset, count);

			public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
				=> WrappedStream.ReadAsync(buffer, offset, count, cancellationToken);

			public override void Write(byte[] buffer, int offset, int count)
				=> throw new NotSupportedException();

			public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
				=> throw new NotSupportedException();

			protected override void Dispose(bool disposing)
			{
				try
				{
					if (disposing)
						m_webResponse.Dispose();
				}
				finally
				{
					base.Dispose(disposing);
				}
			}

			private readonly HttpResponseMessage m_webResponse;
		}
	}
}
