using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Net;
using System.Text;
using Yarp.ReverseProxy;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Model;

namespace Azure.OpenAI.ChargebackProxy.ReverseProxy;

public class RetryMiddleware
{
    private readonly RequestDelegate _next;

    public RetryMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context, IProxyStateLookup proxyStateLookup)
    {
        context.Request.EnableBuffering();
        var reverseProxyFeature = context.GetReverseProxyFeature();

        await _next(context);

        var statusCode = context.Response.StatusCode;
        //switch to fallback if rate limit is hit on primary
        if (statusCode >= 429)
        {
            context.Response.Clear();
            var lookup = context.RequestServices.GetRequiredService<IProxyStateLookup>();
            if (lookup.TryGetCluster("Fallback", out var cluster))
            {
                context.ReassignProxyRequest(cluster);
            }

            context.Request.Body.Position = 0;
            reverseProxyFeature.ProxiedDestination = null;
            await _next(context);
        }
    }

  

}
