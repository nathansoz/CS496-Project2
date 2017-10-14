using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Project2.Models;
using Project2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Controllers
{
    /// <summary>
    /// Controller for /slips
    /// </summary>
    [Route("api/[controller]")]
    public class SlipsController : Controller
    {
        IDocumentDbService<SlipEntity> _documentDbService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SlipsController(IDocumentDbService<SlipEntity> documentDbService)
        {
            _documentDbService = documentDbService;
        }

        /// <summary>
        /// Get a given slip
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id?}")]
        public async Task<ActionResult> GetSlip(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Ok(await _documentDbService.GetAsync());
            }

            try
            {
                return Ok(await _documentDbService.GetAsync(id));
            }
            catch (DocumentClientException ex)
            {
                if (ex.Message.Contains("Resource Not Found"))
                {
                    return BadRequest("Slip does not exist");
                }

                throw;
            }
            catch
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Add a new boat
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddSlip([FromBody]SlipEntity slip)
        {
            if (slip == null)
            {
                return BadRequest("The body must contain Json.");
            }

            if (slip.Number == 0)
            {
                return BadRequest($"The request body must have a number for the slip that is 1 or greater.");
            }

            if (slip.Id != Guid.Empty)
            {
                return BadRequest("The id cannot be included.");
            }

            if ((await _documentDbService.GetAsync()).Any(x => x.Number == slip.Number))
            {
                return BadRequest($"The request tried to add a slip number that already exists.");
            }

            slip.Id = Guid.NewGuid();
            slip.CurrentBoat = null;
            slip.ArrivalDate = null;

            try
            {
                await _documentDbService.UpsertAsync(slip);
                return Ok(slip);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Replace a slip
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> ReplaceSlip(Guid id, [FromBody]SlipEntity replacementSlip)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest();
            }

            if (replacementSlip == null)
            {
                return BadRequest("The body must contain Json.");
            }

            if (replacementSlip.Number == 0)
            {
                return BadRequest("The slip id cannot be 0.");
            }

            if (replacementSlip.Id != Guid.Empty)
            {
                return BadRequest("The id cannot be included.");
            }

            if ((await _documentDbService.GetAsync()).Any(x => x.Number == replacementSlip.Number))
            {
                return BadRequest($"The request tried to replace with a slip that already exists.");
            }

            replacementSlip.Id = id;

            SlipEntity slip;
            try
            {
                slip = await _documentDbService.GetAsync(id);
            }
            catch (DocumentClientException ex)
            {
                if (ex.Message.Contains("Resource Not Found"))
                {
                    return BadRequest("Slip does not exist");
                }
            }

            try
            {
                await _documentDbService.UpsertAsync(replacementSlip);
                return Ok(replacementSlip);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Delete a given slip
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteSlip(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest();
            }

            SlipEntity slip;

            try
            {
                slip = await _documentDbService.GetAsync(id);
            }
            catch (DocumentClientException ex)
            {
                if (ex.Message.Contains("Resource Not Found"))
                {
                    return BadRequest("Slip does not exist");
                }

                return StatusCode(500);
            }

            try
            {
                await _documentDbService.DeleteAsync(slip.Id);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
