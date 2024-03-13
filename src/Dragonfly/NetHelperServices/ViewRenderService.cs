namespace Dragonfly.NetHelperServices
{
	using System;
	using System.Collections.Generic;

	using System.Linq;
	using System.Text;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Threading.Tasks;
	using Dragonfly.NetModels;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Mvc.Abstractions;
	using Microsoft.AspNetCore.Mvc.ModelBinding;
	using Microsoft.AspNetCore.Mvc.Rendering;
	using Microsoft.AspNetCore.Mvc.Razor;
	using Microsoft.AspNetCore.Mvc.ViewFeatures;
	using Microsoft.AspNetCore.Routing;

	//From https://stackoverflow.com/a/57888901/3841490

	public interface IViewRenderService
	{
		//public string RenderToStringWithStatus(HttpContext HttpContext, string ViewName, object Model, out StatusMessage Status);

		//public string RenderToStringWithStatus(HttpContext HttpContext, string ViewName, object Model, Dictionary<string, object> ViewDataDictionary, out StatusMessage Status);

		Task<string> RenderToStringAsync(HttpContext HttpContext, string ViewName, object Model, Dictionary<string, object> ViewDataDictionary = null);
	}

	/// <summary>Allows Rendering a Razor View to a string</summary>
	///DI Setup: services.AddScoped&gt;IViewRenderService, ViewRenderService&lt;();
	///Usage (in a Controller): string html = await _RenderService.RenderToStringAsync(httpContext,"NameOfPartial", new Model());
	public class ViewRenderService : IViewRenderService
	{
		#region CTOR & DI

		private readonly IRazorViewEngine _razorViewEngine;
		private readonly ITempDataProvider _tempDataProvider;
		//     private readonly IServiceProvider _serviceProvider;
		//private readonly IHttpContextAccessor _contextAccessor;

		public ViewRenderService(IRazorViewEngine razorViewEngine, ITempDataProvider tempDataProvider)    //,IHttpContextAccessor contextAccessor IServiceProvider serviceProvider
		{
			_razorViewEngine = razorViewEngine;
			_tempDataProvider = tempDataProvider;
			//  _contextAccessor = contextAccessor;

			//  _serviceProvider = serviceProvider;
		}
		#endregion

		public async Task<string> RenderToStringAsync(HttpContext HttpContext, string ViewName, object Model, Dictionary<string, object> ViewDataDictionary = null)
		{
			//  var httpContext = new DefaultHttpContext { RequestServices = _serviceProvider };
			// var httpContext = _contextAccessor.HttpContext;
			var actionContext = new ActionContext(HttpContext, new RouteData(), new ActionDescriptor());

			using (var sw = new StringWriter())
			{
				var viewResult = _razorViewEngine.GetView(ViewName, ViewName, false);

				if (viewResult.View == null)
				{
					throw new ArgumentNullException($"{ViewName} does not match any available view");
				}

				var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
				{
					Model = Model
				};

				if (ViewDataDictionary != null)
				{
					foreach (var item in ViewDataDictionary)
					{
						viewDictionary.Add(item.Key, item.Value);
					}
				}

				var viewContext = new ViewContext(
					actionContext,
					viewResult.View,
					viewDictionary,
					new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
					sw,
					new HtmlHelperOptions()
				);

				await viewResult.View.RenderAsync(viewContext);
				return sw.ToString();
				//}
				//catch (Exception e)
				//{
				//	var msg = $"RenderToStringAsync: An error occurred while rendering the view '{ViewName}'";
				//	throw new Exception(msg, e);
				//}
			}
		}

		//TODO: Not quite working...
		//public string RenderToStringWithStatus(HttpContext HttpContext, string ViewName, object Model, Dictionary<string, object> ViewDataDictionary, out StatusMessage Status)
		//{
		//	Status = new StatusMessage();
		//	Status.ObjectName = ViewName;
		//	Status.RelatedObject = ViewDataDictionary;

		//	var task = RenderToStringAsync(HttpContext, ViewName, Model, ViewDataDictionary);

		//	if (task.IsCompletedSuccessfully)
		//	{
		//		Status.Success = true;
		//		return task.Result;
		//	}
		//	else
		//	{
		//		var msg = $"RenderToStringAsync: An error occurred while rendering the view '{ViewName}'";
		//		Status.Message = msg;
		//		Status.Success = false;
		//		Status.SetRelatedException(task.Exception);
		//		return "";
		//	}

		//}

		//TODO: Not quite working...
		//public string RenderToStringWithStatus(HttpContext HttpContext, string ViewName, object Model, out StatusMessage Status)
		//{
		//	return RenderToStringWithStatus(HttpContext, ViewName, Model, null, out Status);
		//}

	}
}

