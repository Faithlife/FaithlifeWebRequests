using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Faithlife.Json;
using Faithlife.WebRequests.Json;
using NUnit.Framework;

namespace Faithlife.WebRequests.Tests
{
	[TestFixture]
	public sealed class EchoTests
	{
		[OneTimeSetUp]
		public void CreateEchoServer()
		{
			var random = new Random();
			for (int i = 0; ; i++)
			{
				var port = random.Next(20000, 30000);
				var prefix = $"http://localhost:{port}/";
				try
				{
					m_listener = new HttpListener();
					m_listener.Prefixes.Add(prefix);
					m_listener.Start();
					m_uriPrefix = prefix;
					break;
				}
				catch (HttpListenerException ex) when (ex.ErrorCode == 48)
				{
					// Address already in use
					// This can happen with repeated runs in quick succession on macOS.
					if (i == 10)
						throw;
				}
			}

			m_listenerCts = new CancellationTokenSource();
			var cancellationToken = m_listenerCts.Token;
			m_listenerTask = Task.Run(async () =>
			{
				var waitIndefinitely = Task.Delay(-1, cancellationToken);
				while (m_listener.IsListening)
				{
					var getContext = m_listener.GetContextAsync();
					await Task.WhenAny(getContext, waitIndefinitely);
					cancellationToken.ThrowIfCancellationRequested();
					var context = await getContext;
					EchoRequestDto requestDto = null;
					if (context.Request.HasEntityBody && context.Request.ContentType == "application/json")
					{
						using (var inputStream = context.Request.InputStream)
						using (var textReader = new StreamReader(inputStream))
							requestDto = JsonUtility.FromJsonTextReader<EchoRequestDto>(textReader);
					}
					context.Response.StatusCode = requestDto?.StatusCode ?? 200;
					if (requestDto?.SendContent != false)
					{
						var responseDto = new EchoResponseDto
						{
							Method = context.Request.HttpMethod,
							Path = context.Request.Url.AbsolutePath,
							Query = context.Request.Url.Query,
							RequestHeaders = context.Request.Headers.Cast<string>()
								.ToDictionary(name => name, name => string.Join("; ", context.Request.Headers.GetValues(name))),
							Message = requestDto?.Message,
						};
						context.Response.ContentType = "application/json";
						JsonUtility.ToJsonStream(responseDto, context.Response.OutputStream);
					}
					context.Response.Close();
				}
			});
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			m_listener.Stop();
			m_listenerCts.Cancel();
		}

		[Test]
		public async Task EchoWithMessageAsync()
		{
			var request = new AutoWebServiceRequest<EchoResponseResponse>(new Uri(m_uriPrefix + "echo"))
				.WithPostMethod()
				.WithJsonContent(new EchoRequestDto { Message = "hello" });
			var response = await request.GetResponseAsync();
			Assert.IsNotNull(response.OK);
			Assert.AreEqual("POST", response.OK.Method);
			Assert.AreEqual("hello", response.OK.Message);
		}

		[Test]
		public async Task EchoWithAcceptHeaderAsync()
		{
			var request = new AutoWebServiceRequest<EchoResponseResponse>(new Uri(m_uriPrefix + "echo"))
				.WithAccept("text/plain");
			var response = await request.GetResponseAsync();
			Assert.IsNotNull(response.OK);
			Assert.AreEqual("text/plain", response.OK.RequestHeaders.GetValueOrDefault("Accept"));
		}

		[Test]
		public async Task EchoNoContentAsync()
		{
			var request = new AutoWebServiceRequest<EchoResponseResponse>(new Uri(m_uriPrefix + "echo"))
				.WithJsonContent(new EchoRequestDto { StatusCode = 204, SendContent = false });
			var response = await request.GetResponseAsync();
			Assert.IsTrue(response.NoContent);
		}

		[Test]
		public void EchoInternalError()
		{
			var request = new AutoWebServiceRequest<EchoResponseResponse>(new Uri(m_uriPrefix + "echo"))
				.WithJsonContent(new EchoRequestDto { StatusCode = 500 });
			var exception = Assert.ThrowsAsync<WebServiceException>(async () => await request.GetResponseAsync());
			Assert.AreEqual(HttpStatusCode.InternalServerError, exception.ResponseStatusCode);
		}

		[TestCase(HttpStatusCode.OK)]
		[TestCase(HttpStatusCode.Created)]
		[TestCase(HttpStatusCode.BadRequest)]
		[TestCase(HttpStatusCode.Unauthorized)]
		[TestCase(HttpStatusCode.Forbidden)]
		[TestCase(HttpStatusCode.NotFound)]
		[TestCase(HttpStatusCode.InternalServerError)]
		[TestCase(HttpStatusCode.BadGateway)]
		[TestCase(HttpStatusCode.ServiceUnavailable)]
		[TestCase(HttpStatusCode.GatewayTimeout)]
		public async Task EchoGenericStatusCodeResponse(HttpStatusCode statusCode)
		{
			var request = new AutoWebServiceRequest<GenericStatusCodeResponse>(new Uri(m_uriPrefix + "echo"))
				.WithJsonContent(new EchoRequestDto { StatusCode = (int) statusCode });

			var response = await request.GetResponseAsync();
			Assert.AreEqual(statusCode, response.StatusCode);
			CollectionAssert.IsNotEmpty(response.Headers);
		}

		[TestCase(HttpStatusCode.OK)]
		[TestCase(HttpStatusCode.Created)]
		[TestCase(HttpStatusCode.BadRequest)]
		[TestCase(HttpStatusCode.Unauthorized)]
		[TestCase(HttpStatusCode.Forbidden)]
		[TestCase(HttpStatusCode.NotFound)]
		[TestCase(HttpStatusCode.InternalServerError)]
		[TestCase(HttpStatusCode.BadGateway)]
		[TestCase(HttpStatusCode.ServiceUnavailable)]
		[TestCase(HttpStatusCode.GatewayTimeout)]
		public async Task EchoGenericStatusCodeAsIntResponse(HttpStatusCode statusCode)
		{
			var request = new AutoWebServiceRequest<EchoResponseGenericStatusCodeAsIntResponse>(new Uri(m_uriPrefix + "echo"))
				.WithJsonContent(new EchoRequestDto { StatusCode = (int) statusCode });

			var response = await request.GetResponseAsync();
			Assert.AreEqual((int) statusCode, response.StatusCode);
		}

		class EchoRequestDto
		{
			public string Message { get; set; }
			public int? StatusCode { get; set; }
			public bool? SendContent { get; set; }
		}

		class EchoResponseDto
		{
			public string Method { get; set; }
			public string Path { get; set; }
			public string Query { get; set; }
			public Dictionary<string, string> RequestHeaders { get; set; }
			public string Message { get; set; }
		}

		class EchoResponseResponse
		{
			public EchoResponseDto OK { get; set; }
			public bool NoContent { get; set; }
		}

		class EchoResponseGenericStatusCodeAsIntResponse : AutoWebServiceResponse
		{
			public int StatusCode { get; internal set; }
		}

		HttpListener m_listener;
		string m_uriPrefix;
		Task m_listenerTask;
		CancellationTokenSource m_listenerCts;
	}
}
