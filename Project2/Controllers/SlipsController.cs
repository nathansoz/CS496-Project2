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
        IDocumentDbService<SlipEntity> _slipService;

        IDocumentDbService<BoatEntity> _boatService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SlipsController(IDocumentDbService<SlipEntity> slipService, IDocumentDbService<BoatEntity> boatService)
        {
            _slipService = slipService;
            _boatService = boatService;
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
                return Ok((await _slipService.GetAsync()).Select(x => new SlipResponseEntity(x)));
            }

            try
            {
                var test = await _slipService.GetAsync(id);
                return Ok(new SlipResponseEntity(await _slipService.GetAsync(id)));
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
        /// Get a boat for a given slip
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/boat")]
        public async Task<ActionResult> GetSlipBoat(Guid id)
        {
            SlipEntity slip;
            try
            {
                slip = await _slipService.GetAsync(id);
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

            if(!slip.CurrentBoat.HasValue)
            {
                return NotFound("The current slip has no boat");
            }

            try
            {
                return Ok(new BoatResponseEntity(await _boatService.GetAsync(slip.CurrentBoat.Value)));
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

            if ((await _slipService.GetAsync()).Any(x => x.Number == slip.Number))
            {
                return BadRequest($"The request tried to add a slip number that already exists.");
            }

            slip.Id = Guid.NewGuid();
            slip.CurrentBoat = null;
            slip.ArrivalDate = null;

            try
            {
                await _slipService.UpsertAsync(slip);
                return Ok(new SlipResponseEntity(slip));
            }
            catch
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Replace a slip. As replacing/patching is the same thing, we just use both verbs here
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        [HttpPatch("{id}")]
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

            if ((await _slipService.GetAsync()).Any(x => x.Number == replacementSlip.Number))
            {
                return BadRequest($"The request tried to replace with a slip that already exists.");
            }

            replacementSlip.Id = id;

            SlipEntity slip;
            try
            {
                slip = await _slipService.GetAsync(id);
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
                await _slipService.UpsertAsync(replacementSlip);
                return Ok(new SlipResponseEntity(replacementSlip));
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
                slip = await _slipService.GetAsync(id);
            }
            catch (DocumentClientException ex)
            {
                if (ex.Message.Contains("Resource Not Found"))
                {
                    return BadRequest("Slip does not exist");
                }

                return StatusCode(500);
            }

            if(slip.CurrentBoat.HasValue)
            {
                try
                {
                    BoatEntity boat = await _boatService.GetAsync(slip.CurrentBoat.Value);
                    boat.AtSea = true;
                    await _boatService.UpsertAsync(boat);
                }
                catch (DocumentClientException ex)
                {
                    if (!ex.Message.Contains("Resource Not Found"))
                    {
                        return StatusCode(500);
                    }
                }
            }

            try
            {
                await _slipService.DeleteAsync(slip.Id);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}
