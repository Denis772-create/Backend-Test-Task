using Backend_Test_Task.Exceptions;
using Backend_Test_Task.Infrastructure;
using Backend_Test_Task.Models;

namespace Backend_Test_Task.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, ApplicationDbContext dbContext)
        {
            try
            {
                await _next(context);
            }
            catch (SecureException ex)
            {
                await HandleSecureExceptionAsync(context, dbContext, ex);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, dbContext, ex);
            }
        }

        private async Task HandleSecureExceptionAsync(HttpContext context, ApplicationDbContext dbContext, SecureException ex)
        {
            var eventId = Guid.NewGuid();
            var journalEntry = new ExceptionJournal
            {
                Id = eventId,
                QueryParams = context.Request.QueryString.ToString(),
                BodyParams = await ReadRequestBodyAsync(context.Request),
                StackTrace = ex.StackTrace ?? "",
                ExceptionType = nameof(SecureException)
            };

            dbContext.ExceptionJournals.Add(journalEntry);
            await dbContext.SaveChangesAsync();

            var result = new
            {
                type = "Secure",
                id = eventId.ToString(),
                data = new { message = ex.Message }
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(result);
        }

        private async Task HandleExceptionAsync(HttpContext context, ApplicationDbContext dbContext, Exception ex)
        {
            var eventId = Guid.NewGuid();
            var journalEntry = new ExceptionJournal
            {
                Id = eventId,
                QueryParams = context.Request.QueryString.ToString(),
                BodyParams = await ReadRequestBodyAsync(context.Request),
                StackTrace = ex.StackTrace ?? "",
                ExceptionType = ex.GetType().Name
            };

            dbContext.ExceptionJournals.Add(journalEntry);
            await dbContext.SaveChangesAsync();

            var result = new
            {
                type = "Exception",
                id = eventId.ToString(),
                data = new { message = $"Internal server error ID = {eventId}" }
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(result);
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            using var reader = new StreamReader(request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return body;
        }
    }
}
