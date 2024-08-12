using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Sxb.Framework.AspNetCoreHelper.ResponseModel;
using System.Linq;

namespace Sxb.Framework.AspNetCoreHelper.Filters
{
    public class ValidateModelResultFilter : IResultFilter
    {

        public void OnResultExecuted(ResultExecutedContext context)
        {

        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is BadRequestObjectResult || !context.ModelState.IsValid)
            {
                var result = context.ModelState.Keys
                        .SelectMany(key =>
                        context.ModelState[key].Errors.Select(x => new ValidationError(key, x.ErrorMessage)))
                        .FirstOrDefault();
                context.Result = new JsonResult(ResponseResult.Failed(ResponseCode.ValidationError, result.Message));
            }
        }

        public class ValidationError
        {
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Field { get; }
            public string Message { get; }
            public ValidationError(string field, string message)
            {
                Field = field != string.Empty ? field : null;
                Message = message;
            }
        }
    }
}
