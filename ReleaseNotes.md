# Release Notes

## 0.11.1

* Fix compatibility with Faithlife.Utility 0.12.0.

## 0.11.0

* Populate `AutoWebServiceResponse.ResponseContentPreview` when creating a `WebServiceException` from the response.

## 0.10.0

* Support additional json content types (e.g., application/vnd.api+json).
* **Breaking:** Content-type must not have extra text after "json" (aside from parameters after a semicolon).

## 0.9.0

* Add `netstandard2.1` target framework.
* Add C# 8 nullable annotations.
* Update dependencies to Faithlife.Utility 0.9.0, Faithlife.Json 0.7.0.

## 0.8.0

* Wrap more exceptions thrown from `WebServiceRequestBase.GetResponseAsync` in `WebServiceException`.
* Rethrow `ArgumentException` thrown from `JsonWebResponseUtility.GetJsonAsAsync` as `WebServiceException`: [#20](https://github.com/Faithlife/FaithlifeWebRequests/issues/20).

## 0.7.1

* Remove `System.Net.Http` dependency on .NET 4.7.2.

## 0.7.0

* Update minimum target frameworks to .NET Standard 2.0, .NET 4.7.2.

## 0.6.1

* Accept json responses without Content-Length set.
* Detach `WebServiceResponse.Content` to allow it to be read as a stream.

## 0.6.0

* Add `WebServiceRequestSettings.GetHttpClient` to allow `HttpClient` reuse.

## 0.5.0

* Support responses with an explicit `StatusCode` property.
* Add `WebServiceRequestSettings.StartTrace` callback.

## 0.4.0

* Add `params string[]` option to `CreateRequest`.
* **Breaking:** Adapt to breaking changes in `Faithlife.Utility`.

## 0.3.0

* Change the only ConfigureAwait(true) to ConfigureAwait(false).
* Add If-Match header support.

## 0.2.0

* **Breaking:** Adapt to breaking changes in `Faithlife.Json`.
  * `JsonInputSettings` and `JsonOutputSettings` were merged into `JsonSettings`.
  * `GetRequestUri` now uses `IEnumerable<KeyValuePair<string, object>>` instead of an anonymous `object`.
* **Breaking:** Change `AcceptedStatusCodes` to use `IReadOnlyList<HttpStatusCode>`.

## 0.1.2

* Fix cookie handling in `WebServiceRequestBase`.

## 0.1.1

* Update to Faithlife.Json 0.1.1.

## 0.1.0

* Initial release.
