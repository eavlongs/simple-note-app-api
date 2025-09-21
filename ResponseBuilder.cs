using Microsoft.AspNetCore.Mvc;

namespace simple_note_app_api
{
    public class ResponseBuilder
    {
        public static IActionResult Success(object data, string message = "Request successful")
        {
            return new OkObjectResult(new { success = true, message, data });
        }

        public static IActionResult Error(int statusCode, object error, string message)
        {
            return new ObjectResult(new { success = false, message, error }) { StatusCode = statusCode };
        }

        public static IActionResult BadRequest(object error, string message = "Bad request")
        {
            return Error(400, error, message);
        }

        public static IActionResult NotFound(object error, string message = "Resource not found")
        {
            return Error(404, error, message);
        }

        public static IActionResult InternalServerError(object error, string message = "Internal server error")
        {
            return Error(500, error, message);
        }

        public static IActionResult Unauthorized(object error, string message = "Unauthorized")
        {
            return Error(401, error, message);
        }

        public static IActionResult ValidationError(object error, string message = "Validation error")
        {
            return Error(422, error, message);
        }
    }
}
