[![Donate](https://img.shields.io/badge/Donate-PayPal-green.svg)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=RW6F5XUSMW5NJ)

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

## How to create a package  
  
You will need the *NuGet command line tool* then visit this [NuGet](https://docs.nuget.org/consume/command-line-reference) link to obtain and install this tool.  
  
Do not forget to build the project `Release mode` before generate the package ;D  
  
With NuGet installed, on `\src\NuGetPkg` folder of this repository, execute the following command to generate the package: `nuget.exe pack Package.nuspec`, it's done, the `.nupkg` was created at the same folder.  

Danke  

[WebApiClientTS on nuget.org](https://www.nuget.org/packages/WebApiClientTS/)
