using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace NewsletterProvider.Functions
{
    public class UpdateSubscriber(ILogger<UpdateSubscriber> logger, DataContext dataContext)
    {
        private readonly ILogger<UpdateSubscriber> _logger = logger;
        private readonly DataContext _dataContext = dataContext;

        [Function("UpdateSubscriber")]
        public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "put", "get", "post", Route = "subscribers/{email}")] HttpRequest req, string email)
        {
            _logger.LogInformation("UpdateUser function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            UpdateSubscriberModel updateSubscriberModel;

            try
            {
                updateSubscriberModel = JsonSerializer.Deserialize<UpdateSubscriberModel>(requestBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException)
            {
                return new BadRequestObjectResult("Invalid JSON data.");
            }

            if (updateSubscriberModel == null || updateSubscriberModel.Email == null)
            {
                return new BadRequestObjectResult("Invalid user data or email mismatch.");
            }

            var subscriber = await _dataContext.Subscribers.SingleOrDefaultAsync(u => u.Email == email);
            if (subscriber == null)
            {
                return new NotFoundResult();
            }

            //subscriber.Email = updateSubscriberModel.Email;
            //subscriber.IsSubscribed = updateSubscriberModel.SubscribeToNewsletter;
            

            try
            {
                _dataContext.Subscribers.Remove(subscriber);
                await _dataContext.SaveChangesAsync();

                var newSubscriber = new SubscribeEntity
                {
                    Email = updateSubscriberModel.Email,
                    IsSubscribed = updateSubscriberModel.SubscribeToNewsletter,
                };

                _dataContext.Subscribers.Add(newSubscriber);
                await _dataContext.SaveChangesAsync();
                return new OkObjectResult(newSubscriber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user.");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }

        public class UpdateSubscriberModel
        {
            public string Email { get; set; }
            public bool SubscribeToNewsletter { get; set; }
        }
    }
    
}

