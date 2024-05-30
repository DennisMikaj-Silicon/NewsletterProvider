using Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace NewsletterProvider.Functions
{
    public class GetSubscribers(ILogger<GetSubscribers> logger, DataContext context)
    {
        private readonly ILogger<GetSubscribers> _logger = logger;
        private readonly DataContext _context = context;

        [Function("GetSubscribers")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
        {
            try
            {
                var subscribers = await _context.Subscribers.ToListAsync();
                return new OkObjectResult(subscribers);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "An error occurred while retrieving subscribers.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
