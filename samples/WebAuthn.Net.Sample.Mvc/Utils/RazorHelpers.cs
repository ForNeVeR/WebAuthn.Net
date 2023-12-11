using Microsoft.AspNetCore.Mvc;

namespace WebAuthn.Net.Sample.Mvc.Utils;


public static class RazorHelpers
{
    /// <summary>
    /// Adds active class if controller and action is matched with link
    /// <see href="https://stackoverflow.com/a/72787049"/>
    /// </summary>
    public static string ActiveClassIfMatched(this IUrlHelper urlHelper, string controller, string action)
    {
        ArgumentNullException.ThrowIfNull(urlHelper);
        var result = "active";
        var controllerName = urlHelper.ActionContext.RouteData.Values["controller"]?.ToString();
        var methodName = urlHelper.ActionContext.RouteData.Values["action"]?.ToString();

        if (string.IsNullOrEmpty(controllerName)) return "";
        if (controllerName.Equals(controller, StringComparison.OrdinalIgnoreCase))
        {
            if (methodName != null && methodName.Equals(action, StringComparison.OrdinalIgnoreCase))
            {
                return result;
            }
        }
        return "";
    }
}
