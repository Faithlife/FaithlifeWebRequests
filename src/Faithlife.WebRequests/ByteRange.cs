using System;

namespace Faithlife.WebRequests
{
	/// <summary>
	/// Represents a byte range for the Range header in an HttpRequestMessage.
	/// </summary>
	public class ByteRange
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ByteRange"/> class.
		/// </summary>
		/// <param name="from">The start of the byte range with no definite end.</param>
		public ByteRange(long from)
		{
			if (from < 0)
				throw new ArgumentOutOfRangeException(nameof(from), from, "The parameter must be a non-negative number.");

			From = from;
			m_to = null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ByteRange"/> class.
		/// </summary>
		/// <param name="from">The start of the byte range.</param>
		/// <param name="to">The end of the byte range.</param>
		public ByteRange(long from, long to)
		{
			if (from < 0)
				throw new ArgumentOutOfRangeException(nameof(from), from, "The parameter must be a non-negative number.");
			if (to < 0)
				throw new ArgumentOutOfRangeException(nameof(to), to, "The parameter must be a non-negative number.");
			if (from > to)
				throw new ArgumentOutOfRangeException(nameof(from), "from cannot be greater than to.");

			From = from;
			m_to = to;
		}

		/// <summary>
		/// Start of the byte range.
		/// </summary>
		public long From { get; }

		/// <summary>
		/// End of the byte range.
		/// </summary>
		public long To => m_to ?? throw new InvalidOperationException("The byte range does not have an end value.");

		/// <summary>
		/// Whether or not the byte range has a definite end.
		/// </summary>
		public bool HasEnd => m_to.HasValue;

		private readonly long? m_to;
	}
}
