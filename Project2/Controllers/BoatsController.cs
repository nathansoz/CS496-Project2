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
    /// Controller for /boats
    /// </summary>
    [Route("api/[controller]")]
    public class BoatsController : Controller
    {
        IDocumentDbService<BoatEntity> _boatService;

        IDocumentDbService<SlipEntity> _slipService;

        /// <summary>
        /// Default constructor
        /// </summary>
        public BoatsController(IDocumentDbService<BoatEntity> boatService, IDocumentDbService<SlipEntity> slipService)
        {
            _boatService = boatService;
            _slipService = slipService;
        }

        /// <summary>
        /// Get a given boat
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id?}")]
        public async Task<ActionResult> GetBoat(Guid id)
        {
            if(id == Guid.Empty)
            {
                return Ok(await _boatService.GetAsync());
            }

            try
            {
                return Ok(await _boatService.GetAsync(id));
            }
            catch (DocumentClientException ex)
            {
                if (ex.Message.Contains("Resource Not Found"))
                {
                    return BadRequest("Boat does not exist");
                }

                return StatusCode(500);
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
        public async Task<ActionResult> AddBoat([FromBody]BoatEntity boat)
        {
            if(boat == null)
            {
                return BadRequest("The body must contain Json.");
            }

            if(boat.Name == null)
            {
                return BadRequest($"The request body must have a name for the boat.");
            }

            if(boat.Type == null)
            {
                return BadRequest("The request body must have a type for the boat.");
            }

            if(boat.Length <= 0)
            {
                return BadRequest("invalid length.");
            }

            if(boat.Id != Guid.Empty)
            {
                return BadRequest("The id cannot be included.");
            }

            boat.Id = Guid.NewGuid();
            boat.AtSea = true;

            try
            {
                await _boatService.UpsertAsync(boat);
                return Ok(boat);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Replace a boat
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> ReplaceBoat(Guid id, [FromBody]BoatEntity replacementBoat)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest();
            }

            if (replacementBoat == null)
            {
                return BadRequest("The body must contain Json.");
            }

            if (replacementBoat.Name == null)
            {
                return BadRequest($"The request body must have a name for the boat.");
            }

            if (replacementBoat.Type == null)
            {
                return BadRequest("The request body must have a type for the boat.");
            }

            if (replacementBoat.Length <= 0)
            {
                return BadRequest("invalid length.");
            }

            if (replacementBoat.Id != Guid.Empty)
            {
                return BadRequest("The id cannot be included.");
            }

            replacementBoat.Id = id;

            BoatEntity boat;
            try
            {
                boat = await _boatService.GetAsync(id);
            }
            catch (DocumentClientException ex)
            {
                if (ex.Message.Contains("Resource Not Found"))
                {
                    return BadRequest("Boat does not exist");
                }
            }

            try
            {
                await _boatService.UpsertAsync(replacementBoat);
                return Ok(replacementBoat);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Delete a given boat
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBoat(Guid id)
        {
            if (id == null || id == Guid.Empty)
            {
                return BadRequest();
            }

            BoatEntity boat;

            try
            {
                boat = await _boatService.GetAsync(id);
            }
            catch(DocumentClientException ex)
            {
                if (ex.Message.Contains("Resource Not Found"))
                {
                    return BadRequest("Boat does not exist");
                }

                return StatusCode(500);
            }

            try
            {
                await _boatService.DeleteAsync(boat.Id);
                return Ok();
            }
            catch
            {
                return StatusCode(500);
            }
        }

        /// <summary>
        /// Update a given boat
        /// </summary>
        /// <param name="id"></param>
        /// <param name="inputBoat"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult> ModifyBoat(Guid id, [FromBody]BoatEntity inputBoat)
        {
            // Input validation. Catch easy errors before going to data layer

            if(id == null || id == Guid.Empty)
            {
                return BadRequest();
            }

            if(inputBoat == null)
            {
                return BadRequest("At least one property must be modified.");
            }

            if(inputBoat.Length < 0)
            {
                return BadRequest("Boat length cannot be negative.");
            }

            // Now pull the boat from the data layer and modify it if needed

            BoatEntity boat;

            try
            {
                boat = await _boatService.GetAsync(id);
            }
            catch (DocumentClientException ex)
            {
                if (ex.Message.Contains("Resource Not Found"))
                {
                    return BadRequest("Boat does not exist");
                }

                return StatusCode(500);
            }

            bool changed = false;
            if (inputBoat.Name != null)
            {
                boat.Name = inputBoat.Name;
                changed = true;
            }

            if (inputBoat.Type != null)
            {
                boat.Type = inputBoat.Type;
                changed = true;
            }

            if(inputBoat.Length > 0)
            {
                boat.Length = inputBoat.Length;
                changed = true;
            }

            if(changed == true)
            {
                await _boatService.UpsertAsync(boat);
            }

            return Ok(boat);
        }

        /// <summary>
        /// Allow a boat to arrive at a slip
        /// </summary>
        /// <param name="id">The id of the boat</param>
        /// <param name="entity">The slip that the boat should dock at</param>
        /// <returns></returns>
        [HttpPut("{id}/arrival")]
        public async Task<ActionResult> Arrive(Guid id, [FromBody]SlipEntity entity)
        {
            if(entity.Id == Guid.Empty && entity.Number == 0)
            {
                return BadRequest("Either a slip id or a slip number must be provided.");
            }

            if(!entity.ArrivalDate.HasValue)
            {
                entity.ArrivalDate = DateTime.Now;
            }

            SlipEntity retrievedSlip;
            if(entity.Id != Guid.Empty)
            {
                try
                {
                    retrievedSlip = await _slipService.GetAsync(entity.Id);
                }
                catch (DocumentClientException ex)
                {
                    if (ex.Message.Contains("Resource Not Found"))
                    {
                        return BadRequest("Slip does not exist");
                    }

                    return StatusCode(500);
                }
                catch
                {
                    return StatusCode(500);
                }

                if(entity.Number != 0 && entity.Number != retrievedSlip.Number)
                {
                    return BadRequest("Slip number/id mismatch!.");
                }
            }
            else
            {
                try
                {
                    retrievedSlip = (await _slipService.GetAsync()).SingleOrDefault(x => x.Number == entity.Number);
                }
                catch
                {
                    return StatusCode(500);
                }

                if(retrievedSlip == null)
                {
                    return BadRequest($"Slip with number {entity.Number} does not exist.");
                }
            }

            if(retrievedSlip.CurrentBoat != null)
            {
                return BadRequest($"Slip with number {entity.Number} is occupied!");
            }

            BoatEntity retrievedBoat;
            try
            {
                retrievedBoat = await _boatService.GetAsync(id);
            }
            catch (DocumentClientException ex)
            {
                if (ex.Message.Contains("Resource Not Found"))
                {
                    return BadRequest("Boat does not exist");
                }

                return StatusCode(500);
            }
            catch
            {
                return StatusCode(500);
            }

            retrievedSlip.CurrentBoat = id;
            retrievedSlip.ArrivalDate = entity.ArrivalDate;

            retrievedBoat.AtSea = false;

            List<Task> completionObject = new List<Task>
            {
                _slipService.UpsertAsync(retrievedSlip),
                _boatService.UpsertAsync(retrievedBoat)
            };

            await Task.WhenAll(completionObject);

            return Ok();
        }
    }
}
