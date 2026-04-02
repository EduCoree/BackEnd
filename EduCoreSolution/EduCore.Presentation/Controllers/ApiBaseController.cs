using EduCore.Shared.CommonResult;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduCore.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApiBaseController : ControllerBase
    {
        protected ActionResult HandleResult(Result result)
        {
            if (result.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return HandelProblem(result.Errors);
            }
        }
        protected ActionResult HandleResult<T>(Result<T> result)
        {
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }
            else
            {
                return HandelProblem(result.Errors);
            }
        }
        private ActionResult HandelProblem(IReadOnlyList<Error> errors)
        {
            if (errors.Count == 0)
            {
                return Problem(
                    title: "An error occurred",
                    detail: "An unexpected error occurred. Please try again later.",
                    statusCode: StatusCodes.Status500InternalServerError
                    );
            }
            if (errors.All(e => e.Type == ErrorType.Validation))
            {
                return HandleValidationProblem(errors);
            }
            return HandleSingleProblem(errors[0]);
        }
        private ActionResult HandleSingleProblem(Error error)
        {
            return Problem(
                title: error.Code,
                detail: error.Description,
                type: error.Type.ToString(),
                statusCode: MapErrorTypeToStatusCode(error.Type)
                );

        }
        private static int MapErrorTypeToStatusCode(ErrorType errorType)
        {
            return errorType switch
            {
                ErrorType.Failure => StatusCodes.Status404NotFound,
                ErrorType.Validation => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.InvalidCredentials => StatusCodes.Status401Unauthorized,
                _ => StatusCodes.Status500InternalServerError

            };
        }
        private ActionResult HandleValidationProblem(IReadOnlyList<Error> errors)
        {
            var modelState = new ModelStateDictionary();
            foreach (var error in errors)
            {
                modelState.AddModelError(error.Code, error.Description);
            }
            return ValidationProblem(modelState);
        }
    }
}