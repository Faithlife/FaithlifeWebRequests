using Faithlife.WebRequests.Json;
using NUnit.Framework;

namespace Faithlife.WebRequests.Tests
{
	[TestFixture]
	public sealed class JsonResponseUtilityTests
	{
		[TestCase(true, "application/json")]
		[TestCase(true, " application/json")]
		[TestCase(true, " application/vnd.api+json")]
		[TestCase(true, "application/vnd.api+json")]
		[TestCase(true, "application/vnd.oracle.resource+json; type=singular; charset=UTF-8")]
		[TestCase(false, "application/geo+json-seq")]
		[TestCase(false, "text/html")]
		[TestCase(false, "text/html; charset=UTF-8")]
		[TestCase(false, "text/html; charset=application/json")]
		[TestCase(false, "application/html; charset=application/json")]
		[TestCase(false, "application/html; charset=application/vnd.api+json")]
		public void TestIsJsonContentType(bool isJson, string contentType)
		{
			Assert.AreEqual(isJson, JsonResponseUtility.IsJsonContentType(contentType));
		}
	}
}
