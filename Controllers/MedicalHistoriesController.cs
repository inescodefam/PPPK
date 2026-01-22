using MedApi.DTOs;
using MedCore.Entities;
using MedCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MedicalHistoriesController : ControllerBase
    {
        private readonly IMedicalHistoryRepository _medicalHistoryRepository;

        public MedicalHistoriesController(IMedicalHistoryRepository medicalHistoryRepository)
        {
            _medicalHistoryRepository = medicalHistoryRepository;
        }

        // GET: api/medicalhistories
        [HttpGet]
        public ActionResult<List<MedicalHistoryDto>> GetAllMedicalHistories()
        {
            var histories = _medicalHistoryRepository.GetAll();
            return Ok(histories);
        }

        // GET: api/medicalhistories/5
        [HttpGet("{id}")]
        public ActionResult<MedicalHistoryDto> GetMedicalHistory(int id)
        {
            var history = _medicalHistoryRepository.GetById(id);

            if (history == null)
                return NotFound($"Medical history with ID {id} not found.");

            return Ok(history);
        }

        // GET: api/medicalhistories/patient/5
        [HttpGet("patient/{patientId}")]
        public ActionResult<List<MedicalHistoryDto>> GetByPatient(int patientId)
        {
            var histories = _medicalHistoryRepository.GetByPatientId(patientId);
            return Ok(histories);
        }

        // GET: api/medicalhistories/search?diseaseName=diabetes
        [HttpGet("search")]
        public ActionResult<List<MedicalHistoryDto>> SearchByDiseaseName([FromQuery] string diseaseName)
        {
            var histories = _medicalHistoryRepository.SearchByDiseaseName(diseaseName);
            return Ok(histories);
        }

        // POST: api/medicalhistories
        [HttpPost]
        public ActionResult<MedicalHistoryDto> CreateMedicalHistory([FromBody] MedicalHistoryDto historyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            MedicalHistory history = new MedicalHistory
            {
                PatientId = historyDto.PatientId,
                DiseaseName = historyDto.DiseaseName,
                StartDate = historyDto.StartDate,
                EndDate = historyDto.EndDate,
                Notes = historyDto.Notes
            };

            try
            {
                _medicalHistoryRepository.Add(history);
                _medicalHistoryRepository.Save();
            }
            catch (Exception e)
            {
                return StatusCode(500, "An error occurred while creating the medical history.\n " + e.Message);
            }

            return CreatedAtAction(nameof(GetMedicalHistory), new { id = history.Id }, history);
        }

        // PUT: api/medicalhistories/5
        [HttpPut("{id}")]
        public ActionResult UpdateMedicalHistory(int id, [FromBody] MedicalHistoryDto historyDto)
        {
            var existing = _medicalHistoryRepository.GetById(id);
            if (existing == null)
                return NotFound($"Medical history with ID {id} not found.");

            MedicalHistory history = new MedicalHistory
            {
                Id = id,
                PatientId = historyDto.PatientId,
                DiseaseName = historyDto.DiseaseName,
                StartDate = historyDto.StartDate,
                EndDate = historyDto.EndDate,
                Notes = historyDto.Notes
            };

            _medicalHistoryRepository.Update(history);
            _medicalHistoryRepository.Save();

            return NoContent();
        }

        // DELETE: api/medicalhistories/5
        [HttpDelete("{id}")]
        public ActionResult DeleteMedicalHistory(int id)
        {
            var history = _medicalHistoryRepository.GetById(id);

            if (history == null)
                return NotFound($"Medical history with ID {id} not found.");

            _medicalHistoryRepository.Delete(history);
            _medicalHistoryRepository.Save();

            return NoContent();
        }
    }
}
