using MedApi.DTOs;
using MedCore.Entities;
using MedCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicationsController : ControllerBase
    {
        private readonly IMedicationRepository _medicationRepository;

        public MedicationsController(IMedicationRepository medicationRepository)
        {
            _medicationRepository = medicationRepository;
        }

        // GET: api/medications
        [HttpGet]
        public ActionResult<List<MedicationDto>> GetAllMedications()
        {
            var medications = _medicationRepository.GetAll();
            return Ok(medications);
        }

        // GET: api/medications/5
        [HttpGet("{id}")]
        public ActionResult<MedicationDto> GetMedication(int id)
        {
            var medication = _medicationRepository.GetById(id);

            if (medication == null)
                return NotFound($"Medication with ID {id} not found.");

            return Ok(medication);
        }

        // GET: api/medications/name/Aspirin
        [HttpGet("name/{name}")]
        public ActionResult<MedicationDto> GetByName(string name)
        {
            var medication = _medicationRepository.GetByName(name);

            if (medication == null)
                return NotFound($"Medication with name '{name}' not found.");

            return Ok(medication);
        }

        // GET: api/medications/search?name=aspirin
        [HttpGet("search")]
        public ActionResult<List<MedicationDto>> SearchByName([FromQuery] string name)
        {
            var medications = _medicationRepository.SearchByName(name);
            return Ok(medications);
        }

        // GET: api/medications/5/prescriptions
        [HttpGet("{id}/prescriptions")]
        public ActionResult<MedicationDto> GetMedicationWithPrescriptions(int id)
        {
            var medication = _medicationRepository.GetMedicationWithPrescriptions(id);

            if (medication == null)
                return NotFound($"Medication with ID {id} not found.");

            return Ok(medication);
        }

        // POST: api/medications
        [HttpPost]
        public ActionResult<MedicationDto> CreateMedication([FromBody] MedicationDto medicationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = _medicationRepository.GetByName(medicationDto.Name);
            if (existing != null)
                return Conflict($"Medication with name '{medicationDto.Name}' already exists.");

            Medication medication = new Medication
            {
                Name = medicationDto.Name,
                ActiveIngredient = medicationDto.ActiveIngredient,
                Manufacturer = medicationDto.Manufacturer,
                Form = medicationDto.Form
            };

            try
            {
                _medicationRepository.Add(medication);
                _medicationRepository.Save();
            }
            catch (Exception e)
            {
                return StatusCode(500, "An error occurred while creating the medication.\n " + e.Message);
            }

            return CreatedAtAction(nameof(GetMedication), new { id = medication.Id }, medication);
        }

        // PUT: api/medications/5
        [HttpPut("{id}")]
        public ActionResult UpdateMedication(int id, [FromBody] MedicationDto medicationDto)
        {
            var existing = _medicationRepository.GetById(id);
            if (existing == null)
                return NotFound($"Medication with ID {id} not found.");

            Medication medication = new Medication
            {
                Id = id,
                Name = medicationDto.Name,
                ActiveIngredient = medicationDto.ActiveIngredient,
                Manufacturer = medicationDto.Manufacturer,
                Form = medicationDto.Form
            };

            _medicationRepository.Update(medication);
            _medicationRepository.Save();

            return NoContent();
        }

        // DELETE: api/medications/5
        [HttpDelete("{id}")]
        public ActionResult DeleteMedication(int id)
        {
            var medication = _medicationRepository.GetById(id);

            if (medication == null)
                return NotFound($"Medication with ID {id} not found.");

            _medicationRepository.Delete(medication);
            _medicationRepository.Save();

            return NoContent();
        }
    }
}
