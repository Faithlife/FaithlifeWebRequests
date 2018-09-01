# Version History

## Pending

Add changes here when they're committed to the `master` branch. Move them to "Released" once the version number
is updated in preparation for publishing an updated NuGet package.

Prefix the description of the change with `[major]`, `[minor]` or `[patch]` in accordance with [SemVer](http://semver.org).

* [patch] Accept json responses without Content-Length set.
* [patch] Detach `WebServiceResponse.Content` to allow it to be read as a stream.

## Released

### 0.6.0

* [minor] Add `WebServiceRequestSettings.GetHttpClient` to allow `HttpClient` reuse.

### 0.5.0

* [minor] Support responses with an explicit `StatusCode` property.
* [minor] Add `WebServiceRequestSettings.StartTrace` callback.

### 0.4.0

* [minor] Add `params string[]` option to `CreateRequest`.
* [major] Adapt to breaking changes in `Faithlife.Utility`.

### 0.3.0

* [patch] Change the only ConfigureAwait(true) to ConfigureAwait(false).
* [minor] Add If-Match header support.

### 0.2.0

* [major] Adapt to breaking changes in `Faithlife.Json`.
  * `JsonInputSettings` and `JsonOutputSettings` were merged into `JsonSettings`.
  * `GetRequestUri` now uses `IEnumerable<KeyValuePair<string, object>>` instead of an anonymous `object`.
* [major] Change `AcceptedStatusCodes` to use `IReadOnlyList<HttpStatusCode>`.

### 0.1.2

* [patch] Fix cookie handling in `WebServiceRequestBase`.

### 0.1.1

* Update to Faithlife.Json 0.1.1.

### 0.1.0

* Initial release.
