using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

public class AdminAuthorizeAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var sessionName = context.HttpContext.Session.GetString("_Name");
        var role = context.HttpContext.Session.GetInt32("Role");

        if (string.IsNullOrEmpty(sessionName) || role == null || role != 1)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        base.OnActionExecuting(context);
    }
}
