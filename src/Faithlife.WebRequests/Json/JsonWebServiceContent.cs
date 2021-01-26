using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Faithlife.Json;
using Faithlife.Utility;

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
		public static JsonWebServiceContent FromJson(string json) => new JsonWebServiceContent(json);

		/// <summary>
		/// Creates a new instance of the JsonWebServiceContent class from a value to be serialized.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="value">The value.</param>
		/// <returns>The new instance.</returns>
		public static JsonWebServiceContent<TValue> FromValue<TValue>([AllowNull] TValue value) =>
			new JsonWebServiceContent<TValue>(value);

		/// <summary>
		/// Creates a new instance of the JsonWebServiceContent class from a value to be serialized.
		/// </summary>
		/// <typeparam name="TValue">The type of the value.</typeparam>
		/// <param name="value">The value.</param>
		/// <param name="settings">The settings.</param>
		/// <returns>The new instance.</returns>
		public static JsonWebServiceContent<TValue> FromValue<TValue>(TValue value, JsonSettings? settings) =>
			new JsonWebServiceContent<TValue>(value, settings);

		/// <summary>
		/// Gets the JSON.
		/// </summary>
		/// <value>The JSON.</value>
		public string? Json => m_json ?? (m_json = GenerateJson());

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
		protected virtual string? GenerateJson()
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

		protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
		{
			// don't let StreamWriter close the stream
			using var wrappingStream = new WrappingStream(stream, Ownership.None);
			using var writer = new StreamWriter(wrappingStream);
			await writer.WriteAsync(Json).ConfigureAwait(false);
		}

		protected override bool TryComputeLength(out long length)
		{
			length = Json is object ? Encoding.UTF8.GetByteCount(Json) : 0;
			return Json is object;
		}

		private string? m_json;
	}

	/// <summary>
	/// JSON web service content.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:File may only contain a single type", Justification = "Generic.")]
	public class JsonWebServiceContent<TValue> : JsonWebServiceContent
	{
		/// <summary>
		/// Gets the value.
		/// </summary>
		/// <value>The value.</value>
		[AllowNull]
		public TValue Value { get; }

		/// <summary>
		/// Initializes a new instance of the JsonWebServiceContent class.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <remarks>JsonUtility.ToJson is called just in time when the JSON content is needed.</remarks>
		protected internal JsonWebServiceContent([AllowNull] TValue value)
		{
			Value = value;
			Headers.ContentType = new MediaTypeHeaderValue(JsonContentType);
		}

		/// <summary>
		/// Initializes a new instance of the JsonWebServiceContent class.
		/// </summary>
		/// <param name="value">The content.</param>
		/// <param name="settings">The settings.</param>
		/// <remarks>JsonUtility.ToJson is called just in time when the JSON content is needed.</remarks>
		protected internal JsonWebServiceContent(TValue value, JsonSettings? settings)
		{
			Value = value;
			m_outputSettings = settings;
		}

		/// <summary>
		/// Gets the JSON.
		/// </summary>
		/// <value>The JSON.</value>
		protected override string GenerateJson() => JsonUtility.ToJson(Value, m_outputSettings);

		private readonly JsonSettings? m_outputSettings;
	}
}
