using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantEdgeAPI.Models.IO;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using AdvantEdgeAPI.Models;
using AdvantEdgeAPI.Models.DB;
using System.Net.Http;
using System.Text.Json;

namespace AdvantEdgeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Request : ControllerBase
    {
        private readonly ILogger<Request> _logger;
        private ApplicationDbContext _dbContext;

        public Request(ILogger<Request> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// POST request to call example.com endpoint
        /// </summary>
        /// <returns>The Id generated for the request</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> Post([FromBody] ClientRequest request)
        {
            _logger.LogInformation("POST called on /request endpoint");

            //record the transaction to the database
            Transaction transaction = new Transaction()
            {
                Body = request.Body,
                Detail = null,
                Id = new Guid()
            };

            _dbContext.Add(transaction);

            try
            {
                _dbContext.SaveChanges();
                _logger.LogInformation($"Wrote request id {transaction.Id} to the database");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed writing transaction to database");
                //maybe show more info here depending on who the end user is
                return StatusCode(500);
            }

            //make the request to the example.com API
            HttpClient client = new HttpClient();

            //probably set up any other requirements the api may have here

            //set up the request payload
            ServiceRequest exampleRequest = new ServiceRequest()
            {
                Body = request.Body,
                CallBack = $"http://mysite.com/callback/{transaction.Id}"
            };

            //serialize request to json and post it to the example endpoint
            HttpContent content = new StringContent(JsonSerializer.Serialize(exampleRequest));
            var response = await client.PostAsync("https://example.com/request", content);

            //if we got an okay response
            if(response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var httpResponse = await response.Content.ReadAsStringAsync();
                ServiceResponse exampleResponse = JsonSerializer.Deserialize<ServiceResponse>(httpResponse);

                //return the id generated
                return transaction.Id.ToString();
            }
            //any other response show an error, maybe pass along different errors depending on the example.com response
            else
            {
                _logger.LogError($"Failed getting response from example.com, received response code {response.StatusCode}");
                //maybe show more info here depending on who the end user is
                return StatusCode(500);
            }
        }
    }
}
