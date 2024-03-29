# JsonWebServiceClientBase class

A base class for common json service clients.

```csharp
public abstract class JsonWebServiceClientBase
```

## Protected Members

| name | description |
| --- | --- |
| [JsonWebServiceClientBase](JsonWebServiceClientBase/JsonWebServiceClientBase.md)(…) | Initializes a new instance of the [`JsonWebServiceClientBase`](./JsonWebServiceClientBase.md) class. |
| [CreateRequest&lt;TResponse&gt;](JsonWebServiceClientBase/CreateRequest.md)() | Creates a new AutoWebServiceRequest. |
| [CreateRequest&lt;TResponse&gt;](JsonWebServiceClientBase/CreateRequest.md)(…) | Creates a new AutoWebServiceRequest using the specified relative URI. (3 methods) |
| [GetRequestUri](JsonWebServiceClientBase/GetRequestUri.md)() | Creates a web request URI. |
| [GetRequestUri](JsonWebServiceClientBase/GetRequestUri.md)(…) | Creates a web request URI using the specified relative URI. (3 methods) |
| virtual [OnGetRequestUri](JsonWebServiceClientBase/OnGetRequestUri.md)(…) | Called to modify the request URI before it is sent. |
| virtual [OnRequestCreated](JsonWebServiceClientBase/OnRequestCreated.md)(…) | Called to modify the request before it is sent. |

## See Also

* namespace [Faithlife.WebRequests.Json](../Faithlife.WebRequests.md)
* [JsonWebServiceClientBase.cs](https://github.com/Faithlife/FaithlifeWebRequests/tree/master/src/Faithlife.WebRequests/Json/JsonWebServiceClientBase.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.WebRequests.dll -->
