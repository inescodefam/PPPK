using MedApi.DTOs;
using MedCore.Entities;
using MedCore.Enums;
using MedCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MedApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExaminationsController : ControllerBase
    {
        private readonly IExaminationRepository _examinationRepository;

        public ExaminationsController(IExaminationRepository examinationRepository)
        {
            _examinationRepository = examinationRepository;
        }

        // GET: api/examinations
        [HttpGet]
        public ActionResult<List<ExaminationDto>> GetAllExaminations()
        {
            var examinations = _examinationRepository.GetAll();
            return Ok(examinations);
        }

        // GET: api/examinations/5
        [HttpGet("{id}")]
        public ActionResult<ExaminationDto> GetExamination(int id)
        {
            var examination = _examinationRepository.GetById(id);

            if (examination == null)
                return NotFound($"Examination with ID {id} not found.");

            return Ok(examination);
        }

        // GET: api/examinations/patient/5
        [HttpGet("patient/{patientId}")]
        public ActionResult<List<ExaminationDto>> GetByPatient(int patientId)
        {
            var examinations = _examinationRepository.GetByPatientId(patientId);
            return Ok(examinations);
        }

        // GET: api/examinations/doctor/5
        [HttpGet("doctor/{doctorId}")]
        public ActionResult<List<ExaminationDto>> GetByDoctor(int doctorId)
        {
            var examinations = _examinationRepository.GetByDoctorId(doctorId);
            return Ok(examinations);
        }

        // GET: api/examinations/status/Scheduled
        [HttpGet("status/{status}")]
        public ActionResult<List<ExaminationDto>> GetByStatus(string status)
        {
            var examinations = _examinationRepository.GetByStatus(status);
            return Ok(examinations);
        }

        // GET: api/examinations/5/details
        [HttpGet("{id}/details")]
        public ActionResult<ExaminationDto> GetExaminationWithDetails(int id)
        {
            var examination = _examinationRepository.GetExaminationWithDetails(id);

            if (examination == null)
                return NotFound($"Examination with ID {id} not found.");

            return Ok(examination);
        }

        // POST: api/examinations
        [HttpPost]
        public ActionResult<ExaminationDto> CreateExamination([FromBody] ExaminationDto examinationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Examination examination = new Examination
            {
                PatientId = examinationDto.PatientId,
                DoctorId = examinationDto.DoctorId,
                ExaminationType = examinationDto.ExaminationType,
                ScheduledDateTime = examinationDto.ScheduledDateTime,
                Status = examinationDto.Status,
                Findings = examinationDto.Findings,
                Notes = examinationDto.Notes
            };

            try
            {
                _examinationRepository.Add(examination);
                _examinationRepository.Save();
            }
            catch (Exception e)
            {
                return StatusCode(500, "An error occurred while creating the examination.\n " + e.Message);
            }

            return CreatedAtAction(nameof(GetExamination), new { id = examination.Id }, examination);
        }

        // PUT: api/examinations/5
        [HttpPut("{id}")]
        public ActionResult UpdateExamination(int id, [FromBody] ExaminationDto examinationDto)
        {
            var existing = _examinationRepository.GetById(id);
            if (existing == null)
                return NotFound($"Examination with ID {id} not found.");

            Examination examination = new Examination
            {
                Id = id,
                PatientId = examinationDto.PatientId,
                DoctorId = examinationDto.DoctorId,
                ExaminationType = examinationDto.ExaminationType,
                ScheduledDateTime = examinationDto.ScheduledDateTime,
                Status = examinationDto.Status,
                Findings = examinationDto.Findings,
                Notes = examinationDto.Notes
            };

            _examinationRepository.Update(examination);
            _examinationRepository.Save();

            return NoContent();
        }

        // DELETE: api/examinations/5
        [HttpDelete("{id}")]
        public ActionResult DeleteExamination(int id)
        {
            var examination = _examinationRepository.GetById(id);

            if (examination == null)
                return NotFound($"Examination with ID {id} not found.");

            _examinationRepository.Delete(examination);
            _examinationRepository.Save();

            return NoContent();
        }
    }
}
