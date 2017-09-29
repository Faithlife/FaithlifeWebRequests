using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Faithlife.Utility;
using Faithlife.Json;
using Faithlife.Utility.Threading;

namespace Faithlife.WebRequests.Json
{
	/// <summary>
	/// JSON web service content.
	/// </summary>
	public class JsonWebServiceContent : HttpContent
	{
		/// <summary>
		/// Creates a new instance of the JsonWebServiceContent class from JSON.
		/// </summary>
		/// <param name="json">The JSON.</param>
		/// <returns>The new instance.</returns>
		public static JsonWebServiceContent FromJson(string json)
		{
			return new JsonWebServiceContent(json);
		}

		/// <summary>
		/// Creates a new instance of the JsonWebServiceContent class from a value to be serialized.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="value">The value.</param>
		/// <returns>The new instance.</returns>
		public static JsonWebServiceContent<TValue> FromValue<TValue>(TValue value)
		{
			return new JsonWebServiceContent<TValue>(value);
		}

		/// <summary>
		/// Creates a new instance of the JsonWebServiceContent class from a value to be serialized.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="settings">The settings.</param>
		/// <returns>The new instance.</returns>
		public static JsonWebServiceContent<TValue> FromValue<TValue>(TValue value, JsonOutputSettings settings)
		{
			return new JsonWebServiceContent<TValue>(value, settings);
		}

		/// <summary>
		/// Gets the JSON.
		/// </summary>
		/// <value>The JSON.</value>
		public string Json
		{
			get { return m_json ?? (m_json = GenerateJson()); }
		}

		/// <summary>
		/// The content type for JSON, i.e. "application/json".
		/// </summary>
		public static readonly string JsonContentType = "application/json";

		/// <summary>
		/// Initializes a new instance of the JsonWebServiceContent class.
		/// </summary>
		protected JsonWebServiceContent()
		{
		}

		/// <summary>
		/// Initializes a new instance of the JsonWebServiceContent class.
		/// </summary>
		/// <param name="json">The JSON.</param>
		protected JsonWebServiceContent(string json)
		{
			m_json = json;
		}

		/// <summary>
		/// Generates the JSON.
		/// </summary>
		/// <returns>The JSON.</returns>
		/// <remarks>Used by derived classes that generate the JSON just in time.</remarks>
		protected virtual string GenerateJson()
		{
			return null;
		}

		protected override async Task<Stream> CreateContentReadStreamAsync()
		{
			MemoryStream stream = new MemoryStream();
			await SerializeToStreamAsync(stream, null).ConfigureAwait(false);
			stream.Position = 0;
			return stream;
		}

		protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
		{
			// don't let StreamWriter close the stream
			using (WrappingStream wrappingStream = new WrappingStream(stream, Ownership.None))
			using (StreamWriter writer = new StreamWriter(wrappingStream))
				await writer.WriteAsync(Json).ConfigureAwait(false);
		}

		protected override bool TryComputeLength(out long length)
		{
			length = Json != null ? Encoding.UTF8.GetByteCount(Json) : 0;

			return Json != null;
		}

		string m_json;
	}

	/// <summary>
	/// JSON web service content.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	public class JsonWebServiceContent<TValue> : JsonWebServiceContent
	{
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		public TValue Value
		{
			get { return m_value; }
		}

		/// <summary>
		/// Initializes a new instance of the JsonWebServiceContent class.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <remarks>JsonUtility.ToJson is called just in time when the JSON content is needed.</remarks>
		protected internal JsonWebServiceContent(TValue value)
		{
			m_value = value;
			Headers.ContentType = new MediaTypeHeaderValue(JsonContentType);
		}

		/// <summary>
		/// Initializes a new instance of the JsonWebServiceContent class.
		/// </summary>
		/// <param name="value">The content.</param>
		/// <param name="settings">The settings.</param>
		/// <remarks>JsonUtility.ToJson is called just in time when the JSON content is needed.</remarks>
		protected internal JsonWebServiceContent(TValue value, JsonOutputSettings settings)
		{
			m_value = value;
			m_outputSettings = settings;
		}

		/// <summary>
		/// Gets the JSON.
		/// </summary>
		/// <value>The JSON.</value>
		protected override string GenerateJson()
		{
			return JsonUtility.ToJson(m_value, m_outputSettings);
		}

		readonly TValue m_value;
		readonly JsonOutputSettings m_outputSettings;
	}
}
