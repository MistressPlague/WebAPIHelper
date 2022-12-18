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
        // Tip: Data will be in json format, regardless of the data received. Only tested with FormUrlEncoded Data.
    
        // Send a 200 OK response
        Helper.SendResponse(context, "OK", HttpStatusCode.OK);
    }
}
```

Somewhere in your code when you want to start the web api:
```csharp
Helper = new WebAPIHelper(HandleData, "test", 8000);
```

This will host on http://{YOURIP}:8000/test/

YOURIP being either a domain proxied to your system/servers public IP, or direct. Note it wont have SSL if direct and not proxied. Cloudflare is suggested.

Note it depends on the port being open.
