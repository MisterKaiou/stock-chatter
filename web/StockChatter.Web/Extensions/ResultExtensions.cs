using Functional.Result;
using StockChatter.Shared.Models.Common;
using System.Net.Http.Json;

namespace StockChatter.Web.Extensions
{
	public static class ResultExtensions
	{
		public static async Task<Result<TResponse>> HttpResponseToResultAsync<TResponse>(
			this HttpResponseMessage response, Func<HttpContent, Task<TResponse>> contentHandler)
		{
			if (response.IsSuccessStatusCode)
				return Result.CreateSuccess(
					await contentHandler(response.Content)
				);

			return Result.CreateError<TResponse>(
				(await response.Content.ReadFromJsonAsync<ErrorModel>()).Errors
			);
		}

		public static async Task<Result<Unit>> HttpResponseToResultAsync(this HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode)
				return Result.CreateSuccess();

			return Result.CreateError(
				(await response.Content.ReadFromJsonAsync<ErrorModel>()).Errors
			);
		}
	}
}
