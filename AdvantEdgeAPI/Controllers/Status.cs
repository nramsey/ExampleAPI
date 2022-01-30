using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantEdgeAPI.Models.IO;
using AdvantEdgeAPI.Models.DB;
using Microsoft.EntityFrameworkCore;
using AdvantEdgeAPI.Models;
using Microsoft.AspNetCore.Http;

namespace AdvantEdgeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Status : ControllerBase
    {
        private readonly ILogger<Status> _logger;
        private ApplicationDbContext _dbContext;

        public Status(ILogger<Status> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// GET request to return a single client output object
        /// </summary>
        /// <returns>ClientOutput JSON</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<ClientResponse> Get(Guid id)
        {
            _logger.LogInformation("GET called on /status endpoint");

            //retrienve the 
            Transaction transaction = _dbContext.Transactions.Where(t => t.Id == id).FirstOrDefault();

            //return 404 if we find nothing
            if(transaction == null)
            {
                return NotFound();
            }

            //cast the response to the return type
            ClientResponse output = new ClientResponse()
            {
                Detail = transaction.Detail,
                Body = transaction.Body,
                Status = transaction.Status,
            };

            return output;
        }
    }
}
