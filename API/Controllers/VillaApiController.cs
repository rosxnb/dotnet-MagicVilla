using API.DTOs;
using API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using API.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public VillaApiController(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDTO>>> GetVillas()
        {
            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();
            return Ok(_mapper.Map<List<VillaDTO>>(villaList));
        }

        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDTO>> GetVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            var villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            return villa switch
            {
                null => NotFound(),
                _ => Ok(_mapper.Map<VillaDTO>(villa))
            };
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDTO>> CreateVilla([FromBody] VillaCreateDTO createDTO)
        {
            if (createDTO == null)
            {
                return BadRequest(createDTO);
            }

            // check if villa name is unique or not
            if (await _db.Villas.FirstOrDefaultAsync(u => u.Name == createDTO.Name) != null)
            {
                ModelState.AddModelError("CustomDuplicateError", "Villa already Exits!");
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
            _ = await _db.Villas.AddAsync(villa);
            _ = await _db.SaveChangesAsync();

            return CreatedAtRoute("GetVilla", new { id = villa.Id }, villa);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }

            Villa? villa = await _db.Villas.FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            _ = _db.Villas.Remove(villa);
            _ = await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VillaDTO>> UpdateVilla(int id, [FromBody] VillaUpdateDTO updateDTO)
        {
            if (updateDTO == null || updateDTO.Id != id)
            {
                return BadRequest();
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
            _ = _db.Villas.Update(villa);
            _ = await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<VillaDTO>> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest("dto null");
            }

            Villa? villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);
            if (villa == null)
            {
                return BadRequest("villa null");
            }

            VillaUpdateDTO villaDTO = _mapper.Map<VillaUpdateDTO>(villa);
            patchDTO.ApplyTo(villaDTO, ModelState);

            Villa model = _mapper.Map<Villa>(villaDTO);
            _ = _db.Villas.Update(model);
            _ = await _db.SaveChangesAsync();

            return ModelState.IsValid switch
            {
                true => NoContent(),
                false => BadRequest("modelstate invalid")
            };
        }
    }
}
