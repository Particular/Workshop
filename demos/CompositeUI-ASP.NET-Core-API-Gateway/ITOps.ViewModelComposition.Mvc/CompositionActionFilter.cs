using ITOps.ViewModelComposition.Gateway;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace ITOps.ViewModelComposition.Mvc
{
    public class CompositionActionFilter : IAsyncResultFilter
    {
        public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            if (context.Controller is Controller ctrl)
            {
                //we support only non-POCO Controller ATM
                var compositionResult = await CompositionHandler.HandleGetRequest(context.HttpContext);
                if (compositionResult.StatusCode == StatusCodes.Status200OK)
                {
                    ctrl.ViewData.Model = compositionResult.ViewModel;
                }
            }

            await next();

            //if (context.Result is ViewResult viewResult && viewResult.ViewData.Model == null)
            //{
            //    //MVC
            //    var compositionResult = await CompositionHandler.HandleGetRequest(context.HttpContext);
            //    if (compositionResult.StatusCode == StatusCodes.Status200OK)
            //    {

            //    }
            //}
            //else if (context.Result is ObjectResult objectResult && objectResult.Value == null)
            //{
            //    //WebAPI
            //    var compositionResult = await CompositionHandler.HandleGetRequest(context.HttpContext);
            //    if (compositionResult.StatusCode == StatusCodes.Status200OK)
            //    {
            //        objectResult.Value = compositionResult.ViewModel;
            //        objectResult.StatusCode = StatusCodes.Status200OK;
            //    }
            //}
        }
    }
}
