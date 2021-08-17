using System;
using System.Net;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// Provides utility methods for working with <see cref="WebHeaderCollection"/>.
	/// </summary>
	public static class WebHeaderCollectionUtility
	{
		/// <summary>
		/// Adds headers to an existing header collection.
		/// </summary>
		public static void Add(this WebHeaderCollection headers, WebHeaderCollection additionalHeaders)
		{
			foreach (string name in additionalHeaders.AllKeys)
				headers.Add(name, additionalHeaders[name]);
		}

		/// <summary>
		/// Adds a header to an existing header collection.
		/// </summary>
		public static void Add(this WebHeaderCollection headers, string name, string value)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("Invalid name", nameof(name));
			if (value?.Length > 65535)
				throw new ArgumentOutOfRangeException(nameof(value));

			string currentValue = headers[name];
			headers[name] = currentValue is null ? value : currentValue + ", " + value;
		}

		/// <summary>
		/// Adds a header to an existing header collection.
		/// </summary>
		public static void Add(this WebHeaderCollection headers, HttpRequestHeader header, string value)
		{
			if (value?.Length > 65535)
				throw new ArgumentOutOfRangeException(nameof(value));

			string currentValue = headers[header];
			headers[header] = currentValue is null ? value : currentValue + ", " + value;
		}
	}
}
