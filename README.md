# WebApiClientTS
Asp.NET Web API client generator for TypeScript files  
  
This tool helps you generating the complete client API in TypeScript to consume a Web API made in .NET . It generates interfaces for all the types exposed by the API and classes to request each controller.
 
The generation is based on the ASP.NET ApiExplorer class. wich defines metadata for all the controllers exposed. This metadata is fed on a template (using the RazorEngine), wich generates the client API files in TypeScript.
  
## Usage  
  
First, you need to install the NuGet package, you can do it with that command:  
```
Install-Package WebApiClientTS
```
  
Them you need to implement a specific controller in your API to explore your API and generate the client API when you call that.  
![](./assets/Controller.png)  
  
You need to indicate a **.cshtml** template to the generator, use it like line 21.  
  
Now you can run the web API and then access the controller to generate the typescript client API.  
![](./assets/RunCodeGenerator.png)
  
It's done, the typescript client api was generated  
![](./assets/TSClientApi.png)  

Danke  

[WebApiClientTS on nuget.org](https://www.nuget.org/packages/WebApiClientTS/)
