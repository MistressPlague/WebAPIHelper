# WebAPIHelper
A C# Helper class for making web API's. This makes it extremely easy.

# Example Usage:
Somewhere in your class:
```csharp
private WebAPIHelper Helper;

private async void HandleData(string Data, RequestType requestType, HttpListenerContext context)
{
    if (requestType == RequestType.POST)
    {
        // Send a 200 OK response
        Helper.SendResponse(context, "OK", HttpStatusCode.OK);
    }
}
```

Somewhere in your code when you want to start the web api:
```csharp
Helper = new WebAPIHelper(HandleData, $"{token.MakeAlphaNumeric().Substring(0, 5)}", 8000);
```
