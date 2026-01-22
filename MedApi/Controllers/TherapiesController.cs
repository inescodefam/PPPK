using MedApi.DTOs;
using MedCore.Entities;
using MedCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TherapiesController : ControllerBase
    {
        private readonly ITherapyRepository _therapyRepository;

        public TherapiesController(ITherapyRepository therapyRepository)
        {
            _therapyRepository = therapyRepository;
        }

        // GET: api/therapies
        [HttpGet]
        public ActionResult<List<TherapyDto>> GetAllTherapies()
        {
            var therapies = _therapyRepository.GetAll();
            return Ok(therapies);
        }

        // GET: api/therapies/5
        [HttpGet("{id}")]
        public ActionResult<TherapyDto> GetTherapy(int id)
        {
            var therapy = _therapyRepository.GetById(id);

            if (therapy == null)
                return NotFound($"Therapy with ID {id} not found.");

            return Ok(therapy);
        }

        // GET: api/therapies/patient/5
        [HttpGet("patient/{patientId}")]
        public ActionResult<List<TherapyDto>> GetByPatient(int patientId)
        {
            var therapies = _therapyRepository.GetByPatientId(patientId);
            return Ok(therapies);
        }

        // GET: api/therapies/active
        [HttpGet("active")]
        public ActionResult<List<TherapyDto>> GetActiveTherapies()
        {
            var therapies = _therapyRepository.GetActiveTherapies();
            return Ok(therapies);
        }

        // GET: api/therapies/5/prescriptions
        [HttpGet("{id}/prescriptions")]
        public ActionResult<TherapyDto> GetTherapyWithPrescriptions(int id)
        {
            var therapy = _therapyRepository.GetTherapyWithPrescriptions(id);

            if (therapy == null)
                return NotFound($"Therapy with ID {id} not found.");

            return Ok(therapy);
        }

        // POST: api/therapies
        [HttpPost]
        public ActionResult<TherapyDto> CreateTherapy([FromBody] TherapyDto therapyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Therapy therapy = new Therapy
            {
                PatientId = therapyDto.PatientId,
                TherapyName = therapyDto.TherapyName,
                Diagnosis = therapyDto.Diagnosis,
                StartDate = therapyDto.StartDate,
                EndDate = therapyDto.EndDate,
                IsActive = therapyDto.IsActive
            };

            try
            {
                _therapyRepository.Add(therapy);
                _therapyRepository.Save();
            }
            catch (Exception e)
            {
                return StatusCode(500, "An error occurred while creating the therapy.\n " + e.Message);
            }

            return CreatedAtAction(nameof(GetTherapy), new { id = therapy.Id }, therapy);
        }

        // PUT: api/therapies/5
        [HttpPut("{id}")]
        public ActionResult UpdateTherapy(int id, [FromBody] TherapyDto therapyDto)
        {
            var existing = _therapyRepository.GetById(id);
            if (existing == null)
                return NotFound($"Therapy with ID {id} not found.");

            Therapy therapy = new Therapy
            {
                Id = id,
                PatientId = therapyDto.PatientId,
                TherapyName = therapyDto.TherapyName,
                Diagnosis = therapyDto.Diagnosis,
                StartDate = therapyDto.StartDate,
                EndDate = therapyDto.EndDate,
                IsActive = therapyDto.IsActive
            };

            _therapyRepository.Update(therapy);
            _therapyRepository.Save();

            return NoContent();
        }

        // DELETE: api/therapies/5
        [HttpDelete("{id}")]
        public ActionResult DeleteTherapy(int id)
        {
            var therapy = _therapyRepository.GetById(id);

            if (therapy == null)
                return NotFound($"Therapy with ID {id} not found.");

            _therapyRepository.Delete(therapy);
            _therapyRepository.Save();

            return NoContent();
        }
    }
}
