using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantEdgeAPI.Models.IO;
using Microsoft.AspNetCore.Http;
using AdvantEdgeAPI.Models.DB;
using AdvantEdgeAPI.Models;

namespace AdvantEdgeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Callback : ControllerBase
    {
        private readonly ILogger<Callback> _logger;
        private ApplicationDbContext _dbContext;

        public Callback(ILogger<Callback> logger, ApplicationDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Endpoint for example.com to mark request as started
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Post(Guid id, [FromBody] string status)
        {
            _logger.LogInformation($"Calling POST on callback/{id} endpoint");

            Transaction transaction = _dbContext.Transactions.Where(t => t.Id == id).FirstOrDefault();

            //check to see if we found a record in the database
            if (transaction == null)
            {
                _logger.LogError($"Couldn't find transaction Id {id}");
                //kick off more error handling here, depending if example.com handles it, or if has to raise some internal process
                //don't continue
                return null;
            }

            transaction.Status = status;

            //attempt to update the local database
            try
            {
                _dbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Failed writing updated status to transaction Id {id}");
                //kick off more error handling ehre
            }

            return null;
        }


        /// <summary>
        /// Endpoint for example.com to update the request
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Put(Guid id, [FromBody] ServiceResponse request)
        {
            _logger.LogInformation($"Calling PUT on callback/{id} endpoint");

            Transaction transaction = _dbContext.Transactions.Where(t => t.Id == id).FirstOrDefault();
            
            //check to see if we found a record in the database
            if (transaction == null)
            {
                _logger.LogError($"Couldn't find transaction Id {id}");
                //kick off more error handling here, depending if example.com handles it, or if has to raise some internal process
                //don't continue
                return null;
            }

            transaction.Status = request.Status;
            transaction.Detail = request.Detail;
            try
            {
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed writing updated status to transaction Id {id}");
                //kick off more error handling ehre
            }

            return null;
        }
    }
}
