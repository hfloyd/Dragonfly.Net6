# Dragonfly.Net6 #

A collection of .Net Helpers/Models created by [Heather Floyd](https://www.HeatherFloyd.com).

## Installation ##
[![Nuget Downloads](https://buildstats.info/nuget/Dragonfly.Net6)](https://www.nuget.org/packages/Dragonfly.Net6/)

    PM > Install-Package Dragonfly.Net6

## Features & Usage : Models ###

### StatusMessage

An object used for collecting and reporting information about code operations - a great way to return more detailed information from your custom functions and APIs. You can assign any Exceptions, as well as nest statuses. Explore all the properties and methods for details.


*Example Usage:*

	public StatusMessage GetLocalFilesInfo(out List<FileInfo> FilesList)
    {
        FilesList = new List<FileInfo>();
        var returnStatus = new StatusMessage(true);
        returnStatus.ObjectName = "GetLocalFilesInfo";

        IEnumerable<FileInfo> files;
        var statusGetListOfFiles = GetListOfFiles(out files);
        returnStatus.InnerStatuses.Add(statusGetListOfFiles);

        if (files.Any())
        {
            foreach (var fileInfo in files)
            {
                StatusMessage readStatus = new StatusMessage(true);
                readStatus.RunningFunctionName = "GetLocalFilesInfo";
                try
                {
                    FilesList.Add(fileInfo);
                }
                catch (Exception e)
                {
                    readStatus.Success = false;
                    readStatus.Message = $"GetLocalFilesInfo: Failure getting file '{fileInfo.FullName}'.";
                    readStatus.SetRelatedException(e);
                }
                returnStatus.InnerStatuses.Add(readStatus);
            }
        }

        return returnStatus;
    }

### HttpResponseMessageResult
Allows you to use familiar HttpResponse syntax from .Net Framework to return an IActionResult.


*Example*

        [HttpGet]
        public IActionResult DoSomethings()
        {
            var status = new StatusMessage(true);

            try
            {
				//These two service calls resturn Status messages themselves, so we have additional data
                status.InnerStatuses.Add(_MyService.DoSomething);
                status.InnerStatuses.Add(_MyService.DoSomethingElse);
            }
            catch (Exception ex)
            {
                status.RelatedException = ex;
                status.Success = false;
                status.Message = $"Failure while running Code: FetchAllRemoteNodesData('{EnvironmentType}',{UpdateRemoteFirst})";
                _logger.LogError(ex, status.Message);
            }

            //Return JSON
            string json = JsonConvert.SerializeObject(status);
            var result = new HttpResponseMessage()
            {
                Content = new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json"
                )
            };

            return new HttpResponseMessageResult(result);
        }

## Features & Usage : Static Helpers ##
TBA


## Features & Usage : Helper Services ##

### ViewRenderService

Renders an MVC View to a HTML string.

*Example: Dependency Injection - Setup.cs*

    ...
    services.AddScoped<IViewRenderService, ViewRenderService>();
    ...

	(Might also need:)
	services.AddMvcCore().AddRazorViewEngine();
    services.AddControllersWithViews();
    services.AddRazorPages();

*Example: Usage (in a Controller)*

	public class MyController : Controller
    {
        private readonly IViewRenderService _viewRenderService;

		public MyController(IViewRenderService viewRenderService)
        {
            _viewRenderService = viewRenderService;
        }

        [HttpGet]
        public IActionResult DoStuff()
        {
			//VIEW PATH
			var viewPath = "~/SomeFolder/Views/MyView.cshtml"; 
			//(Or, if in standard View folders, "MyView" will be sufficient.)
		
			//GET DATA TO DISPLAY
		    SomeModel model = new SomeModel(); //Do stuff to actually have data here...
		   
		    //VIEW DATA (OPTIONAL)
		    var viewData = new Dictionary<string, object>();
		    viewData.Add("Title", "The title for the view");
		    viewData.Add("Qty", 5);
		
		    //RENDER
		    var htmlTask = _viewRenderService.RenderToStringAsync(this.HttpContext, viewPath, model, viewData);
		    var displayHtml = htmlTask.Result;
			...
		}


*Example: Usage (in a Controller) - ASYNC*

    ... 
     string html = await _viewRenderService.RenderToStringAsync("MyViewFile", new Model());
    ...
