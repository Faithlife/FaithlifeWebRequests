using System;
using System.Collections.Generic;
using System.Linq;
using Faithlife.WebRequests.Json;
using NUnit.Framework;

namespace Faithlife.WebRequests.Tests
{
	[TestFixture]
	public sealed class CreateRequestTests
	{
		[Test]
		public void CreateRequestWithIEnumerableParameters()
		{
			var client = CreateMockClient();

			var parameters = new Dictionary<string, object?>();
			parameters.Add("id", "1234");
			parameters.Add("test", "1");

			var request = client.CreateRequest<object>("tests/{id}", parameters.ToList());

			Assert.AreEqual(request.RequestUri.AbsoluteUri, "https://testapi.faithlife.com/v1/tests/1234?test=1");
		}

		[Test]
		public void CreateRequestWithStringParams()
		{
			var client = CreateMockClient();

			var request = client.CreateRequest<object>("tests/{id}", "id", "1234", "test", "1");

			Assert.AreEqual(request.RequestUri.AbsoluteUri, "https://testapi.faithlife.com/v1/tests/1234?test=1");
		}

		private MockWebServiceClient CreateMockClient()
		{
			return new MockWebServiceClient(s_baseUri, new JsonWebServiceClientSettings());
		}

		private sealed class MockWebServiceClient : JsonWebServiceClientBase
		{
			public MockWebServiceClient(Uri baseUri, JsonWebServiceClientSettings clientSettings)
				: base(baseUri, clientSettings)
			{
			}

			public new AutoWebServiceRequest<TResponse> CreateRequest<TResponse>(string relativeUriPattern, IEnumerable<KeyValuePair<string, object?>> uriParameters)
			{
				return base.CreateRequest<TResponse>(relativeUriPattern, uriParameters);
			}

			public new AutoWebServiceRequest<TResponse> CreateRequest<TResponse>(string relativeUriPattern, params string[] parameters)
			{
				return base.CreateRequest<TResponse>(relativeUriPattern, parameters);
			}
		}

		private static readonly Uri s_baseUri = new Uri("https://testapi.faithlife.com/v1/");
	}
}
