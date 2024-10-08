﻿namespace Dragonfly.NetModels;

using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

//From: https://stackoverflow.com/questions/54136488/correct-way-to-return-httpresponsemessage-as-iactionresult-in-net-core-2-2/54187518#54187518

/// <summary>
/// Model to return an HttpResponseMessage as an IActionResult
/// </summary>
public class HttpResponseMessageResult : IActionResult
{
	private readonly HttpResponseMessage _responseMessage;

	/// <summary>
	/// Initialize with an HttpResponseMessage
	/// </summary>
	/// <param name="ResponseMessage"></param>
	public HttpResponseMessageResult(HttpResponseMessage ResponseMessage)
	{
		_responseMessage = ResponseMessage; // could add throw if null
	}

	/// <summary>
	/// Executes the Context Asynchronously
	/// </summary>
	/// <param name="Context"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	public async Task ExecuteResultAsync(ActionContext Context)
	{
		var response = Context.HttpContext.Response;
		
		if (_responseMessage == null)
		{
			var message = "Response message cannot be null";

			throw new InvalidOperationException(message);
		}

		using (_responseMessage)
		{
			response.StatusCode = (int)_responseMessage.StatusCode;

			var responseFeature = Context.HttpContext.Features.Get<IHttpResponseFeature>();
			if (responseFeature != null)
			{
				responseFeature.ReasonPhrase = _responseMessage.ReasonPhrase;
			}

			var responseHeaders = _responseMessage.Headers;

			// Ignore the Transfer-Encoding header if it is just "chunked".
			// We let the host decide about whether the response should be chunked or not.
			if (responseHeaders.TransferEncodingChunked == true &&
				responseHeaders.TransferEncoding.Count == 1)
			{
				responseHeaders.TransferEncoding.Clear();
			}

			foreach (var header in responseHeaders)
			{
				response.Headers.Append(header.Key, header.Value.ToArray());
			}

			if (_responseMessage.Content != null)
			{
				var contentHeaders = _responseMessage.Content.Headers;

				// Copy the response content headers only after ensuring they are complete.
				// We ask for Content-Length first because HttpContent lazily computes this
				// and only afterwards writes the value into the content headers.
				var unused = contentHeaders.ContentLength;

				foreach (var header in contentHeaders)
				{
					response.Headers.Append(header.Key, header.Value.ToArray());
				}

				await _responseMessage.Content.CopyToAsync(response.Body);
			}
		}
	}
}

