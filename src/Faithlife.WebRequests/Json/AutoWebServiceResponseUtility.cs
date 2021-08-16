using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Faithlife.WebRequests.Json
{
	/// <summary>
	/// Utility methods for AutoWebServiceResponse.
	/// </summary>
	public static class AutoWebServiceResponseUtility
	{
		/// <summary>
		/// Returns the value of the desired property, or throws an exception if the desired property contains the default value.
		/// </summary>
		/// <typeparam name="TResponse">The type of the response.</typeparam>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="response">The AutoWebServiceResponse to read the property from.</param>
		/// <param name="getProperty">The func that reads the desired property.</param>
		/// <returns>The value of the property.</returns>
		public static TProperty GetExpectedResult<TResponse, TProperty>(this TResponse response, Func<TResponse, TProperty> getProperty)
			where TResponse : AutoWebServiceResponse
		{
			TProperty propertyValue = getProperty(response);

			if (propertyValue is null || EqualityComparer<TProperty>.Default.Equals(propertyValue, default!))
				throw response.CreateException("Unexpected response encountered.");

			return propertyValue;
		}

		/// <summary>
		/// Throws an exception if the desired property contained the default value.
		/// </summary>
		/// <typeparam name="TResponse">The type of the response.</typeparam>
		/// <typeparam name="TProperty">The type of the property.</typeparam>
		/// <param name="response">The AutoWebServiceResponse to read the property from.</param>
		/// <param name="getProperty">The func that reads the desired property.</param>
		public static void VerifyResultIsExpected<TResponse, TProperty>(this TResponse response, Func<TResponse, TProperty> getProperty)
			where TResponse : AutoWebServiceResponse
		{
			GetExpectedResult(response, getProperty);
		}

		/// <summary>
		/// Returns PropertyInfo of the desired property.
		/// </summary>
		/// <param name="response">The response.</param>
		/// <param name="propertyName">The name of the property.</param>
		/// <returns>The PropertyInfo of the property.</returns>
		internal static PropertyInfo? GetStatusCodeProperty(object response, string? propertyName)
		{
			if (propertyName == null)
				return null;

			return response.GetType().GetRuntimeProperties().FirstOrDefault(x =>
				x.GetMethod != null &&
				!x.GetMethod.IsStatic &&
				string.Equals(x.Name, propertyName, StringComparison.OrdinalIgnoreCase));
		}
	}
}
