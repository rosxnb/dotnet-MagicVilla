using API.DTOs;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using AutoMapper;
using API.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaApiController : ControllerBase
    {
        private readonly APIResponse _response;
        private readonly IVillaRepository _dbVilla;
        private readonly IMapper _mapper;

        public VillaApiController(IVillaRepository dbVilla, IMapper mapper)
        {
            _dbVilla = dbVilla;
            _mapper = mapper;
            _response = new();
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetVillas()
        {
            try
            {
                IEnumerable<Villa> villaList = await _dbVilla.GetAllAsync();
                _response.Result = _mapper.Map<List<VillaDTO>>(villaList);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSucess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }
            return _response;
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    return BadRequest();
                }

                Villa? villa = await _dbVilla.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    return NotFound();
                }

                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSucess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }
            return _response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            try
            {
                if (createDTO == null)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Result = createDTO;
                    return BadRequest(_response);
                }

                // check if villa name is unique or not
                if (await _dbVilla.GetAsync(u => u.Name == createDTO.Name) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "Villa already Exits!");
                    return BadRequest(ModelState);
                }

                // if (villaDTO.Id > 0)
                // {
                //     return StatusCode(StatusCodes.Status500InternalServerError);
                // }

                // Villa villa = new()
                // {
                //     Name = createDTO.Name,
                //     Details = createDTO.Details,
                //     Rate = createDTO.Rate,
                //     Occupancy = createDTO.Occupancy,
                //     Sqft = createDTO.Sqft,
                //     ImageUrl = createDTO.ImageUrl,
                //     Amenity = createDTO.Amenity
                // };

                Villa villa = _mapper.Map<Villa>(createDTO);
                await _dbVilla.CreateAsync(villa);

                _response.Result = _mapper.Map<VillaDTO>(villa);
                _response.StatusCode = HttpStatusCode.OK;

                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSucess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }
            return _response;
        }

        [Authorize(Roles = "custom")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                Villa? villa = await _dbVilla.GetAsync(u => u.Id == id);
                if (villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                await _dbVilla.RemoveAsync(villa);

                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSucess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSucess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }
            return _response;
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            try
            {
                if (updateDTO == null || updateDTO.Id != id)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_response);
                }

                // var villa = _db.Villas.FirstOrDefault(u => u.Id == id)!;
                // villa.Name = villaDTO.Name;
                // villa.Occupancy = villaDTO.Occupancy;
                // villa.Sqft = villaDTO.Sqft;

                // Villa villa = new()
                // {
                //     Name = updateDTO.Name,
                //     Details = updateDTO.Details,
                //     Rate = updateDTO.Rate,
                //     Occupancy = updateDTO.Occupancy,
                //     Sqft = updateDTO.Sqft,
                //     ImageUrl = updateDTO.ImageUrl,
                //     Id = updateDTO.Id,
                //     Amenity = updateDTO.Amenity
                // };

                Villa villa = _mapper.Map<Villa>(updateDTO);
                _ = await _dbVilla.UpdateAsync(villa);


                _response.StatusCode = HttpStatusCode.NoContent;
                _response.IsSucess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSucess = false;
                _response.ErrorMessages = new List<string>()
                {
                    ex.ToString()
                };
            }
            return _response;
        }

        // [HttpPatch("{id:int}")]
        // [ProducesResponseType(StatusCodes.Status204NoContent)]
        // [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // public async Task<ActionResult<VillaDTO>> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        // {
        //     if (patchDTO == null || id == 0)
        //     {
        //         return BadRequest("dto null");
        //     }
        //
        //     Villa? villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);
        //     if (villa == null)
        //     {
        //         return BadRequest("villa null");
        //     }
        //
        //     VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);
        //     patchDTO.ApplyTo(villaDTO, ModelState);
        //
        //     Villa model = _mapper.Map<Villa>(villaDTO);
        //     await _dbVilla.UpdateAsync(model);
        //
        //     return ModelState.IsValid switch
        //     {
        //         true => NoContent(),
        //         false => BadRequest("modelstate invalid")
        //     };
        // }
    }
}
