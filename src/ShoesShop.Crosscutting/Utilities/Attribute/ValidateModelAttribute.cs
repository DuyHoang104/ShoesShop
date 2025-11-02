using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShoesShop.Crosscutting.Utilities.Attribute
{
    public class ValidateModelAttribute(string viewName) : ActionFilterAttribute
    {
            private readonly string _viewName = viewName;

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errorMessage = "Please enter valid data.";

                context.HttpContext.Session.SetString("ErrorMessage", errorMessage);

                context.Result = new RedirectResult(context.HttpContext.Request.Path);
            }
        }   
    }
}