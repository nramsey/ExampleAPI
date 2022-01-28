using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdvantEdgeAPI.Models.IO;
using Microsoft.AspNetCore.Http;

namespace AdvantEdgeAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Callback : ControllerBase
    {
        private readonly ILogger<Callback> _logger;

        public Callback(ILogger<Callback> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Endpoint for example.com to mark request as started
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Post(Guid id)
        {
            ClientRequest input = new ClientRequest()
            {
                Body = ""
            };
            return null;
        }


        /// <summary>
        /// Endpoint for example.com to update the request
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Put(Guid id)
        {
            return null;
        }
    }
}
