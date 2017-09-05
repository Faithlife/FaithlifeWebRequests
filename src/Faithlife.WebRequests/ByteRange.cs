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
				throw new ArgumentOutOfRangeException("from", from, "The parameter must be a non-negative number.");

			m_from = from;
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
				throw new ArgumentOutOfRangeException("from", from, "The parameter must be a non-negative number.");
			if (to < 0)
				throw new ArgumentOutOfRangeException("to", to, "The parameter must be a non-negative number.");
			if (from > to)
				throw new ArgumentOutOfRangeException("from", "from cannot be greater than to.");

			m_from = from;
			m_to = to;
		}

		/// <summary>
		/// Start of the byte range.
		/// </summary>
		public long From
		{
			get { return m_from; }
		}

		/// <summary>
		/// End of the byte range.
		/// </summary>
		public long To
		{
			get
			{
				if (m_to == null)
					throw new InvalidOperationException("The byte range does not have an end value.");

				return m_to.Value;
			}
		}

		/// <summary>
		/// Whether or not the byte range has a definite end.
		/// </summary>
		public bool HasEnd
		{
			get { return m_to != null; }
		}

		readonly long m_from;
		readonly long? m_to;
	}
}
