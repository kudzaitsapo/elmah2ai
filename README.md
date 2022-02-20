Elmah is a great tool for logging errors. However, the problem I encountered with it, was that it logged the errors to the file system or database, depending on your configuration. The database is a bit too slow, and the file system can balloon up in size to over tens of gigs in a few minutes ... or seconds for very high volume applications.

Microsoft Azure includes a very powerful log monitoring tool called Application Insights, and this powerful tool has a lot of features I'm not going to talk about. What I have managed to do, is to extend elmah, so that logs can be sent to Azure Application Insights since this feature wasn't available.

What is needed in order for this to work? Nothing much. Just a nuget package file and a dotnet core project.

## Steps Required to make the library work

1. Obtain the Nuget Package of the library from [here](https://www.nuget.org/packages/Elmah2AI.AppInsights/1.0.0), and install it as a dependency to your project.
2. Once installed, create the application insights resource, and get the `Instrumentation Key` from Azure Portal. Click [here](https://docs.microsoft.com/en-us/azure/azure-monitor/app/create-workspace-resource) to find out how to do this.
3. Open the `appsettings.json` of your project and add the following configurations:

```
"ApplicationInsights": {
      "InstrumentationKey": "some-awesome-instrumentation-key-goes-here"
},
```

1. Open the `Startup.cs` file, and inside the `ConfigureServices` method, add the following code:

```
services.AddElmah<AIErrorLog>(options =>
{
    options.ConnectionString = _appConfiguration["ApplicationInsights:InstrumentationKey"];
});
```

The code above initializes the Elmah Error Logging middleware using the instrumentation key for Application Insights.
Don't forget to import `AIErrorLog` from `Elmah2AI.AppInsights`. As for the `_appConfiguration` variable, declare it outside the constructor like this:

```
private readonly IConfigurationRoot _appConfiguration;
```

and then inject it into the constructor like this:

```
public Startup(IWebHostEnvironment hostEnvironment)
{
    _appConfiguration = hostEnvironment.GetAppConfiguration();
}
```

In the `Configure` method, add the following code:

```c#
app.UseElmah();
```



That's it. No complicated code needs to be written. Just four simple steps.

In order to access the logs, log into Azure Portal (https://portal.azure.com/ ) and then navigate to the Application Insights resource for your project. On the left sidebar, click on **Logs** under **Monitoring** (see below).

![Image 1](https://raw.githubusercontent.com/kudzaitsapo/elmah2ai/main/examples/Image1.png)

You might be presented with a modal, at this moment it's not necessary, so close it or disable it completely if you don't want to see it again (see below).

![Image 2](https://raw.githubusercontent.com/kudzaitsapo/elmah2ai/main/examples/Image2.png)

Once that's done, you'll have a screen like below:

![Image 3](https://raw.githubusercontent.com/kudzaitsapo/elmah2ai/main/examples/Image3.png)

From there, you can access request logs, exceptions, page views, custom logged events etc. You'll be accessing these logs by running queries inside the query editor and then pressing **Run**. You can also modify the time range of the logging, as Azure provides various options.

I have discovered only one issue with Azure Application Insights. The problem is, **the logs take about 3 - 4 minutes from the time of logging, to show up on the query results**. This is an issue I have not yet found a solution to.

Other than the above issue, there's a lot of things you can do with application insights, but I won't go into them since it's outside the scope of this page. So ... good luck, and may the code be with ye!
