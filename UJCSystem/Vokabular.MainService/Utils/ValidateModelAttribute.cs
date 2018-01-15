using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Vokabular.RestClient.Contracts;

namespace Vokabular.MainService.Utils
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var modelState = context.ModelState;
                var result = new ValidationResultContract
                {
                    Message = "Validation Failed",
                    Errors = modelState.Keys.SelectMany(key => modelState[key].Errors.Select(x =>
                        new ValidationErrorContract
                        {
                            Field = key,
                            Message = x.ErrorMessage
                        })).ToList(),
                };

                context.Result = new BadRequestObjectResult(result);
            }
        }
    }
}
