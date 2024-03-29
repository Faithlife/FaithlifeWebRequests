# JsonWebServiceRequest&lt;TResponseValue&gt; class

A JSON web service request.

```csharp
public class JsonWebServiceRequest<TResponseValue> : JsonWebServiceRequest
```

| parameter | description |
| --- | --- |
| TResponseValue | The type of the response value. |

## Public Members

| name | description |
| --- | --- |
| [JsonWebServiceRequest](JsonWebServiceRequest-1/JsonWebServiceRequest.md)(…) | Initializes a new instance of the [`JsonWebServiceRequest`](./JsonWebServiceRequest-1.md) class. |
| [JsonSettings](JsonWebServiceRequest-1/JsonSettings.md) { get; set; } | Gets or sets the JSON settings. |
| [GetResponseAsync](JsonWebServiceRequest-1/GetResponseAsync.md)(…) | Gets the response asynchronously. |

## Protected Members

| name | description |
| --- | --- |
| override [CreateResponseAsync](JsonWebServiceRequest-1/CreateResponseAsync.md)(…) | Called to create the response. |

## See Also

* class [JsonWebServiceRequest](./JsonWebServiceRequest.md)
* namespace [Faithlife.WebRequests.Json](../Faithlife.WebRequests.md)
* [JsonWebServiceRequest.cs](https://github.com/Faithlife/FaithlifeWebRequests/tree/master/src/Faithlife.WebRequests/Json/JsonWebServiceRequest.cs)

<!-- DO NOT EDIT: generated by xmldocmd for Faithlife.WebRequests.dll -->
