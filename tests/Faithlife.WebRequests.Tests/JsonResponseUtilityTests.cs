using Faithlife.WebRequests.Json;
using NUnit.Framework;

namespace Faithlife.WebRequests.Tests
{
	[TestFixture]
	public sealed class JsonResponseUtilityTests
	{
		[TestCase("application/json", true)]
		[TestCase(" application/json", true)]
		[TestCase(" application/vnd.api+json", true)]
		[TestCase("application/vnd.api+json", true)]
		[TestCase("text/html", false)]
		[TestCase("text/html; charset=UTF-8", false)]
		[TestCase("text/html; charset=application/json", false)]
		[TestCase("application/html; charset=application/json", false)]
		[TestCase("application/html; charset=application/vnd.api+json", false)]
		public void TestIsJsonContentType(string contentType, bool isJson)
		{
			Assert.AreEqual(isJson, JsonResponseUtility.IsJsonContentType(contentType));
		}
	}
}
